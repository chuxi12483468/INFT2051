using Microsoft.Maui.Storage;

namespace INFT2051;

public static class ReminderSettings
{
    private const string ReminderEnabledKey = "reminder_enabled";
    private const string ReminderHourKey = "reminder_hour";
    private const string ReminderMinuteKey = "reminder_minute";

    public static bool IsReminderEnabled()
    {
        return Preferences.Default.Get(ReminderEnabledKey, false);
    }

    public static void SetReminderEnabled(bool enabled)
    {
        Preferences.Default.Set(ReminderEnabledKey, enabled);
    }

    public static TimeSpan GetReminderTime()
    {
        int hour = Preferences.Default.Get(ReminderHourKey, 20);
        int minute = Preferences.Default.Get(ReminderMinuteKey, 0);
        return new TimeSpan(hour, minute, 0);
    }

    public static void SetReminderTime(TimeSpan time)
    {
        Preferences.Default.Set(ReminderHourKey, time.Hours);
        Preferences.Default.Set(ReminderMinuteKey, time.Minutes);
    }
}