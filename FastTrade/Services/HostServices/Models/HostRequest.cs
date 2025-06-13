namespace FastTrade.Services.HostServices.Models
{
    public class HostRequest
    {
        public string DataType { get; set; } = string.Empty;
        public string Data { get; set; } = string.Empty;
        public string? ConnectionString { get; set; }
        public string ReportTemplate { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string? PrinterName { get; set; }
    }
}
