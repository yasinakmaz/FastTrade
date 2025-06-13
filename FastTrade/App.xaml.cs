namespace FastTrade
{
    public partial class App : Application
    {
        public App()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NNaF5cXmBCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdmWXpcc3RRRGNYUUNwXUBWYUA=");
            InitializeComponent();
        }

        protected override void OnStart()
        {
            base.OnStart();
            var vm = new LoadPublicSettings();
            vm.GetLoad();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}