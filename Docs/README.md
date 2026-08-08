# Banned.Qbittorrent

[English](https://github.com/banned2054/Banned.Qbittorrent/blob/main/README.md) | 简体中文

[![NuGet](https://img.shields.io/nuget/v/Banned.Qbittorrent.svg)](https://www.nuget.org/packages/Banned.Qbittorrent) [![Downloads](https://img.shields.io/nuget/dt/Banned.Qbittorrent.svg)](https://www.nuget.org/packages/Banned.Qbittorrent) [![License](https://img.shields.io/badge/license-Apache_2.0-green)](../LICENSE)

**Banned.Qbittorrent** 是一个强类型、异步的 qBittorrent Web API .NET 客户端，覆盖应用、种子、传输、同步、RSS、搜索、日志、认证和 Torrent Creator 等主要工作流，并处理不同 qBittorrent 版本间的重要 API 差异。

## ✨ 核心特性

- **广泛的 Web API 支持**：按服务模块访问 qBittorrent 的主要 Web API。
- **版本兼容处理**：检测服务端版本、检查端点可用性，并转换有名称变化的参数。
- **可靠的身份验证**：维护登录会话，并在认证失效后自动重新登录。
- **弹性网络处理**：支持重试、超时、诊断，以及双栈主机的可选 IPv4 回退。
- **异步与取消**：公开网络操作使用 `Task` 并接受 `CancellationToken`。
- **NativeAOT 支持**：使用源生成 JSON 元数据，并通过 NativeAOT 冒烟项目验证。
- **现代 .NET 支持**：面向 .NET 8、.NET 9 和 .NET 10。
- **高效上传**：流式读取 Torrent 文件，避免一次性载入内存。

## 📦 安装

从 NuGet 安装：

```bash
dotnet add package Banned.Qbittorrent
```

## 🚀 快速上手

### 1. 初始化客户端

`QBittorrentClient.Create` 会自动登录，并协商 Web API 与 qBittorrent 应用版本。

```csharp
using Banned.Qbittorrent;

using var client = await QBittorrentClient.Create(
    "http://localhost:8080",
    "admin",
    "adminadmin");
```

需要自定义网络行为时可使用 `QBittorrentClientOptions`：

```csharp
using Banned.Qbittorrent.Models;
using Banned.Qbittorrent.Models.Enums;

var options = new QBittorrentClientOptions
{
    AddressFamilyPreference = AddressFamilyPreference.System,
    EnableAutomaticIPv4Fallback = true,
    ConnectTimeout = TimeSpan.FromSeconds(5),
    DiagnosticSink = message => Console.Error.WriteLine(message)
};

using var client = await QBittorrentClient.Create(
    "https://qbittorrent.example.com:8443",
    "admin",
    "adminadmin",
    options);
```

调用者传入的 `HttpClient` 始终由调用者管理，本库不会修改或释放它。

### 2. 种子管理

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

### 3. 应用程序首选项

```csharp
var preferences = await client.Application.GetApplicationPreferences();
if (preferences is not null)
{
    preferences.AlternativeWebUiEnabled = true;
    await client.Application.SetApplicationPreferences(preferences);
}
```

## 🛠 项目架构

| 服务 | 职责 |
| --- | --- |
| Application | 版本、构建信息、偏好设置、Cookie 与服务器控制。 |
| Authentication | 登录、注销与会话恢复。 |
| Torrent | 种子、文件、Tracker、Peer、分类、标签、限速与队列控制。 |
| Transfer | 全局传输统计、速度限制与 Peer 封禁。 |
| Sync | 主数据与 Torrent Peer 增量同步。 |
| RSS | 订阅源、文章与自动下载规则。 |
| Search | 搜索任务、结果、分类与插件。 |
| Log | 主日志与 Peer 日志。 |
| TorrentCreator | Torrent 创建任务、状态、文件获取与删除。 |

## 📜 更新日志

[查看 CHANGELOG](https://github.com/banned2054/Banned.Qbittorrent/blob/main/Docs/CHANGELOG.md)

## ⚖️ 开源协议

Copyright (c) 2026 banned.

本项目基于 [Apache License 2.0](../LICENSE) 开源。上游致谢与第三方声明参阅 [NOTICE](../NOTICE)。

## 🤝 参与贡献

欢迎提交 Issue 和 Pull Request。涉及行为变化时，请尽量附上聚焦的说明与测试。

---

本项目受 [qbittorrent-api](https://github.com/rmartin16/qbittorrent-api) 和 [qBittorrent 官方 WebUI API 文档](<https://github.com/qbittorrent/qBittorrent/wiki/WebUI-API-(qBittorrent-5.0)>) 启发。
