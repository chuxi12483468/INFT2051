using Microsoft.Maui.Storage;

namespace INFT2051;

public static class PinManager
{
    private const string PinKey = "user_pin";
    private const string DefaultPin = "1234";

    private const string SecurityAnswerKey = "security_answer";
    private const string DefaultSecurityAnswer = "blue";

    public static string GetPin()
    {
        return Preferences.Default.Get(PinKey, DefaultPin);
    }

    public static void SetPin(string newPin)
    {
        Preferences.Default.Set(PinKey, newPin);
    }

    public static void ResetPin()
    {
        Preferences.Default.Set(PinKey, DefaultPin);
    }

    public static string GetSecurityAnswer()
    {
        return Preferences.Default.Get(SecurityAnswerKey, DefaultSecurityAnswer);
    }

    public static void SetSecurityAnswer(string answer)
    {
        Preferences.Default.Set(SecurityAnswerKey, (answer ?? string.Empty).Trim().ToLower());
    }

    public static bool VerifySecurityAnswer(string answer)
    {
        string savedAnswer = GetSecurityAnswer();
        string inputAnswer = (answer ?? string.Empty).Trim().ToLower();
        return savedAnswer == inputAnswer;
    }
}