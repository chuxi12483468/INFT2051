using System.Text;

namespace INFT2051;

public partial class SettingPage : ContentPage
{
    private readonly DiaryDatabase _diaryDatabase;
    private bool _isInitializingReminderPickers;

    public SettingPage(DiaryDatabase diaryDatabase)
    {
        InitializeComponent();
        _diaryDatabase = diaryDatabase;

        InitialiseReminderPickers();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (ReminderSwitch != null)
        {
            ReminderSwitch.Toggled -= OnReminderToggled;
            ReminderSwitch.IsToggled = ReminderSettings.IsReminderEnabled();
            ReminderSwitch.Toggled += OnReminderToggled;
        }

        LoadReminderTimeToPickers();
    }

    private void InitialiseReminderPickers()
    {
        if (HourPicker != null && HourPicker.Items.Count == 0)
        {
            for (int i = 1; i <= 12; i++)
            {
                HourPicker.Items.Add(i.ToString("D2"));
            }
        }

        if (MinutePicker != null && MinutePicker.Items.Count == 0)
        {
            for (int i = 0; i < 60; i++)
            {
                MinutePicker.Items.Add(i.ToString("D2"));
            }
        }
    }

    private void LoadReminderTimeToPickers()
    {
        if (AmPmPicker == null || HourPicker == null || MinutePicker == null)
            return;

        _isInitializingReminderPickers = true;

        var savedTime = ReminderSettings.GetReminderTime();
        int hour24 = savedTime.Hours;
        int minute = savedTime.Minutes;

        string ampm = hour24 >= 12 ? "PM" : "AM";
        int hour12 = hour24 % 12;
        if (hour12 == 0)
            hour12 = 12;

        AmPmPicker.SelectedIndex = ampm == "AM" ? 0 : 1;
        HourPicker.SelectedItem = hour12.ToString("D2");
        MinutePicker.SelectedItem = minute.ToString("D2");

        _isInitializingReminderPickers = false;
    }

    private TimeSpan GetSelectedReminderTime()
    {
        if (AmPmPicker == null || HourPicker == null || MinutePicker == null)
            return new TimeSpan(20, 0, 0);

        string ampm = AmPmPicker.SelectedItem?.ToString() ?? "PM";
        int hour12 = int.TryParse(HourPicker.SelectedItem?.ToString(), out int h) ? h : 8;
        int minute = int.TryParse(MinutePicker.SelectedItem?.ToString(), out int m) ? m : 0;

        int hour24;
        if (ampm == "AM")
        {
            hour24 = hour12 == 12 ? 0 : hour12;
        }
        else
        {
            hour24 = hour12 == 12 ? 12 : hour12 + 12;
        }

        return new TimeSpan(hour24, minute, 0);
    }

    private void OnReminderPickerChanged(object? sender, EventArgs e)
    {
        if (_isInitializingReminderPickers)
            return;

        var selectedTime = GetSelectedReminderTime();
        ReminderSettings.SetReminderTime(selectedTime);

        if (ReminderSettings.IsReminderEnabled())
        {
            ReminderService.ScheduleDailyReminder(selectedTime);
        }
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

    private async void OnReminderToggled(object? sender, ToggledEventArgs e)
    {
        ReminderSettings.SetReminderEnabled(e.Value);

        if (e.Value)
        {
            VibrationService.VibrateShort();
            ReminderService.ScheduleDailyReminder(GetSelectedReminderTime());
            await DisplayAlertAsync("Reminder", "Daily reminder has been enabled.", "OK");
        }
        else
        {
            VibrationService.Cancel();
            ReminderService.CancelDailyReminder();
            await DisplayAlertAsync("Reminder", "Daily reminder has been disabled.", "OK");
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