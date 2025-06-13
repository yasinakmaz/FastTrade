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

            RegisterPlatformServices(builder.Services);

            RegisterViewModels(builder.Services);

            RegisterViews(builder.Services);

#if DEBUG
            builder.Logging.AddDebug();
#endif
            return builder.Build();
        }

        private static void RegisterPlatformServices(IServiceCollection services)
        {
#if WINDOWS
            services.AddSingleton<IPrinterService, PrinterService>();
            services.AddSingleton<IHostService, HostService>();
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
}