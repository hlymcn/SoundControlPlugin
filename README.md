# SoundControlPlugin for Counter-Strike 2

## Overview

**SoundControlPlugin** is an innovative CounterStrikeSharp plugin for Counter-Strike 2 servers. Version 2.0 introduces a full background-sound *volume* controller that players can access with the `!dj` command. Instead of simply blocking sounds, each player can now pick a preferred volume level (0%–100%), stored per SteamID in MySQL and enforced in real time through `SosSetSoundEventParams` messages.

## Key Features

- **Interactive Volume Menu:** Built with CS2MenuManager, the `!dj` menu lists predefined volume presets and highlights the current selection with localized text.
- **Per-Player Persistence:** Selected levels are saved immediately to MySQL, refreshed on reconnect, on round transitions, and when players disconnect.
- **Live Sound Param Control:** The plugin hooks `SosStartSoundEvent` (um 208) and pushes custom `SosSetSoundEventParams` (um 210) so every player hears background audio at their chosen volume.
- **Graceful Fallback:** If a map replays its background track next round, the stored volume automatically re-applies, guaranteeing consistent behavior.

## Usage

To utilize the SoundControlPlugin, follow these steps:

1. **Installation:** Acquire the plugin from the [GitHub repository](https://github.com/hlymcn/SoundControlPlugin) and place it in your server's plugin directory.
2. **Configuration:** Edit `.../configs/plugins/SoundControlPlugin/SoundControlPlugin.json` with your MySQL credentials; the plugin will migrate any legacy `IsSoundBlocked` tables to the new `SoundVolume` schema automatically.
3. **Activation:** Reload CounterStrikeSharp. Players can now open the volume menu via `!dj`, pick a preset, and hear the change immediately (or next round at worst).

## Requirements

- CounterStrikeSharp: [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp)
- MetaMod: [MetaMod](https://www.metamodsource.net/downloads.php?branch=dev) 

## Installation

1. Clone the repository or download the latest release from [GitHub](https://github.com/hlymcn/SoundControlPlugin).
2. Copy the plugin files to `....\addons\counterstrikesharp\plugins\SoundControlPlugin`, and edit your MySQL Database in `....\addons\counterstrikesharp\configs\plugins\SoundControlPlugin\SoundControlPlugin.json` 
3. Ensure your server meets the API version requirements and adjust settings as necessary.

## Contribution and Support

We encourage community contributions. 
For suggestions or to report issues, please submit a pull request or open an issue on the [GitHub repository](https://github.com/hlymcn/SoundControlPlugin). Your input is crucial for the ongoing development of the plugin.

## License

This project is licensed under the GPL3.0 License - see the LICENSE file for details.

## Conclusion

The SoundControlPlugin is a valuable addition to Counter-Strike 2 servers, allowing for a more immersive and customizable audio experience. Its straightforward setup and user-friendly commands make it an excellent choice for server administrators seeking to enhance their players' engagement and satisfaction.
