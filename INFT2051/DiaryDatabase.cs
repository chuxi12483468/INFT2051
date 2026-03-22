using SQLite;

namespace INFT2051;

public class DiaryDatabase
{
    private SQLiteAsyncConnection? _database;

    private async Task Init()
    {
        if (_database != null)
            return;

        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "diary.db3");
        _database = new SQLiteAsyncConnection(dbPath);

        await _database.CreateTableAsync<DiaryEntry>();
    }

    public async Task<List<DiaryEntry>> GetAllEntriesAsync()
    {
        await Init();
        return await _database!.Table<DiaryEntry>()
            .OrderByDescending(d => d.Date)
            .ToListAsync();
    }

    public async Task<List<DiaryEntry>> GetRecentEntriesAsync(int count = 3)
    {
        await Init();
        return await _database!.Table<DiaryEntry>()
            .OrderByDescending(d => d.Date)
            .Take(count)
            .ToListAsync();
    }

    public async Task<DiaryEntry?> GetEntryAsync(int id)
    {
        await Init();
        return await _database!.Table<DiaryEntry>()
            .Where(d => d.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<int> SaveEntryAsync(DiaryEntry entry)
    {
        await Init();

        if (entry.Id != 0)
            return await _database!.UpdateAsync(entry);

        return await _database!.InsertAsync(entry);
    }

    public async Task<int> DeleteEntryAsync(DiaryEntry entry)
    {
        await Init();
        return await _database!.DeleteAsync(entry);
    }
}