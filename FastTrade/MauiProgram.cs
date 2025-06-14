namespace FastTrade
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureSyncfusionCore()
                .ConfigureSyncfusionToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("EthosNova-Bold.ttf", "EthosNovaBold");
                    fonts.AddFont("EthosNova-Medium.ttf", "EthosNovaMedium");
                    fonts.AddFont("EthosNova-Heavy.ttf", "EthosNovaHeavy");
                    fonts.AddFont("EthosNova-Regular.ttf", "EthosNovaRegular");
                });

            RegisterServices(builder.Services);
            RegisterViewModels(builder.Services);
            RegisterViews(builder.Services);

#if DEBUG
            builder.Logging.AddDebug();
#endif
            return builder.Build();
        }

        private static void RegisterServices(IServiceCollection services)
        {
            services.AddDbContext<ProductDbContext>();
            services.AddSingleton<ISettingsService, FastTrade.Services.Settings.SettingsService>();
            services.AddSingleton<IHostService, FastTrade.Services.HostServices.HostService>();
            services.AddSingleton<FastTrade.Services.HostServices.EasyHostService>();
#if WINDOWS
            services.AddSingleton<IPrinterService, FastTrade.Platforms.Windows.PrinterService>();
#else
            services.AddSingleton<IPrinterService, DefaultPrinterService>();
#endif
        }

        private static void RegisterViewModels(IServiceCollection services)
        {
            services.AddTransient<SaveSettingsViewModel>();
            services.AddTransient<LoadPublicSettings>();
        }

        private static void RegisterViews(IServiceCollection services)
        {
            services.AddTransient<FastTrade.Views.Settings.SettingsPage>();
        }
    }

    public class DefaultPrinterService : IPrinterService
    {
        public Task<List<string>> GetAvailablePrintersAsync()
        {
            return Task.FromResult(new List<string> { "Default Printer" });
        }

        public Task<string?> GetDefaultPrinterAsync()
        {
            return Task.FromResult<string?>("Default Printer");
        }

        public Task<bool> IsPrinterAvailableAsync(string printerName)
        {
            return Task.FromResult(true);
        }

        public Task<bool> TestPrinterAsync(string printerName)
        {
            return Task.FromResult(true);
        }
    }
}