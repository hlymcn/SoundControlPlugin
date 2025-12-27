using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.UserMessages;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.Localization;
using CounterStrikeSharp.API;
using CS2MenuManager.API.Menu;

namespace SoundControlPlugin;

[MinimumApiVersion(80)]
public class SoundControlPlugin : BasePlugin, IPluginConfig<Config>
{
    public override string ModuleName => "SoundControlPlugin";
    public override string ModuleVersion => "2.0.0";
    public override string ModuleAuthor => "hlymcn";
    public override string ModuleDescription => "Allows players to control background sound volume with the !dj command.";

    // 存储玩家音量设置 (0.0 - 1.0)
    private readonly Dictionary<ulong, float> _playerSoundVolume = new();
    
    // 存储玩家待应用的音量设置（下回合生效）
    private readonly Dictionary<ulong, float> _pendingVolumeChanges = new();

    // 音量选项值
    private static readonly float[] VolumeValues = [0.0f, 0.1f, 0.2f, 0.3f, 0.5f, 0.7f, 1.0f];

    public readonly IStringLocalizer<SoundControlPlugin> _localizer;
    public Config Config { get; set; } = new Config();

    public SoundControlPlugin(IStringLocalizer<SoundControlPlugin> localizer)
    {
        _localizer = localizer;
    }

    public override void Load(bool hotReload)
    {
        AddCommand("dj", "Open background sound volume menu.", OnDjCommand);
        HookUserMessage(208, Hook_SosStartSoundEvent);
        RegisterEventHandler<EventRoundStart>(OnRoundStart);
        RegisterEventHandler<EventRoundEnd>(OnRoundEnd);
    }

    private HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info)
    {
        ApplyPendingVolumeChanges();
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
        if (player == null || !player.IsValid) return;
        OpenVolumeMenu(player);
    }

    private void OpenVolumeMenu(CCSPlayerController player)
    {
        float currentVolume = _playerSoundVolume.GetValueOrDefault(player.SteamID, 1.0f);
        int currentPercent = (int)(currentVolume * 100);

        var menu = new WasdMenu(_localizer["Menu.Title"], this);

        foreach (float value in VolumeValues)
        {
            int percent = (int)(value * 100);
            bool isSelected = percent == currentPercent;
            string displayText = GetVolumeOptionText(percent, isSelected);

            menu.AddItem(displayText, (p, option) =>
            {
                SetPlayerVolume(p, value);
            });
        }

        menu.Display(player, 60);
    }

    private string GetVolumeOptionText(int percent, bool isSelected)
    {
        string text = percent switch
        {
            0 => _localizer["Menu.Option.Muted"],
            100 => _localizer["Menu.Option.Default"],
            _ => _localizer["Menu.Option.Percent", percent]
        };

        return isSelected ? $"{text} ?" : text;
    }

    public void SetPlayerVolume(CCSPlayerController player, float volume)
    {
        if (player == null || !player.IsValid) return;

        volume = Math.Clamp(volume, 0.0f, 1.0f);
        _pendingVolumeChanges[player.SteamID] = volume;
        
        int volumePercent = (int)(volume * 100);
        
        if (volume == 0.0f)
        {
            player.PrintToChat(_localizer["Chat.Volume.Muted"]);
        }
        else
        {
            player.PrintToChat(_localizer["Chat.Volume.Changed", volumePercent]);
        }
        player.PrintToChat(_localizer["Chat.Volume.NextRound"]);
    }

    public float GetPlayerVolume(ulong steamId)
    {
        return _playerSoundVolume.GetValueOrDefault(steamId, 1.0f);
    }

    private void ApplyPendingVolumeChanges()
    {
        foreach (var entry in _pendingVolumeChanges)
        {
            _playerSoundVolume[entry.Key] = entry.Value;
        }
        _pendingVolumeChanges.Clear();
    }

    private HookResult Hook_SosStartSoundEvent(UserMessage userMessage)
    {
        int sourceEntityIndex = userMessage.ReadInt("source_entity_index");
        var entity = Utilities.GetEntityFromIndex<CBaseEntity>(sourceEntityIndex);

        if (!IsBackgroundSoundEntity(entity, sourceEntityIndex))
        {
            return HookResult.Continue;
        }

        uint soundeventGuid = userMessage.ReadUInt("soundevent_guid");

        foreach (var player in Utilities.GetPlayers())
        {
            if (!player.IsValid) continue;

            float volume = _playerSoundVolume.GetValueOrDefault(player.SteamID, 1.0f);

            if (volume < 1.0f)
            {
                Server.NextFrame(() =>
                {
                    if (player.IsValid)
                    {
                        SendVolumeChange(player, soundeventGuid, volume);
                    }
                });
            }
        }

        return HookResult.Continue;
    }

    private static bool IsBackgroundSoundEntity(CBaseEntity? entity, int sourceEntityIndex)
    {
        if (entity == null) return false;
        if (sourceEntityIndex == 0) return false;

        return entity.DesignerName switch
        {
            "point_soundevent" => true,
            "ambient_generic" => true,
            "snd_event_point" => true,
            _ => false
        };
    }

    private void SendVolumeChange(CCSPlayerController player, uint soundeventGuid, float volume)
    {
        try
        {
            var volumeMessage = UserMessage.FromId(210);
            volumeMessage.SetUInt("soundevent_guid", soundeventGuid);
            volumeMessage.SetBytes("packed_params", GetSoundVolumeParams(volume));

            RecipientFilter filter = new RecipientFilter();
            filter.Add(player);
            volumeMessage.Recipients = filter;
            volumeMessage.Send();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SoundControlPlugin] Error sending volume change: {ex.Message}");
        }
    }

    private static byte[] GetSoundVolumeParams(float volume)
    {
        byte[] header = [0xE9, 0x54, 0x60, 0xBD, 0x08, 0x04, 0x00];
        byte[] volumeBytes = BitConverter.GetBytes(volume);
        
        byte[] result = new byte[header.Length + volumeBytes.Length];
        header.CopyTo(result, 0);
        volumeBytes.CopyTo(result, header.Length);
        
        return result;
    }

    private void LoadPlayerSettingsFromDatabase()
    {
        Database.LoadPlayerSettingsFromDatabaseAsync(_playerSoundVolume).GetAwaiter().GetResult();
    }

    private void SavePlayerSettingsToDatabase()
    {
        foreach (var entry in _playerSoundVolume)
        {
            Task.Run(() => Database.SavePlayerSettingsToDatabaseAsync(entry.Key, entry.Value));
        }
    }
}
