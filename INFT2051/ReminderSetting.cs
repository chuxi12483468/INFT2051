using Microsoft.Maui.Storage;

namespace INFT2051;

// Manages reminder settings using local storage (Preferences)
public static class ReminderSettings
{
    private const string ReminderEnabledKey = "reminder_enabled"; // Key for enabling/disabling reminder
    private const string ReminderHourKey = "reminder_hour"; // Key for storing reminder hour
    private const string ReminderMinuteKey = "reminder_minute"; // Key for storing reminder minute

    // Check whether reminder is enabled
    public static bool IsReminderEnabled()
    {
        return Preferences.Default.Get(ReminderEnabledKey, false);
    }

    // Enable or disable reminder
    public static void SetReminderEnabled(bool enabled)
    {
        Preferences.Default.Set(ReminderEnabledKey, enabled);
    }

    // Retrieve saved reminder time
    public static TimeSpan GetReminderTime()
    {
        int hour = Preferences.Default.Get(ReminderHourKey, 20);   // Default hour = 20 (8 PM)
        int minute = Preferences.Default.Get(ReminderMinuteKey, 0); // Default minute = 0

        return new TimeSpan(hour, minute, 0);
    }

    // Save reminder time (hour and minute separately)
    public static void SetReminderTime(TimeSpan time)
    {
        Preferences.Default.Set(ReminderHourKey, time.Hours);
        Preferences.Default.Set(ReminderMinuteKey, time.Minutes);
    }
}