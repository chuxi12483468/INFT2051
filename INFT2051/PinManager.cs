using Microsoft.Maui.Storage;

namespace INFT2051;

public static class PinManager
{
    private const string PinKey = "user_pin";

    public static string GetPin()
    {
        return Preferences.Default.Get(PinKey, "1234");
    }

    public static void SetPin(string newPin)
    {
        Preferences.Default.Set(PinKey, newPin);
    }
}