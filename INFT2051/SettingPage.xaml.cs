using System.Text;

namespace INFT2051;

public partial class SettingPage : ContentPage
{
    private readonly DiaryDatabase _diaryDatabase;

    public SettingPage(DiaryDatabase diaryDatabase)
    {
        InitializeComponent();
        _diaryDatabase = diaryDatabase;
    }

    private async void OnUpdatePinClicked(object sender, EventArgs e)
    {
        string newPin = NewPinEntry?.Text ?? string.Empty;

        if (string.IsNullOrWhiteSpace(newPin) || newPin.Length != 4)
        {
            await DisplayAlertAsync("Error", "PIN must be exactly 4 digits.", "OK");
            return;
        }

        PinManager.SetPin(newPin);

        if (NewPinEntry != null)
            NewPinEntry.Text = string.Empty;

        await DisplayAlertAsync("Success", "PIN updated successfully.", "OK");
    }

    private async void OnReminderToggled(object sender, ToggledEventArgs e)
    {
        if (e.Value)
        {
            await DisplayAlertAsync("Reminder", "Daily reminder turned on.", "OK");
        }
        else
        {
            await DisplayAlertAsync("Reminder", "Daily reminder turned off.", "OK");
        }
    }

    private async void OnExportClicked(object sender, EventArgs e)
    {
        var diaries = await _diaryDatabase.GetAllEntriesAsync();

        if (diaries.Count == 0)
        {
            await DisplayAlertAsync("Export", "No diaries to export.", "OK");
            return;
        }

        var sb = new StringBuilder();

        foreach (var diary in diaries)
        {
            sb.AppendLine($"Title: {diary.Title}");
            sb.AppendLine($"Date: {diary.Date:dd/MM/yyyy}");
            sb.AppendLine("Content:");
            sb.AppendLine(diary.Content);
            sb.AppendLine(new string('-', 40));
        }

        string fileName = $"DiaryExport_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
        string filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);

        File.WriteAllText(filePath, sb.ToString());

        await Share.Default.RequestAsync(new ShareFileRequest
        {
            Title = "Export Diaries",
            File = new ShareFile(filePath)
        });
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}