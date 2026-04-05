using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;

namespace INFT2051;

[BroadcastReceiver(Enabled = true, Exported = false)]
public class ReminderReceiver : BroadcastReceiver
{
    public const string ChannelId = "daily_reminder_channel";
    public const int NotificationId = 1001;

    public override void OnReceive(Context? context, Intent? intent)
    {
        if (context == null)
            return;

        CreateNotificationChannel(context);

        Intent openIntent = new Intent(context, typeof(MainActivity));
        openIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);

        PendingIntent pendingIntent = PendingIntent.GetActivity(
            context,
            0,
            openIntent,
            PendingIntentFlags.Immutable | PendingIntentFlags.UpdateCurrent);

        var builder = new NotificationCompat.Builder(context, ChannelId)
            .SetSmallIcon(Resource.Mipmap.appicon)
            .SetContentTitle("Daily Reminder")
            .SetContentText("Time to write your diary.")
            .SetAutoCancel(true)
            .SetContentIntent(pendingIntent)
            .SetPriority((int)NotificationPriority.High);

        var manager = NotificationManagerCompat.From(context);
        manager.Notify(NotificationId, builder.Build());

        try
        {
            Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(500));
        }
        catch
        {
        }

        if (ReminderSettings.IsReminderEnabled())
        {
            var savedTime = ReminderSettings.GetReminderTime();
            ReminderService.ScheduleDailyReminder(savedTime);
        }
    }

    private static void CreateNotificationChannel(Context context)
    {
        if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            return;

        var channel = new NotificationChannel(
            ChannelId,
            "Daily Reminder",
            NotificationImportance.High)
        {
            Description = "Daily diary reminder notifications"
        };

        var notificationManager =
            (NotificationManager?)context.GetSystemService(Context.NotificationService);

        notificationManager?.CreateNotificationChannel(channel);
    }
}