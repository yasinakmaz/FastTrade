namespace FastTrade.Services
{
    public interface IPrinterService
    {
        Task<List<string>> GetAvailablePrintersAsync();
        Task<string?> GetDefaultPrinterAsync();
        Task<bool> IsPrinterAvailableAsync(string printerName);
        Task<bool> TestPrinterAsync(string printerName);
    }
}