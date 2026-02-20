# Banned.Qbittorrent

[English Version](https://github.com/banned2054/Banned.Qbittorrent/blob/main/README.md)

[![License](https://img.shields.io/badge/license-Apache_2.0-green)](./LICENSE) [![NuGet](https://img.shields.io/nuget/v/Banned.Qbittorrent.svg)](https://www.nuget.org/packages/Banned.Qbittorrent)

**Banned.Qbittorrent** 是一个高性能、强类型的 qBittorrent Web API .NET 客户端库。它采用现代异步设计，功能强大且稳健，实现了对 qBittorrent 功能集的完整覆盖。

## ✨ 核心特性

* **完整的 API 实现**: 全面覆盖种子管理（Torrent）、应用程序设置、RSS、搜索以及同步（Sync）模块。
* **自动版本协商**: 自动检测 Web API 版本，并针对特定功能执行兼容性检查。
* **智能身份验证**: 内置会话保持（Keep-alive）机制与自动重新登录逻辑。
* **弹性网络处理**: 包含指数退避（Exponential Backoff）重试机制，从容应对瞬时网络抖动。
* **异步优先**: 原生支持基于 `Task` 的异步模式及 `CancellationToken` 取消令牌。

## 📦 安装

通过 NuGet 包管理器安装：

```bash
dotnet add package Banned.Qbittorrent
```

## 🚀 快速上手

1. 初始化客户端
    使用静态工厂方法 `Create` 自动处理初始身份验证和 API 版本协商。

```csharp
using Banned.Qbittorrent;

// 自动登录并配置版本兼容性
var client = await QBittorrentClient.Create("http://localhost:8080", "admin", "adminadmin");
```
2. 种子管理
```csharp
// 获取所有种子信息
var torrents = await client.Torrent.GetInfos();

// 通过磁力链接添加新种子
await client.Torrent.Add(new AddTorrentsRequest {
    Urls = new[] { "magnet:?xt=urn:btih:..." },
    SavePath = "/downloads/movies"
});

// 管理特定种子
var hashes = new[] { "hash1", "hash2" };
await client.Torrent.Pause(hashes);
await client.Torrent.Resume(hashes);
```
3. 修改应用程序首选项
```csharp
// 获取当前设置
var prefs = await client.Application.GetApplicationPreferences();

// 修改并保存
prefs.AlternativeWebuiEnabled = true;
await client.Application.SetApplicationPreferences(prefs);
```

## 🛠 项目架构

| 服务模块       | 描述                                             |
| -------------- | ------------------------------------------------ |
| Application    | 包含应用版本、构建信息、偏好设置及 Cookie 管理。 |
| Authentication | 负责登录、注销及会话持久化。                     |
| Torrent        | 管理种子、分类、标签及文件优先级。               |
| Transfer       | 管理全局限速、传输统计数据及状态信息。           |
| Sync           | 核心数据同步及增量状态更新。                     |
| Rss            | 管理 RSS 订阅源及自动下载规则。                  |
| Search         | 管理搜索引擎任务及插件。                         |
| Log            | 系统事件日志及 P2P 连接日志。                    |

## 📜 更新日志

[🧾 查看 CHANGELOG](https://github.com/banned2054/Banned.Qbittorrent/blob/main/Docs/CHANGELOG.md)

## ⚖️ 开源协议

本项目基于 Apache License 2.0 协议开源。详情请参阅 [LICENSE](https://github.com/banned2054/Banned.Qbittorrent/blob/main/LICENSE.txt) 文件。

## 🤝 参与贡献

欢迎任何形式的贡献！如果您发现 Bug 或有新功能建议，请提交 Issue。我们也期待您的 Pull Request。

---
本项目灵感源自 [qbittorrent-api](https://github.com/rmartin16/qbittorrent-api) 和 [qBittorrent 官方 WebUI Wiki](https://github.com/qbittorrent/qBittorrent/wiki/WebUI-API-(qBittorrent-5.0)).