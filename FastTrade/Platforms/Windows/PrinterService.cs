#if WINDOWS
using System.Drawing.Printing;
using System.Management;

namespace FastTrade.Platforms.Windows
{
    public class PrinterService : IPrinterService
    {
        public async Task<List<string>> GetAvailablePrintersAsync()
        {
            return await Task.Run(() =>
            {
                var printers = new List<string>();
                try
                {
                    using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Printer");
                    foreach (ManagementObject printer in searcher.Get())
                    {
                        var printerName = printer["Name"]?.ToString();
                        var printerStatus = printer["PrinterStatus"];
                        var workOffline = (bool?)printer["WorkOffline"] ?? false;

                        if (!string.IsNullOrEmpty(printerName) && !workOffline)
                        {
                            printers.Add(printerName);
                        }
                    }

                    if (printers.Count == 0)
                    {
                        foreach (string printerName in PrinterSettings.InstalledPrinters)
                        {
                            printers.Add(printerName);
                        }
                    }

                    if (printers.Count == 0)
                    {
                        printers.AddRange(GetDefaultPrinters());
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Windows yazıcılar alınırken hata: {ex.Message}");
                    printers.AddRange(GetDefaultPrinters());
                }
                return printers.Distinct().ToList();
            });
        }

        public async Task<string> GetDefaultPrinterAsync()
        {
            return await Task.Run(() =>
            {
                try
                {
                    using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Printer WHERE Default = True");
                    foreach (ManagementObject printer in searcher.Get())
                    {
                        return printer["Name"]?.ToString() ?? GetFallbackPrinter();
                    }

                    var printerSettings = new PrinterSettings();
                    return printerSettings.PrinterName ?? GetFallbackPrinter();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Windows varsayılan yazıcı alınırken hata: {ex.Message}");
                    return GetFallbackPrinter();
                }
            });
        }

        public async Task<bool> IsPrinterAvailableAsync(string printerName)
        {
            var printers = await GetAvailablePrintersAsync();
            return printers.Any(p => p.Equals(printerName, StringComparison.OrdinalIgnoreCase));
        }

        private static List<string> GetDefaultPrinters()
        {
            return new List<string>
            {
                "Microsoft Print to PDF",
                "Microsoft XPS Document Writer",
                "Fax"
            };
        }

        private static string GetFallbackPrinter()
        {
            return "Microsoft Print to PDF";
        }
    }
}
#endif