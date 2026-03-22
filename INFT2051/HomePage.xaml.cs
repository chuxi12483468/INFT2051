using System.Linq;

namespace INFT2051;

public partial class HomePage : ContentPage
{
    private readonly DiaryDatabase _diaryDatabase;

    public class DiaryPreviewItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string Preview { get; set; } = string.Empty;
    }

    public HomePage(DiaryDatabase diaryDatabase)
    {
        InitializeComponent();
        _diaryDatabase = diaryDatabase;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadRecentDiariesAsync();
    }

    private async Task LoadRecentDiariesAsync()
    {
        var diaryEntries = await _diaryDatabase.GetRecentEntriesAsync(3);

        var recentItems = diaryEntries
            .Select(d => new DiaryPreviewItem
            {
                Id = d.Id,
                Title = d.Title,
                Date = d.Date.ToString("dd/MM/yyyy"),
                Preview = d.Content
            })
            .ToList();

        RecentDiaryCollection.ItemsSource = recentItems;
    }

    private async void OnCreateDiaryClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(DiaryEditPage));
    }

    private async void OnSettingsClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(SettingPage));
    }

    private async void OnEditClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter != null)
        {
            int id = Convert.ToInt32(button.CommandParameter);
            await Shell.Current.GoToAsync($"{nameof(DiaryEditPage)}?id={id}");
        }
    }
}