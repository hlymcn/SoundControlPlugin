using MySqlConnector;

namespace SoundControlPlugin;

public static class Database
{
    private static string? _connectionString;

    public static async Task InitializeAsync(Config.DatabaseConfig dbConfig)
    {
        _connectionString = $"server={dbConfig.Host};database={dbConfig.Name};userid={dbConfig.User};password={dbConfig.Password};port={dbConfig.Port};";
        await EnsureTableExistsAsync();
    }

    public static async Task EnsureTableExistsAsync()
    {
        if (_connectionString != null)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            var createTableQuery = connection.CreateCommand();
            createTableQuery.CommandText = @"
                CREATE TABLE IF NOT EXISTS DJ_Settings (
                    steamid64 BIGINT NOT NULL UNIQUE,
                    IsSoundBlocked BOOLEAN NOT NULL
                );
             ";
            await createTableQuery.ExecuteNonQueryAsync();
        }
        else
        {
            throw new InvalidOperationException("Database connection is not initialized.");
        }
    }

    public static async Task LoadPlayerSettingsFromDatabaseAsync(Dictionary<ulong, bool> playerSoundBlocked)
    {
        if (_connectionString == null)
        {
            throw new InvalidOperationException("Database connection is not initialized.");
        }

        try
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            var selectQuery = connection.CreateCommand();
            selectQuery.CommandText = "SELECT steamid64, IsSoundBlocked FROM DJ_Settings;";
            var reader = await selectQuery.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                ulong playerId = reader.GetUInt64("steamid64");
                bool isBlocked = reader.GetBoolean("IsSoundBlocked");
                playerSoundBlocked[playerId] = isBlocked;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading player settings: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }
    }


    public static async Task SavePlayerSettingsToDatabaseAsync(ulong playerId, bool isBlocked)
    {
        if (_connectionString != null)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            var updateCommand = connection.CreateCommand();
            updateCommand.CommandText = "INSERT INTO DJ_Settings (steamid64, IsSoundBlocked) VALUES (@steamid64, @IsSoundBlocked) ON DUPLICATE KEY UPDATE IsSoundBlocked = VALUES(IsSoundBlocked);";
            updateCommand.Parameters.AddWithValue("@steamid64", playerId);
            updateCommand.Parameters.AddWithValue("@IsSoundBlocked", isBlocked);
            await updateCommand.ExecuteNonQueryAsync();
        }
        else
        {
            throw new InvalidOperationException("Database connection is not initialized.");
        }
    }
}
