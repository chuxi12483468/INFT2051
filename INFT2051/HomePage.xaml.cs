using System.Linq;

namespace INFT2051;

public partial class HomePage : ContentPage
{
    private readonly DiaryDatabase _diaryDatabase; // Database service for retrieving diary data

    // Model used to display diary preview in the UI
    public class DiaryPreviewItem
    {
        public int Id { get; set; } // Diary ID
        public string Title { get; set; } = string.Empty; // Diary title
        public string Date { get; set; } = string.Empty; // Formatted date
        public string Preview { get; set; } = string.Empty; // Short preview of content
    }

    // Constructor with dependency injection
    public HomePage(DiaryDatabase diaryDatabase)
    {
        InitializeComponent(); // Initialize UI components
        _diaryDatabase = diaryDatabase;
    }

    // Triggered when the page becomes visible
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Load recent diary entries when page appears
        await LoadRecentDiariesAsync();
    }

    // Load recent diary entries from database
    private async Task LoadRecentDiariesAsync()
    {
        // Get the latest 3 diary entries
        var diaryEntries = await _diaryDatabase.GetRecentEntriesAsync(3);

        // Convert database model into UI-friendly preview model
        var recentItems = diaryEntries
            .Select(d => new DiaryPreviewItem
            {
                Id = d.Id,
                Title = d.Title,
                Date = d.Date.ToString("dd/MM/yyyy"), // Format date for display
                Preview = d.Content // Display content preview
            })
            .ToList();

        // Bind data to the collection view
        RecentDiaryCollection.ItemsSource = recentItems;
    }

    // Navigate to create a new diary entry
    private async void OnCreateDiaryClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(DiaryEditPage));
    }

    // Navigate to settings page
    private async void OnSettingsClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(SettingPage));
    }

    // Navigate to edit a selected diary entry
    private async void OnEditClicked(object sender, EventArgs e)
    {
        // Retrieve diary ID from button parameter
        if (sender is Button button && button.CommandParameter != null)
        {
            int id = Convert.ToInt32(button.CommandParameter);

            // Pass ID via navigation query parameter
            await Shell.Current.GoToAsync($"{nameof(DiaryEditPage)}?id={id}");
        }
    }
}