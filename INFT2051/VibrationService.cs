namespace INFT2051;

public static class VibrationService
{
    public static void VibrateShort()
    {
        try
        {
            Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(500));
        }
        catch
        {
            // Some devices or platforms may not support vibration.
        }
    }

    public static void Cancel()
    {
        try
        {
            Vibration.Default.Cancel();
        }
        catch
        {
            // Ignore if vibration cancel is not supported.
        }
    }
}