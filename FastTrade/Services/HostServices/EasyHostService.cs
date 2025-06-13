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
        /// <param name="filePath">JSON veya XML dosya yolu</param>
        /// <param name="format">DataFormat.JSON veya DataFormat.XML</param>
        /// <param name="printerName">Yazıcı adı (null ise varsayılan kullanılır)</param>
        /// <param name="designPath">Tasarım dosyası yolu (null ise varsayılan kullanılır)</param>
        /// <param name="design">Tasarım modunda açılsın mı?</param>
        /// <param name="print">Yazdırılsın mı?</param>
        /// <returns>HostResponse</returns>
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

                // Format doğrulaması
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
        /// <param name="data">JSON veya XML string verisi</param>
        /// <param name="format">DataFormat.JSON veya DataFormat.XML</param>
        /// <param name="printerName">Yazıcı adı (null ise varsayılan kullanılır)</param>
        /// <param name="designPath">Tasarım dosyası yolu (null ise varsayılan kullanılır)</param>
        /// <param name="design">Tasarım modunda açılsın mı?</param>
        /// <param name="print">Yazdırılsın mı?</param>
        /// <returns>HostResponse</returns>
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

                // Format doğrulaması
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
    }
}

        /// <summary>
        /// Object'i JSON'a çevirip host'a gönderir
        /// </summary>
        /// <param name="dataObject">JSON'a çevrilecek object</param>
        /// <param name="printerName">Yazıcı adı (null ise varsayılan kullanılır)</param>
        /// <param name="designPath">Tasarım dosyası yolu (null ise varsayılan kullanılır)</param>
        /// <param name="design">Tasarım modunda