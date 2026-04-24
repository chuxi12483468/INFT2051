using System.Globalization;
using Microsoft.Extensions.DependencyInjection;

namespace INFT2051
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent(); // Initialize components defined in App.xaml

            // Set the default culture for the entire application
            // This ensures consistent formatting (e.g., date, number, language)
            var culture = new CultureInfo("en-US");

            // Apply the culture settings to all threads
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }

        // This method defines the main window of the application
        protected override Window CreateWindow(IActivationState? activationState)
        {
            // Set AppShell as the root navigation container
            return new Window(new AppShell());
        }
    }
}