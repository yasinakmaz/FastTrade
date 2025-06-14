namespace FastTrade.Services.HostServices
{
    public class EasyHostService : IDisposable
    {
        private readonly IHostService _hostService;
        private readonly ILogger<EasyHostService>? _logger;

        public EasyHostService(IHostService hostService, ILogger<EasyHostService>? logger = null)
        {
            _hostService = hostService ?? throw new ArgumentNullException(nameof(hostService));
            _logger = logger;
        }

        /// <summary>
        /// Dosyadan veri okuyup host'a gönderir
        /// </summary>
        public async Task<HostResponse> PushHostFromFileAsync(string filePath, DataFormat format,
            string? printerName = null, string? designPath = null, bool design = false, bool print = true)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    return new HostResponse
                    {
                        Success = false,
                        Message = $"File not found: {filePath}"
                    };
                }

                var fileContent = await File.ReadAllTextAsync(filePath);

                if (!await ValidateDataFormatAsync(fileContent, format))
                {
                    return new HostResponse
                    {
                        Success = false,
                        Message = $"Invalid {format} format in file: {filePath}"
                    };
                }

                _logger?.LogInformation($"Pushing data from file: {filePath}, Format: {format}");

                return await _hostService.PushHostAsync(fileContent, format, printerName, designPath, design, print);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"Error pushing data from file: {filePath}");
                return new HostResponse
                {
                    Success = false,
                    Message = $"Error reading file: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// String veriyi doğrudan host'a gönderir
        /// </summary>
        public async Task<HostResponse> PushHostAsync(string data, DataFormat format,
            string? printerName = null, string? designPath = null, bool design = false, bool print = true)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(data))
                {
                    return new HostResponse
                    {
                        Success = false,
                        Message = "Data cannot be null or empty"
                    };
                }

                if (!await ValidateDataFormatAsync(data, format))
                {
                    return new HostResponse
                    {
                        Success = false,
                        Message = $"Invalid {format} format"
                    };
                }

                _logger?.LogInformation($"Pushing data, Format: {format}, Length: {data.Length}");

                return await _hostService.PushHostAsync(data, format, printerName, designPath, design, print);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error pushing data");
                return new HostResponse
                {
                    Success = false,
                    Message = $"Error: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Object'i JSON'a çevirip host'a gönderir
        /// </summary>
        public async Task<HostResponse> PushHostFromObjectAsync(object dataObject,
            string? printerName = null, string? designPath = null, bool design = false, bool print = true)
        {
            try
            {
                if (dataObject == null)
                {
                    return new HostResponse
                    {
                        Success = false,
                        Message = "Data object cannot be null"
                    };
                }

                var jsonData = JsonSerializer.Serialize(dataObject, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                });

                _logger?.LogInformation($"Pushing object as JSON, Type: {dataObject.GetType().Name}");

                return await _hostService.PushHostAsync(jsonData, DataFormat.JSON, printerName, designPath, design, print);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error pushing object as JSON");
                return new HostResponse
                {
                    Success = false,
                    Message = $"Error serializing object: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// SQL sorgusu çalıştırır ve sonucu host'a gönderir
        /// </summary>
        public async Task<HostResponse> PushHostFromSqlAsync(string sqlQuery, string connectionString,
            string? printerName = null, string? designPath = null, bool design = false, bool print = true)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(sqlQuery))
                {
                    return new HostResponse
                    {
                        Success = false,
                        Message = "SQL query cannot be null or empty"
                    };
                }

                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    return new HostResponse
                    {
                        Success = false,
                        Message = "Connection string cannot be null or empty"
                    };
                }

                _logger?.LogInformation($"Pushing SQL query, Length: {sqlQuery.Length}");

                var request = new HostRequest
                {
                    DataType = "sql",
                    Data = sqlQuery,
                    ConnectionString = connectionString,
                    ReportTemplate = designPath ?? "",
                    Action = design ? "design" : (print ? "print" : "export"),
                    PrinterName = printerName
                };

                return await _hostService.SendRequestAsync(request);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error pushing SQL query");
                return new HostResponse
                {
                    Success = false,
                    Message = $"Error: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Dosya seçici açar ve seçilen dosyayı host'a gönderir
        /// </summary>
        public async Task<HostResponse> PushHostFromFilePickerAsync(
            string? printerName = null, string? designPath = null, bool design = false, bool print = true)
        {
            try
            {
                var customFileTypes = new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.WinUI, new[] { ".json", ".xml" } },
                    { DevicePlatform.Android, new[] { "application/json", "text/xml" } },
                    { DevicePlatform.iOS, new[] { "public.json", "public.xml" } }
                };

                var customFileType = new FilePickerFileType(customFileTypes);

                var result = await FilePicker.Default.PickAsync(new PickOptions
                {
                    PickerTitle = "JSON veya XML Dosyası Seçin",
                    FileTypes = customFileType
                });

                if (result == null)
                {
                    return new HostResponse
                    {
                        Success = false,
                        Message = "File selection cancelled"
                    };
                }

                var extension = Path.GetExtension(result.FileName).ToLower();
                var format = extension switch
                {
                    ".json" => DataFormat.JSON,
                    ".xml" => DataFormat.XML,
                    _ => throw new NotSupportedException($"Unsupported file format: {extension}")
                };

                return await PushHostFromFileAsync(result.FullPath, format, printerName, designPath, design, print);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error in file picker operation");
                return new HostResponse
                {
                    Success = false,
                    Message = $"File picker error: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Veri formatını doğrular
        /// </summary>
        private async Task<bool> ValidateDataFormatAsync(string data, DataFormat format)
        {
            return await Task.Run(() =>
            {
                try
                {
                    switch (format)
                    {
                        case DataFormat.JSON:
                            JsonDocument.Parse(data);
                            return true;

                        case DataFormat.XML:
                            System.Xml.Linq.XDocument.Parse(data);
                            return true;

                        case DataFormat.SQL:
                            return !string.IsNullOrWhiteSpace(data);

                        default:
                            return false;
                    }
                }
                catch (Exception ex)
                {
                    _logger?.LogWarning(ex, $"Data format validation failed for {format}");
                    return false;
                }
            });
        }

        public bool IsConnected => _hostService.IsConnected;

        public async Task<bool> ConnectAsync(string hostIP, int hostPort, int timeoutMs = 5000)
        {
            return await _hostService.ConnectAsync(hostIP, hostPort, timeoutMs);
        }

        public async Task DisconnectAsync()
        {
            await _hostService.DisconnectAsync();
        }

        public async Task<HostResponse> TestConnectionAsync(string hostIP, int hostPort)
        {
            try
            {
                var connected = await ConnectAsync(hostIP, hostPort, 5000);

                if (connected)
                {
                    await DisconnectAsync();
                    return new HostResponse
                    {
                        Success = true,
                        Message = "Connection test successful"
                    };
                }
                else
                {
                    return new HostResponse
                    {
                        Success = false,
                        Message = "Connection test failed"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error testing connection");
                return new HostResponse
                {
                    Success = false,
                    Message = $"Connection test error: {ex.Message}"
                };
            }
        }

        public void Dispose()
        {
            _hostService?.Dispose();
        }
    }
}