# Banned.Qbittorrent

[中文文档](https://github.com/banned2054/Banned.Qbittorrent/blob/main/Docs/README.md)

[![License](https://img.shields.io/badge/license-Apache_2.0-green)](./LICENSE) [![NuGet](https://img.shields.io/nuget/v/Banned.Qbittorrent.svg)](https://www.nuget.org/packages/Banned.Qbittorrent)

**Banned.Qbittorrent** is a high-performance, strongly-typed .NET client library for qBittorrent's Web API. It is designed to be modern, asynchronous, and robust, providing full coverage of the qBittorrent feature set.

## ✨ Key Features

* **Full API Implementation**: Complete coverage of Torrent management, Application settings, RSS, Search, and Sync modules.
* **Automatic Version Negotiation**: Automatically detects the Web API version and enforces compatibility checks for specific features.
* **Smart Auth Handling**: Built-in session keep-alive and automatic re-login logic.
* **Resilient Networking**: Includes an exponential backoff retry mechanism to handle transient network issues.
* **Asynchronous First**: First-class support for `Task`-based asynchronous patterns and `CancellationToken`.

## 📦 Installation

Install via NuGet Package Manager:

```bash
dotnet add package Banned.Qbittorrent
```

## 🚀 Quick Start

1. Initialize the Client
    Use the static Create factory method to handle initial authentication and API version negotiation automatically.

```csharp
using Banned.Qbittorrent;

// Automatically logs in and configures version compatibility
var client = await QBittorrentClient.Create("http://localhost:8080", "admin", "adminadmin");
```
2. Torrent Management
```csharp
// Get all torrents
var torrents = await client.Torrent.GetInfos();

// Add a new torrent via Magnet link
await client.Torrent.Add(new AddTorrentsRequest {
    Urls = new[] { "magnet:?xt=urn:btih:..." },
    SavePath = "/downloads/movies"
});

// Manage specific torrents
var hashes = new[] { "hash1", "hash2" };
await client.Torrent.Pause(hashes);
await client.Torrent.Resume(hashes);
```
3. Application Preferences
```csharp
// Retrieve current settings
var prefs = await client.Application.GetApplicationPreferences();

// Modify and save
prefs.AlternativeWebuiEnabled = true;
await client.Application.SetApplicationPreferences(prefs);
```

## 🛠 Project Architecture

|Service|Description|
|----------|-----------|
|Application|"App versions, Build info, Preferences, and Cookie management."|
|Authentication|"Login, Logout, and Session persistence."|
|Torrent|"Management of torrents, categories, tags, and file priority."|
|Transfer|"Global speed limits, transfer statistics, and info."|
|Sync|Main data synchronization and incremental status updates.|
|Rss|Management of RSS feeds and automated download rules.|
|Search|Search engine tasks and plugin management.|
|Log|System events and peer-to-peer connection logs.|

## 📜 Changelog

[🧾 View CHANGELOG](https://github.com/banned2054/Banned.Qbittorrent/blob/main/Docs/CHANGELOG.md)

## ⚖️ License

This project is licensed under the Apache License 2.0. See the [LICENSE](https://github.com/banned2054/Banned.Qbittorrent/blob/main/LICENSE.txt) file for details.

## 🤝 Contributing

Contributions are welcome! If you encounter a bug or have a feature request, please open an issue. Pull requests are highly appreciated.

---
Inspired by [qbittorrent-api](https://github.com/rmartin16/qbittorrent-api) and [official qBittorrent WebUI Wiki](https://github.com/qbittorrent/qBittorrent/wiki/WebUI-API-(qBittorrent-5.0)).
