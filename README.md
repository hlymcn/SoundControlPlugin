# SoundControlPlugin

[![跳转至中文介绍](https://img.shields.io/badge/%E8%B7%B3%E8%BD%AC%E5%88%B0%E4%B8%AD%E6%96%87%E7%89%88-%E4%B8%AD%E6%96%87-red)](#中文版介绍)
[![Release](https://img.shields.io/github/v/release/DearCrazyLeaf/SoundControlPlugin?include_prereleases&color=blueviolet)](https://github.com/DearCrazyLeaf/SoundControlPlugin/releases/latest)
[![License](https://img.shields.io/badge/License-GPL%203.0-orange)](https://www.gnu.org/licenses/gpl-3.0.txt)
[![Issues](https://img.shields.io/github/issues/DearCrazyLeaf/SoundControlPlugin?color=darkgreen)](https://github.com/DearCrazyLeaf/SoundControlPlugin/issues)
[![Pull Requests](https://img.shields.io/github/issues-pr/DearCrazyLeaf/SoundControlPlugin?color=blue)](https://github.com/DearCrazyLeaf/SoundControlPlugin/pulls)
[![Downloads](https://img.shields.io/github/downloads/DearCrazyLeaf/SoundControlPlugin/total?color=brightgreen)](https://github.com/DearCrazyLeaf/SoundControlPlugin/releases)
[![GitHub Stars](https://img.shields.io/github/stars/DearCrazyLeaf/SoundControlPlugin?color=yellow)](https://github.com/DearCrazyLeaf/SoundControlPlugin/stargazers)

**A CounterStrikeSharp plugin for Counter-Strike 2 servers that lets every player curate map music and ambience through the in-game `!dj` console with persistent per-player preferences.**

> [!WARNING]
> SoundControlPlugin depends on the Source 2 `SosStartSoundEvent` (UM 208) and `SosSetSoundEventParams` (UM 210) message flow. Custom server builds that block or rewrite those messages will prevent the volume overrides from applying.

## Features

- **Interactive Volume Menu:** Players open `!dj` to choose from built-in loudness presets, all previewed in real time.
- **Per-Player Persistence:** A MySQL table stores each SteamID's preferred multiplier so the choice survives reconnects and map rotations.
- **Event-Safe Hooking:** The plugin watches `SosStartSoundEvent` and injects `SosSetSoundEventParams` instead of muting tracks outright.
- **Localized Copy:** Language packs in `lang/en.json` and `lang/zh-Hans.json` keep the menu bilingual by default.
- **Graceful Fallback:** When a map restarts its ambient loop, the saved volume is reapplied automatically.

## Requirements

- [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp)
- [MetaMod:Source (CS2 branch)](https://www.metamodsource.net/downloads.php?branch=dev)
- [CS2MenuManager](https://github.com/schwarper/CS2MenuManager)
- MySQL 5.7+

## Installation

1. Download the latest release from the [SoundControlPlugin releases](https://github.com/DearCrazyLeaf/SoundControlPlugin/releases).
2. Extract to `game/csgo/addons/counterstrikesharp/plugins/SoundControlPlugin`.
3. Copy `Config/SoundControlPlugin.json` to `addons/counterstrikesharp/configs/plugins/SoundControlPlugin/SoundControlPlugin.json`.
4. Restart your CS2 server or reload via `css_plugins unload SoundControlPlugin` then `css_plugins load SoundControlPlugin`.

> [!NOTE]
> On first launch the plugin validates the `DJ_Settings` table. If an older schema is detected it is rebuilt automatically, so always back up your database before upgrading production servers.

## Configuration

The configuration file only needs database credentials and lives at `addons/counterstrikesharp/configs/plugins/SoundControlPlugin/SoundControlPlugin.json`.

```json
{
  "Database": {
    "Host": "127.0.0.1",
    "Port": 3306,
    "User": "",
    "Password": "",
    "Name": ""
  }
}
```

| Field | Description |
|-------|-------------|
| `Host` | Database hostname or IP address. |
| `Port` | Listening port (defaults to `3306`). |
| `User` | Account with permission to create and update `DJ_Settings`. |
| `Password` | Password for the MySQL user. |
| `Name` | Target schema where the plugin table should be created. |

Restart the server after editing the file so CounterStrikeSharp reloads the config into the plugin.

## Persistence & Database

- Table name: `DJ_Settings`
- Columns: `steamid64 BIGINT UNIQUE`, `SoundVolume FLOAT DEFAULT 1.0`
- Migrations: if an old `IsSoundBlocked` column is detected, the table is dropped and recreated with the new structure.
- Saves occur asynchronously on player updates, disconnects, and round transitions to avoid blocking gameplay threads.

## Commands

- `!dj` — Opens the menu (aliases `dj` or `/dj` depending on your chat trigger configuration).

## Contributing

Issues, feature requests, and pull requests are welcome! Please discuss substantial changes via [issues](https://github.com/DearCrazyLeaf/SoundControlPlugin/issues) before submitting major PRs so we can keep configuration compatibility intact.

## License

<a href="https://www.gnu.org/licenses/gpl-3.0.txt" target="_blank" style="margin-left: 10px; text-decoration: none;">
    <img src="https://img.shields.io/badge/License-GPL%203.0-orange?style=for-the-badge&logo=gnu" alt="GPL v3 License">
</a>

---

# 中文版介绍

[![Back to English](https://img.shields.io/badge/Back_to_English-English-blue)](#soundcontrolplugin)
[![Release](https://img.shields.io/github/v/release/DearCrazyLeaf/SoundControlPlugin?include_prereleases&color=blueviolet&label=%E6%9C%80%E6%96%B0%E7%89%88)](https://github.com/DearCrazyLeaf/SoundControlPlugin/releases/latest)
[![License](https://img.shields.io/badge/%E8%AE%B8%E5%8F%AF-GPL%203.0-orange)](https://www.gnu.org/licenses/gpl-3.0.txt)
[![Issues](https://img.shields.io/github/issues/DearCrazyLeaf/SoundControlPlugin?color=darkgreen&label=%E5%8F%8D%E9%A6%88)](https://github.com/DearCrazyLeaf/SoundControlPlugin/issues)
[![Downloads](https://img.shields.io/github/downloads/DearCrazyLeaf/SoundControlPlugin/total?color=brightgreen&label=%E4%B8%8B%E8%BD%BD)](https://github.com/DearCrazyLeaf/SoundControlPlugin/releases)

**一个用于 Counter-Strike 2 服务器的背景音乐音量控制插件，玩家可通过 `!dj` 菜单即时调节地图原声，设置会自动保存并在所有服务器上同步。**

> [!WARNING]
> 插件依赖 `SosStartSoundEvent` / `SosSetSoundEventParams` 消息链路。如果您的服务端屏蔽了这些消息，音量覆盖将无法生效。

## 特性

- **交互式音量菜单：** 玩家输入 `!dj` 即可弹出预设音量选项，并实时生效。
- **玩家独立存档：** 使用 MySQL 表记录 SteamID 对应音量，重连或换图都能保持。 
- **安全事件注入：** 捕获背景音启动事件后，仅修改音量参数，不破坏原始音轨。 
- **多语言支持：** `lang/en.json`、`lang/zh-Hans.json` 已内置，可自行扩展更多语言。 
- **自动回写：** 地图或回合重新播放 BGM 时，插件会自动重新设置对应音量。

## 要求

- [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp)
- [MetaMod:Source (CS2)](https://www.metamodsource.net/downloads.php?branch=dev)
- [CS2MenuManager](https://github.com/schwarper/CS2MenuManager)
- MySQL 5.7+

## 安装

1. 在 [Release 页面](https://github.com/DearCrazyLeaf/SoundControlPlugin/releases) 下载最新版本。
2. 解压到 `game/csgo/addons/counterstrikesharp/plugins/SoundControlPlugin`。
3. 将 `Config/SoundControlPlugin.json` 复制到 `addons/counterstrikesharp/configs/plugins/SoundControlPlugin/`。
4. 重启服务器或通过 `css_plugins reload SoundControlPlugin` 热重载。

> [!NOTE]
> 首次运行会检测 `DJ_Settings` 表结构。如发现旧版字段会自动删除并重建，请在生产环境升级前做好备份。

## 配置

本插件的配置仅包含数据库信息，示例：

```json
{
  "Database": {
    "Host": "127.0.0.1",
    "Port": 3306,
    "User": "",
    "Password": "",
    "Name": ""
  }
}
```

| 字段 | 说明 |
|------|------|
| `Host` | 数据库主机或 IP。 |
| `Port` | 端口，默认 3306。 |
| `User` | 拥有创建/写入 `DJ_Settings` 权限的账号。 |
| `Password` | 上述账号的密码。 |
| `Name` | 插件创建数据表的目标库名。 |

修改完成后请重启服务器以重新加载配置。

## 数据与持久化

- 表名：`DJ_Settings`
- 字段：`steamid64 BIGINT UNIQUE`、`SoundVolume FLOAT DEFAULT 1.0`
- 迁移逻辑：检测到旧的 `IsSoundBlocked` 列会自动丢弃旧表并创建新结构。
- 保存策略：在玩家调节、断线和回合切换时异步写入，避免阻塞游戏线程。

## 命令

- `!dj` — 打开菜单（根据聊天前缀也可输入 `/dj` 或 `dj`）。

## 贡献

欢迎提交 Issue、Pull Request 或通过 [讨论区](https://github.com/DearCrazyLeaf/SoundControlPlugin/issues) 反馈需求。大改动请先沟通以避免配置破坏性变化。

## 许可证

<a href="https://www.gnu.org/licenses/gpl-3.0.txt" target="_blank" style="margin-left: 10px; text-decoration: none;">
    <img src="https://img.shields.io/badge/%E8%AE%B8%E5%8F%AF-GPL%203.0-orange?style=for-the-badge&logo=gnu" alt="GPL v3 License">
</a>
