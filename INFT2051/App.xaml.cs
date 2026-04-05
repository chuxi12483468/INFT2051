using System.Globalization;
using Microsoft.Extensions.DependencyInjection;

namespace INFT2051
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            var culture = new CultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}