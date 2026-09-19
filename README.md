# Banned.Qbittorrent

English | [简体中文](https://github.com/banned2054/Banned.Qbittorrent/blob/main/Docs/README.md)

[![NuGet](https://img.shields.io/nuget/v/Banned.Qbittorrent.svg)](https://www.nuget.org/packages/Banned.Qbittorrent) [![Downloads](https://img.shields.io/nuget/dt/Banned.Qbittorrent.svg)](https://www.nuget.org/packages/Banned.Qbittorrent) [![License](https://img.shields.io/badge/license-Apache_2.0-green)](./LICENSE)

**Banned.Qbittorrent** is a strongly typed, asynchronous .NET client for qBittorrent's Web API. It covers the main application, torrent, transfer, sync, RSS, search, log, authentication, and torrent-creator workflows while handling important API differences between qBittorrent versions.

## ✨ Key Features

- **Broad Web API Support**: Service-oriented access to qBittorrent's major Web API modules.
- **Version-Aware Compatibility**: Detects server versions, checks endpoint availability, and translates renamed parameters where needed.
- **Reliable Authentication**: Maintains sessions and automatically re-authenticates after authorization failures.
- **Resilient Networking**: Supports retries, timeouts, diagnostics, and optional IPv4 fallback for dual-stack hosts.
- **Async and Cancellable**: Public network operations use `Task` and accept `CancellationToken`.
- **NativeAOT Ready**: Uses source-generated JSON metadata and is validated by a NativeAOT smoke application.
- **Modern .NET Support**: Targets .NET 8, .NET 9, and .NET 10.
- **Efficient Uploads**: Streams torrent files instead of loading complete files into memory.

## 📦 Installation

Install the package from NuGet:

```bash
dotnet add package Banned.Qbittorrent
```

## 🚀 Quick Start

### 1. Initialize the Client

`QBittorrentClient.Create` logs in and negotiates the Web API and qBittorrent application versions.

```csharp
using Banned.Qbittorrent;

using var client = await QBittorrentClient.Create(
    "http://localhost:8080",
    "admin",
    "adminadmin");
```

Use `QBittorrentClientOptions` for custom networking behavior. A caller-provided `HttpClient` remains caller-owned and is not modified or disposed by the library.

### 2. Torrent Management

```csharp
using Banned.Qbittorrent.Models.Requests;

var torrents = await client.Torrent.GetTorrentInfos();

await client.Torrent.AddTorrent(new AddTorrentRequest
{
    Urls = ["magnet:?xt=urn:btih:..."],
    SavePath = "/downloads/movies",
    Tags = "movies"
});

await client.Torrent.PauseTorrents(["hash1", "hash2"]);
await client.Torrent.ResumeTorrents(["hash1", "hash2"]);
```

### 3. Application Preferences

```csharp
var preferences = await client.Application.GetApplicationPreferences();
if (preferences is not null)
{
    preferences.AlternativeWebUiEnabled = true;
    await client.Application.SetApplicationPreferences(preferences);
}
```

## 🔄 Compatibility

The client supports qBittorrent Web API versions through v2.16.2, including the qBittorrent 5.3.0 endpoints.
Versioned operations check the server API version and throw `QbittorrentNotSupportedException` before sending
requests to older servers. See the [CHANGELOG](https://github.com/banned2054/Banned.Qbittorrent/blob/main/Docs/CHANGELOG.md)
for endpoint additions and compatibility changes.

## 🛠 Project Architecture

| Service | Responsibility |
| --- | --- |
| Application | Versions, build information, preferences, cookies, free space, and server controls. |
| Authentication | Login, logout, and session recovery. |
| Torrent | Torrents, files, local file paths, trackers, peers, categories, tags, limits, and queue controls. |
| Transfer | Global transfer statistics, session controls, speed limits, and peer banning. |
| Sync | Main-data and torrent-peer synchronization. |
| RSS | Feeds, articles, and automatic download rules. |
| Search | Search jobs, results, categories, and plugins. |
| Log | Main and peer log access. |
| TorrentCreator | Torrent creation tasks, status, output, and deletion. |

## 📜 Changelog

[View CHANGELOG](https://github.com/banned2054/Banned.Qbittorrent/blob/main/Docs/CHANGELOG.md)

## ⚖️ License

Copyright (c) 2026 banned.

Licensed under the [Apache License 2.0](./LICENSE). See [NOTICE](./NOTICE) for upstream acknowledgements and third-party notices.

## 🤝 Contributing

Issues and pull requests are welcome. Please include a focused description and tests for behavior changes where practical.

---

Inspired by [qbittorrent-api](https://github.com/rmartin16/qbittorrent-api) and the [official qBittorrent WebUI API documentation](<https://github.com/qbittorrent/qBittorrent/wiki/WebUI-API-(qBittorrent-5.0)>).
