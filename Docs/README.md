# Banned.Qbittorrent

[English Documentation](../README.md)

[![License](https://img.shields.io/badge/license-Apache_2.0-green)](../LICENSE)[![Ask DeepWiki](https://deepwiki.com/badge.svg)](https://deepwiki.com/banned2054/Banned.Qbittorrent)

一个基于 .NET 的 qBittorrent Web API 客户端库，参考了 [qbittorrent-api](https://github.com/rmartin16/qbittorrent-api) 和官方 qBittorrent wiki。相比 [qbittorrent-net-client](https://github.com/fedarovich/qbittorrent-net-client)，本库将许多 API 请求从 GET 改为 POST，以获得更好的数据同步效果。

## 安装

通过 NuGet 包管理器安装：

```bash
dotnet add package Banned.Qbittorrent
```

## 快速开始

```csharp
using Banned.Qbittorrent;

// 创建带认证的客户端实例
var client = await QbittorrentClient.Create("http://localhost:8080", "username", "password");

// 获取种子列表
var torrents = await client.GetTorrentList();

// 添加新种子
await client.AddTorrent("magnet:?xt=urn:btih:...");

// 暂停种子
await client.PauseTorrents([ "torrent_hash" ]);

// 恢复种子
await client.ResumeTorrents([ "torrent_hash" ]);
```

## 许可证

本项目采用 Apache-2.0 许可证。

## 贡献

欢迎提交 Issue 和 Pull Request！ 

## API 实现进度

[📘 查看 API 实现进度](https://github.com/banned2054/Banned.Qbittorrent/blob/main/Docs/API%20Implementation.md)

## Changelog

[🧾查看 CHANGELOG](https://github.com/banned2054/Banned.Qbittorrent/blob/main/Docs/CHANGELOG.md)