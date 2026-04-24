#if ANDROID
using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
#endif

namespace INFT2051;

// Handles scheduling and cancelling daily reminders (Android-specific)
public static class ReminderService
{
    private const int RequestCode = 2001; // Unique request code for PendingIntent

    // Check whether the app is allowed to schedule exact alarms
    public static bool CanScheduleDailyReminder()
    {
#if ANDROID
        var context = Android.App.Application.Context;
        var alarmManager = context.GetSystemService(Context.AlarmService) as AlarmManager;

        if (alarmManager == null)
            return false;

        // Android 12+ requires explicit permission for exact alarms
        if (Build.VERSION.SdkInt >= BuildVersionCodes.S)
            return alarmManager.CanScheduleExactAlarms();

        return true;
#else
        return true;
#endif
    }

    // Open system settings to request exact alarm permission
    public static void OpenExactAlarmSettings()
    {
#if ANDROID
        var context = Android.App.Application.Context;

        if (Build.VERSION.SdkInt >= BuildVersionCodes.S)
        {
            Intent intent = new Intent(Settings.ActionRequestScheduleExactAlarm);

            // Direct user to this app's settings page
            intent.SetData(Android.Net.Uri.Parse($"package:{context.PackageName}"));
            intent.AddFlags(ActivityFlags.NewTask);

            context.StartActivity(intent);
        }
#endif
    }

    // Schedule a daily reminder at a specific time
    public static void ScheduleDailyReminder(TimeSpan time)
    {
#if ANDROID
        var context = Android.App.Application.Context;
        var alarmManager = context.GetSystemService(Context.AlarmService) as AlarmManager;

        if (alarmManager == null)
            return;

        // Check permission for exact alarms (Android 12+)
        if (Build.VERSION.SdkInt >= BuildVersionCodes.S && !alarmManager.CanScheduleExactAlarms())
            return;

        // Create intent to trigger ReminderReceiver
        Intent intent = new Intent(context, typeof(ReminderReceiver));

        var flags = PendingIntentFlags.UpdateCurrent;

        // Required for newer Android versions
        if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
        {
            flags |= PendingIntentFlags.Immutable;
        }

        // Create PendingIntent
        PendingIntent? pendingIntent = PendingIntent.GetBroadcast(
            context,
            RequestCode,
            intent,
            flags);

        if (pendingIntent == null)
            return;

        DateTime now = DateTime.Now;

        // Set the first trigger time today at specified time
        DateTime firstTrigger = new DateTime(
            now.Year,
            now.Month,
            now.Day,
            time.Hours,
            time.Minutes,
            0);

        // If time already passed, schedule for next day
        if (firstTrigger <= now)
            firstTrigger = firstTrigger.AddDays(1);

        // Convert to milliseconds for AlarmManager
        long triggerMillis = new DateTimeOffset(firstTrigger).ToUnixTimeMilliseconds();

        // Use exact alarm depending on Android version
        if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
        {
            alarmManager.SetExactAndAllowWhileIdle(
                AlarmType.RtcWakeup,
                triggerMillis,
                pendingIntent);
        }
        else
        {
            alarmManager.SetExact(
                AlarmType.RtcWakeup,
                triggerMillis,
                pendingIntent);
        }
#endif
    }

    // Cancel scheduled reminder
    public static void CancelDailyReminder()
    {
#if ANDROID
        var context = Android.App.Application.Context;
        var alarmManager = context.GetSystemService(Context.AlarmService) as AlarmManager;

        if (alarmManager == null)
            return;

        Intent intent = new Intent(context, typeof(ReminderReceiver));

        var flags = PendingIntentFlags.UpdateCurrent;

        if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
        {
            flags |= PendingIntentFlags.Immutable;
        }

        PendingIntent? pendingIntent = PendingIntent.GetBroadcast(
            context,
            RequestCode,
            intent,
            flags);

        if (pendingIntent == null)
            return;

        // Cancel the alarm
        alarmManager.Cancel(pendingIntent);
#endif
    }
}