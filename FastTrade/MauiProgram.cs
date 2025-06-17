namespace FastTrade
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseMauiCommunityToolkitCore()
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

            ConfigureMssqlDbContext(builder.Services);
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
            services.AddSingleton<ISettingsService, FastTrade.Services.Settings.SettingsService>();
            services.AddSingleton<IHostService, FastTrade.Services.HostServices.HostService>();
            services.AddSingleton<FastTrade.Services.HostServices.EasyHostService>();
#if WINDOWS
            services.AddSingleton<IPrinterService, FastTrade.Platforms.Windows.PrinterService>();
#else
            services.AddSingleton<IPrinterService, DefaultPrinterService>();
#endif
        }

        private static void ConfigureMssqlDbContext(IServiceCollection services)
        {
            services.AddDbContextFactory<ProductDbContext>(options =>
            {
                options.UseSqlServer(sqloptions =>
                {
                    sqloptions.CommandTimeout(30);

                    sqloptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorNumbersToAdd: null);

                    sqloptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                })

                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                .EnableServiceProviderCaching(true)
                .EnableDetailedErrors(true)

                .ConfigureWarnings(warnings =>
                {
                    warnings.Ignore(CoreEventId.RowLimitingOperationWithoutOrderByWarning);
                    warnings.Ignore(SqlServerEventId.DecimalTypeDefaultWarning);
                    warnings.Log(CoreEventId.SensitiveDataLoggingEnabledWarning);
                });
            }, ServiceLifetime.Singleton);

            services.AddDbContextFactory<LoginDbContext>(options =>
            {
                options.UseSqlServer(sqloptions =>
                {
                    sqloptions.CommandTimeout(30);

                    sqloptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorNumbersToAdd: null);

                    sqloptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                })

                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                .EnableServiceProviderCaching(true)
                .EnableDetailedErrors(true)

                .ConfigureWarnings(warnings =>
                {
                    warnings.Ignore(CoreEventId.RowLimitingOperationWithoutOrderByWarning);
                    warnings.Ignore(SqlServerEventId.DecimalTypeDefaultWarning);
                    warnings.Log(CoreEventId.SensitiveDataLoggingEnabledWarning);
                });
            }, ServiceLifetime.Singleton);

            services.AddDbContextFactory<UsersDbContext>(options =>
            {
                options.UseSqlServer(sqloptions =>
                {
                    sqloptions.CommandTimeout(30);

                    sqloptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorNumbersToAdd: null);

                    sqloptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                })

                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                .EnableServiceProviderCaching(true)
                .EnableDetailedErrors(true)

                .ConfigureWarnings(warnings =>
                {
                    warnings.Ignore(CoreEventId.RowLimitingOperationWithoutOrderByWarning);
                    warnings.Ignore(SqlServerEventId.DecimalTypeDefaultWarning);
                    warnings.Log(CoreEventId.SensitiveDataLoggingEnabledWarning);
                });
            }, ServiceLifetime.Singleton);
        }

        private static void RegisterViewModels(IServiceCollection services)
        {
            services.AddScoped<SaveSettingsViewModel>();
            services.AddScoped<LoadPublicSettings>();
            services.AddScoped<CreateManuelProductViewModel>();
            services.AddScoped<ManageStockViewModel>();
            services.AddScoped<LoginViewModel>();
        }

        private static void RegisterViews(IServiceCollection services)
        {
            services.AddScoped<SettingsPage>();
            services.AddScoped<AddManuelStock>();
            services.AddScoped<ManageStock>();
            services.AddScoped<LoginPage>();
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