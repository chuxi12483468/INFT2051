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
}