using Microsoft.Extensions.Logging;

namespace INFT2051
{
    public static class MauiProgram
    {
        // Entry point for configuring and building the MAUI application
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>() // Set the main application class
                .ConfigureFonts(fonts =>
                {
                    // Register custom fonts for use in the app
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Register DiaryDatabase as a singleton service (shared instance)
            builder.Services.AddSingleton<DiaryDatabase>();

#if DEBUG
            // Enable debug logging during development
            builder.Logging.AddDebug();
#endif

            // Build and return the configured app
            return builder.Build();
        }
    }
}