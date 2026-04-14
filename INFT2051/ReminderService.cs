#if ANDROID
using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
#endif

namespace INFT2051;

public static class ReminderService
{
    private const int RequestCode = 2001;

    public static bool CanScheduleDailyReminder()
    {
#if ANDROID
        var context = Android.App.Application.Context;
        var alarmManager = context.GetSystemService(Context.AlarmService) as AlarmManager;

        if (alarmManager == null)
            return false;

        if (Build.VERSION.SdkInt >= BuildVersionCodes.S)
            return alarmManager.CanScheduleExactAlarms();

        return true;
#else
        return true;
#endif
    }

    public static void OpenExactAlarmSettings()
    {
#if ANDROID
        var context = Android.App.Application.Context;

        if (Build.VERSION.SdkInt >= BuildVersionCodes.S)
        {
            Intent intent = new Intent(Settings.ActionRequestScheduleExactAlarm);
            intent.SetData(Android.Net.Uri.Parse($"package:{context.PackageName}"));
            intent.AddFlags(ActivityFlags.NewTask);
            context.StartActivity(intent);
        }
#endif
    }

    public static void ScheduleDailyReminder(TimeSpan time)
    {
#if ANDROID
        var context = Android.App.Application.Context;
        var alarmManager = context.GetSystemService(Context.AlarmService) as AlarmManager;

        if (alarmManager == null)
            return;

        if (Build.VERSION.SdkInt >= BuildVersionCodes.S && !alarmManager.CanScheduleExactAlarms())
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

        DateTime now = DateTime.Now;
        DateTime firstTrigger = new DateTime(
            now.Year,
            now.Month,
            now.Day,
            time.Hours,
            time.Minutes,
            0);

        if (firstTrigger <= now)
            firstTrigger = firstTrigger.AddDays(1);

        long triggerMillis = new DateTimeOffset(firstTrigger).ToUnixTimeMilliseconds();

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

        alarmManager.Cancel(pendingIntent);
#endif
    }
}