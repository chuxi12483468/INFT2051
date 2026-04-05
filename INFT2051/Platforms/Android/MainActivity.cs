using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using AndroidX.Core.App;
using AndroidX.Core.Content;

namespace INFT2051
{
    [Activity(Theme = "@style/Maui.SplashTheme",
              MainLauncher = true,
              LaunchMode = LaunchMode.SingleTop,
              WindowSoftInputMode = SoftInput.AdjustResize,
              ConfigurationChanges = ConfigChanges.ScreenSize
                                   | ConfigChanges.Orientation
                                   | ConfigChanges.UiMode
                                   | ConfigChanges.ScreenLayout
                                   | ConfigChanges.SmallestScreenSize
                                   | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        private const int NotificationPermissionRequestCode = 3001;

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RequestNotificationPermissionIfNeeded();
        }

        private void RequestNotificationPermissionIfNeeded()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.Tiramisu)
                return;

            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.PostNotifications)
                != Permission.Granted)
            {
                ActivityCompat.RequestPermissions(
                    this,
                    new[] { Manifest.Permission.PostNotifications },
                    NotificationPermissionRequestCode);
            }
        }
    }
}