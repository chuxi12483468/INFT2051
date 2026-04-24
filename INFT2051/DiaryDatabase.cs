using SQLite;

namespace INFT2051;

public class DiaryDatabase
{
    private SQLiteAsyncConnection? _database; // SQLite database connection

    private async Task Init()
    {
        // If the database has already been initialized, do not initialize it again
        if (_database != null)
            return;

        // Create the database file path in the app's local storage
        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "diary.db3");

        // Create a new SQLite asynchronous connection
        _database = new SQLiteAsyncConnection(dbPath);

        // Create the diary table if it does not already exist
        await _database.CreateTableAsync<DiaryEntry>();
    }

    public async Task<List<DiaryEntry>> GetAllEntriesAsync()
    {
        await Init();

        // Get all diary entries and order them by ID, newest first
        return await _database!.Table<DiaryEntry>()
            .OrderByDescending(d => d.Id)
            .ToListAsync();
    }

    public async Task<List<DiaryEntry>> GetRecentEntriesAsync(int count = 3)
    {
        await Init();

        // Get the latest diary entries based on the auto-increment ID
        return await _database!.Table<DiaryEntry>()
            .OrderByDescending(d => d.Id)
            .Take(count)
            .ToListAsync();
    }

    public async Task<DiaryEntry?> GetEntryAsync(int id)
    {
        await Init();

        // Find and return one diary entry by its ID
        return await _database!.Table<DiaryEntry>()
            .Where(d => d.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<int> SaveEntryAsync(DiaryEntry entry)
    {
        await Init();

        // If the diary already has an ID, update the existing record
        if (entry.Id != 0)
            return await _database!.UpdateAsync(entry);

        // If the diary has no ID, insert it as a new record
        return await _database!.InsertAsync(entry);
    }

    public async Task<int> DeleteEntryAsync(DiaryEntry entry)
    {
        await Init();

        // Delete the selected diary entry from the database
        return await _database!.DeleteAsync(entry);
    }
}