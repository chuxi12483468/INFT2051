using Microsoft.Maui.Storage;

namespace INFT2051;

// Manages PIN and security answer using local storage (Preferences)
public static class PinManager
{
    private const string PinKey = "user_pin"; // Key for storing PIN
    private const string DefaultPin = "1234"; // Default PIN

    private const string SecurityAnswerKey = "security_answer"; // Key for security answer
    private const string DefaultSecurityAnswer = "blue"; // Default answer

    // Retrieve the stored PIN (or default if not set)
    public static string GetPin()
    {
        return Preferences.Default.Get(PinKey, DefaultPin);
    }

    // Save a new PIN
    public static void SetPin(string newPin)
    {
        Preferences.Default.Set(PinKey, newPin);
    }

    // Reset PIN to default value
    public static void ResetPin()
    {
        Preferences.Default.Set(PinKey, DefaultPin);
    }

    // Retrieve stored security answer
    public static string GetSecurityAnswer()
    {
        return Preferences.Default.Get(SecurityAnswerKey, DefaultSecurityAnswer);
    }

    // Save security answer (normalized to avoid case and space issues)
    public static void SetSecurityAnswer(string answer)
    {
        Preferences.Default.Set(SecurityAnswerKey, (answer ?? string.Empty).Trim().ToLower());
    }

    // Verify user input against stored answer
    public static bool VerifySecurityAnswer(string answer)
    {
        string savedAnswer = GetSecurityAnswer();

        // Normalize input for comparison
        string inputAnswer = (answer ?? string.Empty).Trim().ToLower();

        return savedAnswer == inputAnswer;
    }
}