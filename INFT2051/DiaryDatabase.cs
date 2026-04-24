using SQLite;

namespace INFT2051;

public class DiaryDatabase
{
    private SQLiteAsyncConnection? _database;
    //Initialize SQLite database connection
    private async Task Init()
    {
        if (_database != null)
            return;

        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "diary.db3");
        _database = new SQLiteAsyncConnection(dbPath);

        await _database.CreateTableAsync<DiaryEntry>();
    }
    // Retrieve all diary entries from the database
    public async Task<List<DiaryEntry>> GetAllEntriesAsync()
    {
        await Init();
        return await _database!.Table<DiaryEntry>()
            .OrderByDescending(d => d.Date)
            .ToListAsync();
    }
    // Retrieve a limited number of recent diary entries
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
    //save diary entry to database
    public async Task<int> SaveEntryAsync(DiaryEntry entry)
    {
        await Init();

        if (entry.Id != 0)
            return await _database!.UpdateAsync(entry);

        return await _database!.InsertAsync(entry);
    }
    //delect diary entry from SQLite database
    public async Task<int> DeleteEntryAsync(DiaryEntry entry)
    {
        await Init();
        return await _database!.DeleteAsync(entry);
    }
}