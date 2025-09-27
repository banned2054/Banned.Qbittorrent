# Banned.Qbittorrent

[中文文档](https://github.com/banned2054/Banned.Qbittorrent/blob/master/Docs/README.md)

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
var client = await QbittorrentClient.CreateAsync("http://localhost:8080", "username", "password");

// Get torrent list
var torrents = await client.GetTorrentInfosAsync();

// Add a new torrent
await client.AddTorrentAsync("magnet:?xt=urn:btih:...");

// Pause torrents
await client.PauseTorrentsAsync(new[] { "torrent_hash" });

// Resume torrents
await client.ResumeTorrentsAsync(new[] { "torrent_hash" });
```

## License

This project is licensed under the Apache License 2.0.

## Contributing

Contributions are welcome! Feel free to submit issues and pull requests.
