using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;

namespace INFT2051;

// Define a BroadcastReceiver to handle reminder events triggered by the system
[BroadcastReceiver(Enabled = true, Exported = false)]
public class ReminderReceiver : BroadcastReceiver
{
    // Notification channel ID (required for Android 8.0+)
    public const string ChannelId = "daily_reminder_channel";

    // Unique notification ID
    public const int NotificationId = 1001;

    // This method is triggered when the scheduled reminder is received
    public override void OnReceive(Context? context, Intent? intent)
    {
        // Ensure context is not null
        if (context == null)
            return;

        // Create notification channel (required for Android 8.0 and above)
        CreateNotificationChannel(context);

        // Intent to open the app when notification is clicked
        Intent openIntent = new Intent(context, typeof(MainActivity));

        // Ensure only one instance of activity is used
        openIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);

        // Create a PendingIntent for notification click action
        PendingIntent pendingIntent = PendingIntent.GetActivity(
            context,
            0,
            openIntent,
            PendingIntentFlags.Immutable | PendingIntentFlags.UpdateCurrent);

        // Build the notification
        var builder = new NotificationCompat.Builder(context, ChannelId)
            .SetSmallIcon(Resource.Mipmap.appicon) // Notification icon
            .SetContentTitle("Daily Reminder")     // Title
            .SetContentText("Time to write your diary.") // Message
            .SetAutoCancel(true)                  // Remove notification when clicked
            .SetContentIntent(pendingIntent)      // Action when clicked
            .SetPriority((int)NotificationPriority.High); // High priority notification

        // Send the notification
        var manager = NotificationManagerCompat.From(context);
        manager.Notify(NotificationId, builder.Build());

        // Trigger vibration for 500 milliseconds
        try
        {
            Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(500));
        }
        catch
        {
            // Ignore vibration errors (e.g., device does not support vibration)
        }

        // If reminder is still enabled, reschedule the next reminder
        if (ReminderSettings.IsReminderEnabled())
        {
            var savedTime = ReminderSettings.GetReminderTime();
            ReminderService.ScheduleDailyReminder(savedTime);
        }
    }

    // Create notification channel for Android 8.0+ devices
    private static void CreateNotificationChannel(Context context)
    {
        // Only required for Android Oreo (API 26) and above
        if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            return;

        // Define notification channel
        var channel = new NotificationChannel(
            ChannelId,
            "Daily Reminder",
            NotificationImportance.High)
        {
            Description = "Daily diary reminder notifications"
        };

        // Get system notification service
        var notificationManager =
            (NotificationManager?)context.GetSystemService(Context.NotificationService);

        // Register the channel
        notificationManager?.CreateNotificationChannel(channel);
    }
}