namespace FastTrade.Services.HostServices
{
    public static class HostServiceExtensions
    {
        /// <summary>
        /// Kolay kullanım için extension method
        /// </summary>
        public static async Task<HostResponse> PushAsync(this EasyHostService service,
            string data, string format, string? printer = null, string? design = null,
            bool isDesign = false, bool isPrint = true)
        {
            var dataFormat = format.ToLower() switch
            {
                "json" => DataFormat.JSON,
                "xml" => DataFormat.XML,
                "sql" => DataFormat.SQL,
                _ => throw new ArgumentException($"Unsupported format: {format}")
            };

            return await service.PushHostAsync(data, dataFormat, printer, design, isDesign, isPrint);
        }

        /// <summary>
        /// File path ile kolay kullanım
        /// </summary>
        public static async Task<HostResponse> PushFromFileAsync(this EasyHostService service,
            string filePath, string? printer = null, string? design = null,
            bool isDesign = false, bool isPrint = true)
        {
            var extension = Path.GetExtension(filePath).ToLower();
            var format = extension switch
            {
                ".json" => DataFormat.JSON,
                ".xml" => DataFormat.XML,
                _ => throw new ArgumentException($"Unsupported file extension: {extension}")
            };

            return await service.PushHostFromFileAsync(filePath, format, printer, design, isDesign, isPrint);
        }

        /// <summary>
        /// Settings'den connection string oluştur
        /// </summary>
        public static async Task<string> GetConnectionStringAsync(this ISettingsService settingsService)
        {
            try
            {
                var server = await settingsService.GetStringAsync(PublicServices.MSSQLServer);
                var username = await settingsService.GetStringAsync(PublicServices.MSSQLUSERNAME);
                var password = await settingsService.GetStringAsync(PublicServices.MSSQLPASSWORD);
                var database = await settingsService.GetStringAsync(PublicServices.MSSQLDATABASE);

                if (string.IsNullOrEmpty(server) || string.IsNullOrEmpty(username) ||
                    string.IsNullOrEmpty(password) || string.IsNullOrEmpty(database))
                {
                    return "";
                }

                return $"Server={server};Database={database};User Id={username};Password={password};Connection Timeout=30;TrustServerCertificate=True;Encrypt=False;";
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// Host ayarlarını al
        /// </summary>
        public static async Task<(string ip, int port)> GetHostSettingsAsync(this ISettingsService settingsService)
        {
            try
            {
                var hostIP = await settingsService.GetStringAsync(PublicServices.HOSTIP) ?? "";
                var hostPortStr = await settingsService.GetStringAsync(PublicServices.HOSTPORT) ?? "8080";

                if (!int.TryParse(hostPortStr, out var hostPort))
                {
                    hostPort = 8080;
                }

                return (hostIP, hostPort);
            }
            catch
            {
                return ("", 8080);
            }
        }
    }
}
