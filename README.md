# SoundControlPlugin for Counter-Strike 2

## Overview

**SoundControlPlugin** is an innovative plugin for Counter-Strike 2 servers, offering players the ability to control background sound playback with the `!dj` command. This plugin enriches the gaming experience by allowing players to customize their audio environment, leading to a more personalized and enjoyable gameplay.

## Key Features

- **Dynamic Sound Control:** Players can activate or deactivate background sounds, providing an adaptable audio experience.
- **Individual Preferences:** Each player's sound settings are stored independently, ensuring a tailored experience without affecting others.
- **Event-Driven Filtering:** The plugin identifies specific sound events and applies player preferences to decide which sounds are broadcasted.

## Usage

To utilize the SoundControlPlugin, follow these steps:

1. **Installation:** Acquire the plugin from the [GitHub repository](https://github.com/hlymcn/SoundControlPlugin) and place it in your server's plugin directory.
2. **Configuration:** Customize the plugin by setting up the command and user message hooks as detailed in the plugin's documentation.
3. **Activation:** Upon activation, the plugin will enable players to use the `!dj` command to manage their sound settings.

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

This project is licensed under the MIT License - see the LICENSE file for details.

## Conclusion

The SoundControlPlugin is a valuable addition to Counter-Strike 2 servers, allowing for a more immersive and customizable audio experience. Its straightforward setup and user-friendly commands make it an excellent choice for server administrators seeking to enhance their players' engagement and satisfaction.
