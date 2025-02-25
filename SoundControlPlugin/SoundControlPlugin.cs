using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.UserMessages;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.Localization;
using CounterStrikeSharp.API;

namespace SoundControlPlugin;

[MinimumApiVersion(80)]
public class SoundControlPlugin : BasePlugin, IPluginConfig<Config>
{
    public override string ModuleName => "SoundControlPlugin";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "hlymcn";
    public override string ModuleDescription => "Allows players to control background sound playback with the !dj command.";

    private Dictionary<ulong, bool> playerSoundBlocked = new Dictionary<ulong, bool>();
    public readonly IStringLocalizer<SoundControlPlugin> _localizer;
    public Config Config { get; set; } = new Config();

    public SoundControlPlugin(IStringLocalizer<SoundControlPlugin> localizer)
    {
        _localizer = localizer;
    }

    public override void Load(bool hotReload)
    {
        AddCommand("dj", "Toggle background sound playback.", OnDjCommand);
        HookUserMessage(208, Hook_SosStartSoundEvent);
        RegisterEventHandler<EventRoundStart>(OnRoundStart);
        RegisterEventHandler<EventRoundEnd>(OnRoundEnd);
    }

    private HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info)
    {
        LoadPlayerSettingsFromDatabase();
        return HookResult.Continue;
    }

    private HookResult OnRoundEnd(EventRoundEnd @event, GameEventInfo info)
    {
        SavePlayerSettingsToDatabase();
        return HookResult.Continue;
    }

    public void OnConfigParsed(Config config)
    {
        Config = config;
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        Database.InitializeAsync(Config.DatabaseSettings).GetAwaiter().GetResult();
    }

    private void OnDjCommand(CCSPlayerController? player, CommandInfo info)
    {
        if (player == null) return;

        ulong playerId = player.SteamID;
        bool isBlocked = playerSoundBlocked.GetValueOrDefault(playerId, false);
        playerSoundBlocked[playerId] = !isBlocked;

        if (playerSoundBlocked[playerId])
        {
            player.PrintToChat(_localizer["Background.sound.been.blocked"]);
        }
        else
        {
            player.PrintToChat(_localizer["Background.sound.been.resumed"]);
        }
    }

    private HookResult Hook_SosStartSoundEvent(UserMessage userMessage)
    {
        int sourceEntityIndex = userMessage.ReadInt("source_entity_index");
        var entity = Utilities.GetEntityFromIndex<CBaseEntity>(sourceEntityIndex);
        if (entity != null && (sourceEntityIndex == 0 || entity.DesignerName == "point_soundevent" || entity.DesignerName == "logic_timer" || entity.DesignerName == "ambient_generic" || entity.DesignerName == "snd_event_point"))
        {
            RecipientFilter filter = new RecipientFilter();
            foreach (var player in Utilities.GetPlayers())
            {
                if (!playerSoundBlocked.ContainsKey(player.SteamID) || !playerSoundBlocked[player.SteamID])
                {
                    filter.Add(player);
                }
            }
            userMessage.Recipients = filter;
        }
        return HookResult.Continue;
    }

    private void LoadPlayerSettingsFromDatabase()
    {
        Database.LoadPlayerSettingsFromDatabaseAsync(playerSoundBlocked).GetAwaiter().GetResult();
    }

    private void SavePlayerSettingsToDatabase()
    {
        foreach (var entry in playerSoundBlocked)
        {
            Task.Run(() => Database.SavePlayerSettingsToDatabaseAsync(entry.Key, entry.Value));
        }
    }
}
