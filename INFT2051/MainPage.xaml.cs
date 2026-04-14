using System.Linq;

namespace INFT2051;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private async void Login_Clicked(object sender, EventArgs e)
    {
        string enteredPin = PinEntry?.Text ?? string.Empty;
        string savedPin = PinManager.GetPin();

        if (enteredPin == savedPin)
        {
            await DisplayAlertAsync("Login", "Login Successful", "OK");
            await Shell.Current.GoToAsync(nameof(HomePage));
        }
        else
        {
            await DisplayAlertAsync("Error", "Wrong PIN", "OK");
        }
    }

    private async void ForgotPin_Clicked(object sender, EventArgs e)
    {
        string answer = await DisplayPromptAsync(
            "Security Question",
            "What is your favourite colour?",
            "Next",
            "Cancel",
            "Enter your answer");

        if (answer == null)
            return;

        if (!PinManager.VerifySecurityAnswer(answer))
        {
            await DisplayAlertAsync("Error", "Wrong answer to the security question.", "OK");
            return;
        }

        string newPin = await DisplayPromptAsync(
            "Reset PIN",
            "Enter your new 4-digit PIN",
            "Next",
            "Cancel",
            "New PIN",
            maxLength: 4,
            keyboard: Keyboard.Numeric);

        if (newPin == null)
            return;

        if (string.IsNullOrWhiteSpace(newPin) || newPin.Length != 4 || !newPin.All(char.IsDigit))
        {
            await DisplayAlertAsync("Error", "PIN must be exactly 4 digits.", "OK");
            return;
        }

        string confirmPin = await DisplayPromptAsync(
            "Confirm PIN",
            "Re-enter your new PIN",
            "Reset",
            "Cancel",
            "Confirm PIN",
            maxLength: 4,
            keyboard: Keyboard.Numeric);

        if (confirmPin == null)
            return;

        if (newPin != confirmPin)
        {
            await DisplayAlertAsync("Error", "The two PIN entries do not match.", "OK");
            return;
        }

        PinManager.SetPin(newPin);

        if (PinEntry != null)
            PinEntry.Text = string.Empty;

        await DisplayAlertAsync("Success", "PIN reset successfully.", "OK");
    }
}