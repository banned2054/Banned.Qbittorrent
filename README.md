# Banned.Qbittorrent

[中文文档](https://github.com/banned2054/Banned.Qbittorrent/blob/main/Docs/README.md)

[![License](https://img.shields.io/badge/license-Apache_2.0-green)](./LICENSE) [![Ask DeepWiki](https://deepwiki.com/badge.svg)](https://deepwiki.com/banned2054/Banned.Qbittorrent)

A .NET client library for qBittorrent's Web API, inspired by [qbittorrent-api](https://github.com/rmartin16/qbittorrent-api) and the official qBittorrent wiki. This library improves upon [qbittorrent-net-client](https://github.com/fedarovich/qbittorrent-net-client) by using POST requests instead of GET for better data synchronization.

## Installation

Install via NuGet Package Manager:

```bash
dotnet add package Banned.Qbittorrent
```

## Quick Start

```csharp
using Banned.Qbittorrent;

// Create a client instance with authentication
var client = await QbittorrentClient.Create("http://localhost:8080", "username", "password");

// Get torrent list
var torrents = await client.GetTorrentInfos();

// Add a new torrent
await client.AddTorrent("magnet:?xt=urn:btih:...");

// Pause torrents
await client.PauseTorrents([ "torrent_hash" ]);

// Resume torrents
await client.ResumeTorrents([ "torrent_hash" ]);
```

## License

This project is licensed under the Apache License 2.0.

## Contributing

Contributions are welcome! Feel free to submit issues and pull requests.

## Implementation Status

[📘 API Implementation List](https://github.com/banned2054/Banned.Qbittorrent/blob/main/Docs/API%20Implementation.md)

## Changelog

[🧾 View CHANGELOG](https://github.com/banned2054/Banned.Qbittorrent/blob/main/Docs/CHANGELOG.md)
