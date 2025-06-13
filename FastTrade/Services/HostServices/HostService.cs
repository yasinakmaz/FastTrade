namespace FastTrade.Services.HostServices
{
    public class HostService : IHostService, IDisposable
    {
        private readonly ILogger<HostService>? _logger;
        private readonly ISettingsService _settingsService;
        private TcpClient? _tcpClient;
        private NetworkStream? _networkStream;
        private bool _disposed = false;

        public bool IsConnected => _tcpClient?.Connected == true;
        public string HostIP { get; private set; } = string.Empty;
        public int HostPort { get; private set; } = 0;

        public event EventHandler<HostConnectionEventArgs>? ConnectionChanged;
        public event EventHandler<HostErrorEventArgs>? ErrorOccurred;

        public HostService(ISettingsService settingsService, ILogger<HostService>? logger = null)
        {
            _settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));
            _logger = logger;
        }

        public async Task<bool> ConnectAsync(string hostIP, int hostPort, int timeoutMs = 5000)
        {
            try
            {
                if (IsConnected)
                {
                    await DisconnectAsync();
                }

                if (string.IsNullOrWhiteSpace(hostIP))
                    throw new ArgumentException("Host IP cannot be null or empty", nameof(hostIP));

                if (hostPort <= 0 || hostPort > 65535)
                    throw new ArgumentException("Invalid port number", nameof(hostPort));

                _tcpClient = new TcpClient();

                // Timeout ayarları
                _tcpClient.ReceiveTimeout = timeoutMs;
                _tcpClient.SendTimeout = timeoutMs;

                var connectTask = _tcpClient.ConnectAsync(hostIP, hostPort);
                var timeoutTask = Task.Delay(timeoutMs);

                var completedTask = await Task.WhenAny(connectTask, timeoutTask);

                if (completedTask == timeoutTask)
                {
                    _tcpClient?.Close();
                    throw new TimeoutException($"Connection timeout after {timeoutMs}ms");
                }

                await connectTask;

                _networkStream = _tcpClient.GetStream();
                HostIP = hostIP;
                HostPort = hostPort;

                await _settingsService.SetStringAsync(PublicServices.HOSTIP, hostIP);
                await _settingsService.SetIntAsync(PublicServices.HOSTPORT, hostPort);

                _logger?.LogInformation($"Connected to host: {hostIP}:{hostPort}");

                ConnectionChanged?.Invoke(this, new HostConnectionEventArgs
                {
                    IsConnected = true,
                    HostIP = hostIP,
                    HostPort = hostPort,
                    Message = "Successfully connected"
                });

                return true;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"Failed to connect to {hostIP}:{hostPort}");

                ErrorOccurred?.Invoke(this, new HostErrorEventArgs(ex, "Connection failed"));

                ConnectionChanged?.Invoke(this, new HostConnectionEventArgs
                {
                    IsConnected = false,
                    HostIP = hostIP,
                    HostPort = hostPort,
                    Message = $"Connection failed: {ex.Message}"
                });

                await DisconnectAsync();
                return false;
            }
        }

        public async Task DisconnectAsync()
        {
            try
            {
                if (_networkStream != null)
                {
                    await _networkStream.DisposeAsync();
                    _networkStream = null;
                }

                _tcpClient?.Close();
                _tcpClient?.Dispose();
                _tcpClient = null;

                _logger?.LogInformation($"Disconnected from host: {HostIP}:{HostPort}");

                ConnectionChanged?.Invoke(this, new HostConnectionEventArgs
                {
                    IsConnected = false,
                    HostIP = HostIP,
                    HostPort = HostPort,
                    Message = "Disconnected"
                });
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error during disconnect");
                ErrorOccurred?.Invoke(this, new HostErrorEventArgs(ex, "Disconnect error"));
            }
        }

        public async Task<HostResponse> SendRequestAsync(HostRequest request, int timeoutMs = 30000)
        {
            if (!IsConnected)
                throw new InvalidOperationException("Not connected to host");

            if (_networkStream == null)
                throw new InvalidOperationException("Network stream is null");

            try
            {
                // JSON serileştirme
                var requestJson = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = false
                });

                var requestData = Encoding.UTF8.GetBytes(requestJson);
                var lengthBytes = BitConverter.GetBytes(requestData.Length);

                // Veri gönderme
                await _networkStream.WriteAsync(lengthBytes, 0, 4);
                await _networkStream.WriteAsync(requestData, 0, requestData.Length);
                await _networkStream.FlushAsync();

                _logger?.LogDebug($"Sent request: {requestJson}");

                // Yanıt okuma
                var responseLengthBytes = new byte[4];
                await ReadExactAsync(_networkStream, responseLengthBytes, 4);
                var responseLength = BitConverter.ToInt32(responseLengthBytes, 0);

                if (responseLength <= 0 || responseLength > 10485760) // 10MB limit
                    throw new InvalidDataException($"Invalid response length: {responseLength}");

                var responseData = new byte[responseLength];
                await ReadExactAsync(_networkStream, responseData, responseLength);

                var responseJson = Encoding.UTF8.GetString(responseData);
                _logger?.LogDebug($"Received response: {responseJson}");

                var response = JsonSerializer.Deserialize<HostResponse>(responseJson, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                return response ?? new HostResponse { Success = false, Message = "Invalid response format" };
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error sending request to host");
                ErrorOccurred?.Invoke(this, new HostErrorEventArgs(ex, "Request failed"));

                return new HostResponse
                {
                    Success = false,
                    Message = $"Request failed: {ex.Message}"
                };
            }
        }

        public async Task<HostResponse> PushHostAsync(string data, DataFormat format,
            string? printerName = null, string? designPath = null, bool design = false, bool print = true)
        {
            try
            {
                // Settings'den varsayılan değerleri al
                if (string.IsNullOrEmpty(printerName))
                {
                    printerName = await _settingsService.GetStringAsync(PublicServices.HOSTDEFAULTPRINTER);
                }

                if (string.IsNullOrEmpty(designPath))
                {
                    designPath = await _settingsService.GetStringAsync(PublicServices.HOSTDEFAULTDESIGN);
                }

                if (string.IsNullOrEmpty(designPath))
                {
                    throw new ArgumentException("Design path cannot be null or empty. Please set default design or provide designPath parameter.");
                }

                string action;
                if (design && print)
                    action = "design";
                else if (design)
                    action = "design";
                else if (print)
                    action = "print";
                else
                    action = "export";

                // Request oluşturma
                var request = new HostRequest
                {
                    DataType = format.ToString().ToLower(),
                    Data = data,
                    ReportTemplate = Path.GetFileName(designPath),
                    Action = action,
                    PrinterName = printerName
                };

                if (!IsConnected)
                {
                    var hostIP = await _settingsService.GetStringAsync(PublicServices.HOSTIP);
                    var hostPortStr = await _settingsService.GetStringAsync(PublicServices.HOSTPORT);

                    if (string.IsNullOrEmpty(hostIP) || string.IsNullOrEmpty(hostPortStr))
                    {
                        throw new InvalidOperationException("Host connection settings not found. Please configure host settings first.");
                    }

                    if (!int.TryParse(hostPortStr, out var hostPort))
                    {
                        throw new InvalidOperationException("Invalid host port in settings.");
                    }

                    var connected = await ConnectAsync(hostIP, hostPort);
                    if (!connected)
                    {
                        throw new InvalidOperationException("Failed to connect to host.");
                    }
                }

                return await SendRequestAsync(request);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error in PushHostAsync");
                ErrorOccurred?.Invoke(this, new HostErrorEventArgs(ex, "PushHost failed"));

                return new HostResponse
                {
                    Success = false,
                    Message = $"PushHost failed: {ex.Message}"
                };
            }
        }

        private static async Task ReadExactAsync(NetworkStream stream, byte[] buffer, int count)
        {
            int totalRead = 0;

            while (totalRead < count)
            {
                int bytesRead = await stream.ReadAsync(buffer, totalRead, count - totalRead);
                if (bytesRead == 0)
                    throw new EndOfStreamException("Unexpected end of stream");

                totalRead += bytesRead;
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                DisconnectAsync().Wait(5000);
                _disposed = true;
            }
        }
    }
}
