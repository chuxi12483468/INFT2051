using SQLite;

namespace INFT2051;

public class DiaryEntry
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public DateTime Date { get; set; }
}