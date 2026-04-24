namespace INFT2051;

// Allow the page to receive a diary ID through navigation (e.g., ?id=1)
[QueryProperty(nameof(DiaryId), "id")]
public partial class DiaryEditPage : ContentPage
{
    private readonly DiaryDatabase _diaryDatabase; // Database service
    private DiaryEntry? _currentDiary; // Current diary entry being edited

    private readonly DrawingCanvas _drawingCanvas = new(); // Drawing canvas for sketching
    private bool _isDrawing; // Flag to indicate whether user is currently drawing

    private int _diaryId;

    // Property to receive diary ID from navigation
    public int DiaryId
    {
        get => _diaryId;
        set
        {
            _diaryId = value;

            // Ensure UI updates run on the main thread
            MainThread.BeginInvokeOnMainThread(async () => await LoadDiaryAsync());
        }
    }

    // Constructor
    public DiaryEditPage(DiaryDatabase diaryDatabase)
    {
        InitializeComponent(); // Initialize UI components

        _diaryDatabase = diaryDatabase;

        // Set default date to current date
        if (DiaryDatePicker != null)
            DiaryDatePicker.Date = DateTime.Now;

        // Assign drawing canvas to GraphicsView
        if (DiaryGraphicsView != null)
            DiaryGraphicsView.Drawable = _drawingCanvas;
    }

    // Load diary entry from database
    private async Task LoadDiaryAsync()
    {
        // Do nothing if ID is invalid
        if (_diaryId <= 0)
            return;

        // Retrieve diary entry by ID
        _currentDiary = await _diaryDatabase.GetEntryAsync(_diaryId);

        // Populate UI if diary exists
        if (_currentDiary != null)
        {
            if (TitleEntry != null)
                TitleEntry.Text = _currentDiary.Title;

            if (ContentEditor != null)
                ContentEditor.Text = _currentDiary.Content;

            if (DiaryDatePicker != null)
                DiaryDatePicker.Date = _currentDiary.Date;

            // Load drawing data into canvas
            _drawingCanvas.Deserialize(_currentDiary.DrawingData);
            DiaryGraphicsView?.Invalidate(); // Refresh canvas
        }
    }

    // Start drawing when touch begins
    private void OnDrawingStart(object? sender, TouchEventArgs e)
    {
        if (e.Touches.Length == 0)
            return;

        var point = e.Touches[0];

        _drawingCanvas.StartStroke((float)point.X, (float)point.Y); // Start new stroke
        _isDrawing = true;

        DiaryGraphicsView?.Invalidate(); // Refresh canvas
    }

    // Continue drawing when finger moves
    private void OnDrawingDrag(object? sender, TouchEventArgs e)
    {
        if (!_isDrawing || e.Touches.Length == 0)
            return;

        var point = e.Touches[0];

        _drawingCanvas.AddPoint((float)point.X, (float)point.Y); // Add point to stroke
        DiaryGraphicsView?.Invalidate(); // Refresh canvas
    }

    // Stop drawing when touch ends
    private void OnDrawingEnd(object? sender, TouchEventArgs e)
    {
        _isDrawing = false;
    }

    // Set drawing color to black
    private void OnBlackColorClicked(object sender, EventArgs e)
    {
        _drawingCanvas.IsEraserMode = false;
        _drawingCanvas.CurrentColor = Colors.Black;
    }

    // Set drawing color to red
    private void OnRedColorClicked(object sender, EventArgs e)
    {
        _drawingCanvas.IsEraserMode = false;
        _drawingCanvas.CurrentColor = Colors.Red;
    }

    // Set drawing color to blue
    private void OnBlueColorClicked(object sender, EventArgs e)
    {
        _drawingCanvas.IsEraserMode = false;
        _drawingCanvas.CurrentColor = Colors.Blue;
    }

    // Enable eraser mode
    private void OnEraserClicked(object sender, EventArgs e)
    {
        _drawingCanvas.IsEraserMode = true;
    }

    // Set thin brush size
    private void OnThinClicked(object sender, EventArgs e)
    {
        _drawingCanvas.CurrentStrokeSize = 2;
    }

    // Set medium brush size
    private void OnMediumClicked(object sender, EventArgs e)
    {
        _drawingCanvas.CurrentStrokeSize = 4;
    }

    // Set thick brush size
    private void OnThickClicked(object sender, EventArgs e)
    {
        _drawingCanvas.CurrentStrokeSize = 7;
    }

    // Clear the entire drawing canvas
    private void OnClearDrawingClicked(object sender, EventArgs e)
    {
        _drawingCanvas.Clear();
        DiaryGraphicsView?.Invalidate();
    }

    // Save diary entry (create or update)
    private async void OnSaveClicked(object sender, EventArgs e)
    {
        // Retrieve user input
        string title = TitleEntry?.Text ?? string.Empty;
        string content = ContentEditor?.Text ?? string.Empty;
        DateTime selectedDate = DiaryDatePicker?.Date ?? DateTime.Now;

        // Create new diary if not existing
        if (_currentDiary == null)
            _currentDiary = new DiaryEntry();

        // Assign values
        _currentDiary.Title = title;
        _currentDiary.Content = content;
        _currentDiary.Date = selectedDate;

        // Serialize drawing data into storable format
        _currentDiary.DrawingData = _drawingCanvas.Serialize();

        // Save to database
        await _diaryDatabase.SaveEntryAsync(_currentDiary);

        await DisplayAlertAsync("Saved", "Diary saved successfully.", "OK");

        // Navigate back
        await Shell.Current.GoToAsync("..");
    }

    // Delete diary entry
    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        // Ask for confirmation
        bool confirm = await DisplayAlertAsync("Delete",
            "Are you sure you want to delete this diary?",
            "Yes",
            "No");

        if (!confirm)
            return;

        // If diary exists, delete from database
        if (_currentDiary != null)
        {
            await _diaryDatabase.DeleteEntryAsync(_currentDiary);
            await DisplayAlertAsync("Deleted", "Diary deleted successfully.", "OK");

            await Shell.Current.GoToAsync("..");
            return;
        }

        // Otherwise, clear UI fields
        if (TitleEntry != null)
            TitleEntry.Text = string.Empty;

        if (ContentEditor != null)
            ContentEditor.Text = string.Empty;

        if (DiaryDatePicker != null)
            DiaryDatePicker.Date = DateTime.Now;

        _drawingCanvas.Clear();
        DiaryGraphicsView?.Invalidate();

        await DisplayAlertAsync("Deleted", "Diary content cleared.", "OK");
    }

    // Navigate back to previous page
    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}