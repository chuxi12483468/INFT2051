using System.Linq;

namespace INFT2051;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();// Initialize UI components defined in XAML
    }

    private async void Login_Clicked(object sender, EventArgs e)
    {
        // Get the PIN entered by the user (default to empty if null)
        string enteredPin = PinEntry?.Text ?? string.Empty;

        // Retrieve the saved PIN from local storage
        string savedPin = PinManager.GetPin();

        // Check if the entered PIN matches the saved PIN
        if (enteredPin == savedPin)
        {
            // Show success message
            await DisplayAlertAsync("Login", "Login Successful", "OK");
            await Shell.Current.GoToAsync(nameof(HomePage));
        }
        else
        {
            await DisplayAlertAsync("Error", "Wrong PIN", "OK");
        }
    }

    private async void ForgotPin_Clicked(object sender, EventArgs e)  //ask about security question
    {
        // Prompt user to answer the security question
        string answer = await DisplayPromptAsync(
            "Security Question",
            "What is your favourite colour?",
            "Next",
            "Cancel",
            "Enter your answer");

        // Exit if user cancels the input
        if (answer == null)
            return;

        if (!PinManager.VerifySecurityAnswer(answer))
        {
            await DisplayAlertAsync("Error", "Wrong answer to the security question.", "OK");
            return;
        }
        // Prompt user to enter a new 4-digit PIN
        string newPin = await DisplayPromptAsync(
            "Reset PIN",
            "Enter your new 4-digit PIN",
            "Next",
            "Cancel",
            "New PIN",
            maxLength: 4,
            keyboard: Keyboard.Numeric);
        // Exit if user cancels
        if (newPin == null)
            return;
        // Validate PIN: must be exactly 4 numeric digits
        if (string.IsNullOrWhiteSpace(newPin) || newPin.Length != 4 || !newPin.All(char.IsDigit))
        {
            await DisplayAlertAsync("Error", "PIN must be exactly 4 digits.", "OK");
            return;
        }
        // Ask user to confirm the new PIN
        string confirmPin = await DisplayPromptAsync(
            "Confirm PIN",
            "Re-enter your new PIN",
            "Reset",
            "Cancel",
            "Confirm PIN",
            maxLength: 4,
            keyboard: Keyboard.Numeric);
        // Exit if user cancels
        if (confirmPin == null)
            return;
        // Check if both PIN entries match
        if (newPin != confirmPin)
        {
            await DisplayAlertAsync("Error", "The two PIN entries do not match.", "OK");
            return;
        }
        // Save the new PIN to local storage
        PinManager.SetPin(newPin);
        // Clear the PIN input field
        if (PinEntry != null)
            PinEntry.Text = string.Empty;

        await DisplayAlertAsync("Success", "PIN reset successfully.", "OK");
    }
}