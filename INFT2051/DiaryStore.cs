namespace INFT2051;

public class DiaryItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime Date { get; set; }
}

public static class DiaryStore
{
    public static List<DiaryItem> Diaries { get; } = new List<DiaryItem>();
    private static int _nextId = 1;
    public static int GetNextId()
    {
        return _nextId++;
    }
}