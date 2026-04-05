namespace INFT2051;

[QueryProperty(nameof(DiaryId), "id")]
public partial class DiaryEditPage : ContentPage
{
    private readonly DiaryDatabase _diaryDatabase;
    private DiaryEntry? _currentDiary;

    private readonly DrawingCanvas _drawingCanvas = new();
    private bool _isDrawing;

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
            DiaryDatePicker.Date = DateTime.Now;

        if (DiaryGraphicsView != null)
            DiaryGraphicsView.Drawable = _drawingCanvas;
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

            _drawingCanvas.Deserialize(_currentDiary.DrawingData);
            DiaryGraphicsView?.Invalidate();
        }
    }

    private void OnDrawingStart(object? sender, TouchEventArgs e)
    {
        if (e.Touches.Length == 0)
            return;

        var point = e.Touches[0];
        _drawingCanvas.StartStroke((float)point.X, (float)point.Y);
        _isDrawing = true;
        DiaryGraphicsView?.Invalidate();
    }

    private void OnDrawingDrag(object? sender, TouchEventArgs e)
    {
        if (!_isDrawing || e.Touches.Length == 0)
            return;

        var point = e.Touches[0];
        _drawingCanvas.AddPoint((float)point.X, (float)point.Y);
        DiaryGraphicsView?.Invalidate();
    }

    private void OnDrawingEnd(object? sender, TouchEventArgs e)
    {
        _isDrawing = false;
    }

    private void OnBlackColorClicked(object sender, EventArgs e)
    {
        _drawingCanvas.IsEraserMode = false;
        _drawingCanvas.CurrentColor = Colors.Black;
    }

    private void OnRedColorClicked(object sender, EventArgs e)
    {
        _drawingCanvas.IsEraserMode = false;
        _drawingCanvas.CurrentColor = Colors.Red;
    }

    private void OnBlueColorClicked(object sender, EventArgs e)
    {
        _drawingCanvas.IsEraserMode = false;
        _drawingCanvas.CurrentColor = Colors.Blue;
    }

    private void OnEraserClicked(object sender, EventArgs e)
    {
        _drawingCanvas.IsEraserMode = true;
    }

    private void OnThinClicked(object sender, EventArgs e)
    {
        _drawingCanvas.CurrentStrokeSize = 2;
    }

    private void OnMediumClicked(object sender, EventArgs e)
    {
        _drawingCanvas.CurrentStrokeSize = 4;
    }

    private void OnThickClicked(object sender, EventArgs e)
    {
        _drawingCanvas.CurrentStrokeSize = 7;
    }

    private void OnClearDrawingClicked(object sender, EventArgs e)
    {
        _drawingCanvas.Clear();
        DiaryGraphicsView?.Invalidate();
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        string title = TitleEntry?.Text ?? string.Empty;
        string content = ContentEditor?.Text ?? string.Empty;
        DateTime selectedDate = DiaryDatePicker?.Date ?? DateTime.Now;

        if (_currentDiary == null)
            _currentDiary = new DiaryEntry();

        _currentDiary.Title = title;
        _currentDiary.Content = content;
        _currentDiary.Date = selectedDate;
        _currentDiary.DrawingData = _drawingCanvas.Serialize();

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

        _drawingCanvas.Clear();
        DiaryGraphicsView?.Invalidate();

        await DisplayAlertAsync("Deleted", "Diary content cleared.", "OK");
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}