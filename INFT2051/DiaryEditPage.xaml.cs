namespace INFT2051;

[QueryProperty(nameof(DiaryId), "id")]
public partial class DiaryEditPage : ContentPage
{
    private readonly DiaryDatabase _diaryDatabase;
    private DiaryEntry? _currentDiary;

    private int _diaryId;
    public int DiaryId
    {
        get => _diaryId;
        set
        {
            _diaryId = value;
            MainThread.BeginInvokeOnMainThread(async () => await LoadDiaryAsync());
        }
    }

    public DiaryEditPage(DiaryDatabase diaryDatabase)
    {
        InitializeComponent();
        _diaryDatabase = diaryDatabase;

        if (DiaryDatePicker != null)
        {
            DiaryDatePicker.Date = DateTime.Now;
        }
    }

    private async Task LoadDiaryAsync()
    {
        if (_diaryId <= 0)
            return;

        _currentDiary = await _diaryDatabase.GetEntryAsync(_diaryId);

        if (_currentDiary != null)
        {
            if (TitleEntry != null)
                TitleEntry.Text = _currentDiary.Title;

            if (ContentEditor != null)
                ContentEditor.Text = _currentDiary.Content;

            if (DiaryDatePicker != null)
                DiaryDatePicker.Date = _currentDiary.Date;
        }
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        string title = TitleEntry?.Text ?? string.Empty;
        string content = ContentEditor?.Text ?? string.Empty;
        DateTime selectedDate = DiaryDatePicker?.Date ?? DateTime.Now;

        if (_currentDiary == null)
        {
            _currentDiary = new DiaryEntry();
        }

        _currentDiary.Title = title;
        _currentDiary.Content = content;
        _currentDiary.Date = selectedDate;

        await _diaryDatabase.SaveEntryAsync(_currentDiary);

        await DisplayAlertAsync("Saved", "Diary saved successfully.", "OK");
        await Shell.Current.GoToAsync("..");
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlertAsync("Delete",
            "Are you sure you want to delete this diary?",
            "Yes",
            "No");

        if (!confirm)
            return;

        if (_currentDiary != null)
        {
            await _diaryDatabase.DeleteEntryAsync(_currentDiary);
            await DisplayAlertAsync("Deleted", "Diary deleted successfully.", "OK");
            await Shell.Current.GoToAsync("..");
            return;
        }

        if (TitleEntry != null)
            TitleEntry.Text = string.Empty;

        if (ContentEditor != null)
            ContentEditor.Text = string.Empty;

        if (DiaryDatePicker != null)
            DiaryDatePicker.Date = DateTime.Now;

        await DisplayAlertAsync("Deleted", "Diary content cleared.", "OK");
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}