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
        if (_connectionString == null)
        {
            throw new InvalidOperationException("Database connection is not initialized.");
        }

        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();
        
        // 检查表结构是否需要迁移（旧表有 IsSoundBlocked 列）
        var checkColumnCommand = connection.CreateCommand();
        checkColumnCommand.CommandText = @"
            SELECT COUNT(*) 
            FROM INFORMATION_SCHEMA.COLUMNS 
            WHERE TABLE_SCHEMA = DATABASE() 
            AND TABLE_NAME = 'DJ_Settings' 
            AND COLUMN_NAME = 'IsSoundBlocked';
        ";
        
        var hasOldColumn = Convert.ToInt32(await checkColumnCommand.ExecuteScalarAsync()) > 0;
        
        if (hasOldColumn)
        {
            // 删除旧表
            var dropTableCommand = connection.CreateCommand();
            dropTableCommand.CommandText = "DROP TABLE IF EXISTS DJ_Settings;";
            await dropTableCommand.ExecuteNonQueryAsync();
            Console.WriteLine("[SoundControlPlugin] Old table structure detected, migrating to new format...");
        }

        // 创建新表
        var createTableCommand = connection.CreateCommand();
        createTableCommand.CommandText = @"
            CREATE TABLE IF NOT EXISTS DJ_Settings (
                steamid64 BIGINT NOT NULL UNIQUE,
                SoundVolume FLOAT NOT NULL DEFAULT 1.0
            );
        ";
        await createTableCommand.ExecuteNonQueryAsync();
    }

    public static async Task LoadPlayerSettingsFromDatabaseAsync(Dictionary<ulong, float> playerSoundVolume)
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
            selectQuery.CommandText = "SELECT steamid64, SoundVolume FROM DJ_Settings;";
            var reader = await selectQuery.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                ulong playerId = reader.GetUInt64("steamid64");
                float volume = reader.GetFloat("SoundVolume");
                playerSoundVolume[playerId] = volume;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SoundControlPlugin] Error loading player settings: {ex.Message}");
        }
    }

    public static async Task SavePlayerSettingsToDatabaseAsync(ulong playerId, float volume)
    {
        if (_connectionString == null)
        {
            throw new InvalidOperationException("Database connection is not initialized.");
        }

        try
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            var updateCommand = connection.CreateCommand();
            updateCommand.CommandText = @"
                INSERT INTO DJ_Settings (steamid64, SoundVolume) 
                VALUES (@steamid64, @SoundVolume) 
                ON DUPLICATE KEY UPDATE SoundVolume = VALUES(SoundVolume);
            ";
            updateCommand.Parameters.AddWithValue("@steamid64", playerId);
            updateCommand.Parameters.AddWithValue("@SoundVolume", volume);
            await updateCommand.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SoundControlPlugin] Error saving player settings: {ex.Message}");
        }
    }
}
