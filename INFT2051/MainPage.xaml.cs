namespace INFT2051;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private async void Login_Clicked(object sender, EventArgs e)
    {
        if (PinEntry.Text == "1234")
        {
            await DisplayAlert("Login", "Login Successful", "OK");
        }
        else
        {
            await DisplayAlert("Error", "Wrong PIN", "OK");
        }
    }
}
