using CounterStrikeSharp.API.Core;
using System.Text.Json.Serialization;

namespace SoundControlPlugin;

public class Config : BasePluginConfig
{
    [JsonPropertyName("Database")]
    public DatabaseConfig DatabaseSettings { get; set; } = new DatabaseConfig();

    public class DatabaseConfig
    {
        [JsonPropertyName("Host")]
        public string Host { get; set; } = string.Empty;

        [JsonPropertyName("Port")]
        public uint Port { get; set; } = 3306;

        [JsonPropertyName("User")]
        public string User { get; set; } = string.Empty;

        [JsonPropertyName("Password")]
        public string Password { get; set; } = string.Empty;

        [JsonPropertyName("Name")]
        public string Name { get; set; } = string.Empty;
    }
}