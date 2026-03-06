# Changelog

All notable changes to this project will be documented in this file.  

This format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),  and this project adheres to [Semantic Versioning](https://semver.org/).

## 📘 Versions

- [v1.1.0](#-release-v110--performance-optimizations--flexibility-enhancements)
- [v1.0.0](#-release-v100--full-api-completion--unified-standard)
- [v0.1.1](#-release-v011--authentication-service--network-refactoring)
- [v0.1.0](#-release-v010--application-preferences-api--net-10-upgrade)
- [v0.0.9](#-release-v009--torrent-api-completion--category-management)
- [v0.0.8](#-release-v008--qbittorrent-net-client-tag-management--api-refinement)
- [v0.0.7](#-release-v007--qbittorrent-net-client-refinement)
- [v0.0.6](#-release-v006--qbittorrent-net-client-enhancement)
- [v0.0.5](#-release-v005--qbittorrent-net-client-update)

## 🚀 Release v1.1.0 — Performance Optimizations & Flexibility Enhancements

**Release Date:** 2026-03-06

This minor release focuses on **performance optimizations**, **flexibility enhancements**, and **observability improvements** to the qBittorrent .NET client library. It introduces parallel request execution, memory usage optimizations, and enhanced configuration options.

---

### ✨ Added

* **Parallel Request Execution**
  - Added `ExecuteParallelRequests` method to `NetService` for executing multiple requests concurrently
  - Significantly improves performance when fetching multiple pieces of data simultaneously

* **Enhanced Client Configuration**
  - Added `maxRetries` parameter to `QBittorrentClient.Create` for customizing retry behavior
  - Added `timeout` parameter for setting custom request timeout
  - Added `enableDetailedLogging` parameter for verbose operation logging

* **Detailed Logging**
  - Added `EnableDetailedLogging` property to `NetService`
  - Added detailed log output for request attempts, retries, and file uploads

---

### 🔧 Changed

* **Unified Client Creation**
  - Merged two `Create` methods in `QBittorrentClient` into a single method with optional parameters
  - Simplified API surface while maintaining backward compatibility

* **NetService Refactoring**
  - Updated `NetService` constructor to accept optional `HttpClient` and `timeout` parameters
  - Added `MaxRetries` property for centralized retry configuration

* **TorrentService Parameter Alignment**
  - Updated parameter names in `TorrentService` to maintain consistency with naming conventions
  - Changed `Reverse` to `ReverseEnabled` for better clarity

---

### 🚀 Optimized

* **Memory Usage**
  - Improved file upload mechanism to use `FileStream` instead of `File.ReadAllBytes`
  - Reduces memory consumption when uploading large torrent files

* **Retry Mechanism**
  - Enhanced `ExecuteWithRetry` method to support asynchronous request factories
  - Improved retry logic with detailed delay calculations and logging

* **Performance**
  - Added parallel request execution capability for faster data retrieval
  - Optimized internal request handling for better throughput

---

### 📦 Notes

This is a **minor release** with no breaking changes. All existing code will continue to work as before, while new features and optimizations are available for those who wish to use them.

Key improvements include:
- Better performance for multi-request scenarios
- Reduced memory usage during file uploads
- More flexible configuration options
- Enhanced observability through detailed logging

## 🚀 Release v1.0.0 — Full API Completion & Unified Standard

**Release Date:** 2026-02-20

This major milestone marks the **100% completion** of the qBittorrent Web API implementation. Version 1.0.0 introduces the remaining service modules, full documentation coverage, and a significant refactoring of naming conventions to ensure long-term consistency and predictability across the library.

---

### ✨ Added

* **Full API Coverage:** – Implemented all remaining core services:
    * `ApplicationService` – System state and server controls.
    * `LogService` – Server log and main log access.
    * `TransferService` – Global transfer info and speed limits.
    * `SyncService` – Main and torrent-specific synchronization.
    * `RssService` – RSS feeds, articles, and automated rules.
    * `SearchService` – Search jobs, plugin management, and results.


* **Comprehensive Documentation:** Added XML documentation comments to all public classes, properties, and methods for enhanced IntelliSense support.

---

### 🔧 Changed (Breaking Changes)

* **Unified Boolean Naming:** To eliminate ambiguity, most boolean properties have been renamed from `IsXXX` or `EnableXXX` to the `XXXEnabled` pattern.
* **Standardized Property Suffixes:** Replaced inconsistent suffixes like `Total`, `Num`, or `List` with `Count` and other descriptive terms.

**Key Renaming Mapping:**

| Old Name | New Name (v1.0.0) |
| --- | --- |
| `DiskCacheTtlSeconds` | `DiskCacheTtl` |
| `dht` | `DistributedHashTableEnabled` |
| `pex` | `PeerExchangeEnabled` |
| `lsd` | `LocalServiceDiscoveryEnabled` |
| `EnableCoalesceReadWrite` | `CoalesceReadWriteEnabled` |
| `EnableEmbeddedTracker` | `EmbeddedTrackerEnabled` |
| `IsSeed` / `IsPrivate` | `SeedEnabled` / `PrivateEnabled` |
| `AutoTmm` / `ForceStart` | `AutoTmmEnabled` / `ForceStartEnabled` |
| `SeqDl` / `SuperSeeding` | `SequentialDownloadEnabled` / `SuperSeedingEnabled` |
| `IsQueueingEnabled` | `QueueingEnabled` |
| `UseAltSpeedLimits` | `AltSpeedLimitsEnabled` |
| `PeersTotal` / `NumPeers` | `PeersCount` |
| `SeedsTotal` / `NumSeeds` | `SeedsCount` |
| `NumLeeches` / `NumDownloaded` | `LeechesCount` / `DownloadedCount` |
| `DhtNodes` | `DhtNodesCount` |

---

### 🧱 Completed Module

* ✅ **100% API Implementation:** Every endpoint group defined in the official qBittorrent Web API is now fully supported.

---

### 🧹 Improved

* **Naming Consistency:** Performed a global sweep to align method signatures and property names with the new unified standard.
* **Internal Refactoring:** Improved internal mapping logic and exception handling for better stability.

---

### 📦 Notes

This is a **major release** and contains **breaking changes** regarding property names. If you are upgrading from any v0.x.x version, please refer to the renaming table above to update your implementation.

## 🚀 Release v0.1.1 — Authentication Service & Network Refactoring

**Release Date:** 2025-12-06

This release focuses on **internal architectural improvements**, decoupling authentication logic from core network utilities.  
It introduces a dedicated **Authentication Service** and moves network handling to a new namespace to better align with the qBittorrent API structure.

---

### ✨ Added

- Added **AuthenticationService**
  - Implements explicit `Login` and `Logout` functionality.
  - Authentication logic is now separated from generic network requests, adhering to the API endpoint categorization.

---

### 🔧 Changed

- **Refactored Network Utilities:**
  - Moved `NetUtils` from `Banned.Qbittorrent.Utils.NetUtils` to `Banned.Qbittorrent.Services.NetServices`.
- **Decoupled Authentication Logic:**
  - Login operations are no longer implicitly handled within internal `Get` and `Post` network calls.
  - Authentication is now managed independently via the new `AuthenticationService`.

---

### 📦 Notes

This is primarily a structural refactor to improve code organization and maintainability.  
If your code directly references the internal `NetUtils` class, please update your namespace references to `Banned.Qbittorrent.Services`.

## 🚀 Release v0.1.0 — Application Preferences API & .NET 10 Upgrade

**Release Date:** 2025-11-21

This release introduces the new **Application Preferences API**, adds a dedicated **CHANGELOG file**, and upgrades the entire project to **.NET 10** for improved performance and long-term platform support.

---

### ✨ Added

- Added **Application Preferences API**
  - `GetApplicationPreferences` – Retrieve all qBittorrent application settings.
  - `SetApplicationPreferences` – Update application preference settings.

- Added root-level **CHANGELOG.md** file  
  - All releases are now properly documented in a unified changelog format.

---

### 🔧 Changed

- Updated target frameworks:  
  → **Now supports .NET 10.0**.

---

### 📦 Notes

This is a minor feature release that extends functionality into the application-level API and aligns the project with the latest .NET ecosystem (.NET 10).  
No breaking changes are introduced, but users should update target frameworks accordingly if building custom extensions.

## 🚀 Release v0.0.9 — Torrent API Completion & Category Management

**Release Date:** 2025-10-11  

This release completes the **entire Torrent API module**, introduces full **category management support**, and expands **bandwidth, share, and automation controls** for torrents.  
It also adds multi-target framework support (.NET 8 & 9) and fixes a critical tag deletion bug.  

---

### ✨ **Added**

* Added full **Category Management API**  
  - `CreateCategory` – Create a new category.  
  - `EditCategory` – Edit an existing category.  
  - `DeleteCategory`, `DeleteCategories` – Remove one or more categories.  

* Added **Torrent Download Limit** controls  
  - `GetTorrentDownloadLimit`, `GetTorrentsDownloadLimit`  
  - `SetTorrentDownloadLimit`, `SetTorrentsDownloadLimit`, `SetAllTorrentsDownloadLimit`  

* Added **Torrent Upload Limit** controls  
  - `GetTorrentUploadLimit`, `GetTorrentsUploadLimit`  
  - `SetTorrentUploadLimit`, `SetTorrentsUploadLimit`, `SetAllTorrentsUploadLimit`  

* Added **Torrent Share Limit** management  
  - `SetTorrentShareLimit`, `SetTorrentsShareLimit`, `SetAllTorrentsShareLimit`  

* Added **Automatic Torrent Management** toggles  
  - `SetTorrentAutoManagement`, `SetTorrentsAutoManagement`, `SetAllTorrentsAutoManagement`  

* Added **Sequential Download** toggles  
  - `ToggleTorrentSequentialDownload`, `ToggleTorrentsSequentialDownload`, `ToggleAllTorrentsSequentialDownload`  

* Added **First/Last Piece Priority** toggles  
  - `ToggleTorrentFirstLastPiecePriority`, `ToggleTorrentsFirstLastPiecePriority`, `ToggleAllTorrentsFirstLastPiecePriority`  

* Added **Force Start** controls  
  - `SetTorrentForceStart`, `SetTorrentsForceStart`, `SetAllTorrentsForceStart`  

* Added **Super Seeding** controls  
  - `SetTorrentSuperSeeding`, `SetTorrentsSuperSeeding`, `SetAllTorrentsSuperSeeding`  

---

### 🧱 **Completed Module**

* ✅ All APIs under the **TorrentService** are now fully implemented.  
  This marks the completion of the entire `Torrents` endpoint group.

---

### 🔧 **Changed**

* Updated target frameworks:  
  → **Now supports both .NET 8.0 and .NET 9.0**.  

---

### 🐞 **Fixed**

* Fixed incorrect version bug in **DeleteTags** API.  
* Reviewed and corrected **function signatures** and **XML documentation** for accuracy and consistency.  

---

### 🧹 **Improved**

* Unified code style, verified all function comments, and corrected mis-named or mis-mapped API calls.  
* Enhanced developer readability and method naming consistency across single/multi/all-torrent operations.  

---

### 📦 **Notes**

This release finalizes all torrent-related endpoints and improves reliability across management APIs.  
If you’re upgrading from v0.0.8, no breaking changes are expected, but it’s recommended to re-check custom wrappers for renamed methods or documentation updates.  

## 🚀 Release v0.0.8 — qBittorrent .NET Client Tag Management & API Refinement

**Release Date:** 2025-10-01

This release introduces comprehensive tag management APIs, improves naming consistency, and refines parameter handling for multi-torrent operations.

---

### 🇨🇳 **Happy National Day!**

Wishing everyone a joyful and relaxing **National Day** 🎆  
Thank you for supporting the project — enjoy the holidays and happy coding!

---

### ✨ **Added**

* Added **CreateTag** and **CreateTags** functions for creating one or more tags.  
* Added **DeleteTag** and **DeleteTags** functions for removing tags from qBittorrent.  
* Added **RemoveTorrentTag**, **RemoveTorrentTags**, **RemoveTorrentsTag**, and **RemoveTorrentsTags** functions for detaching tags from torrents.  
* Added **AddTorrentTag**, **AddTorrentTags**, **AddTorrentsTag**, and **AddTorrentsTags** functions for assigning tags to torrents.

---

### 🔧 **Changed**

* `GetTorrentInfo` now retrieves **only a single** `TorrentInfo` object.  
  > ⚠️ Note: the **parameter order** for `GetTorrentInfo` and `GetTorrentInfos` has changed.  
* All pluralized parameter names (e.g. multiple `hashes`) now use plural form instead of “List” suffix for consistency.  
* Updated function names to match singular/plural logic — e.g., `AddPeers` → `AddTorrentPeer` / `AddTorrentPeers` based on argument count.  
* Corrected and standardized **ArgumentException** descriptions for better clarity.

---

### 🧹 **Improved**

* Parameter naming and API design now follow a unified and predictable convention between single- and multi-torrent operations.  
* Improved internal code readability and developer ergonomics.

---

### 📦 **Notes**

This release introduces minor breaking changes in method signatures (parameter order and naming).  
Please review your code when upgrading from earlier versions, especially for functions involving **GetTorrentInfo** or **peer/tag operations**.

## 🚀 Release v0.0.7 — qBittorrent .NET Client Refinement

**Release Date:** 2025-09-30

This release fixes core torrent control issues, adds new torrent management APIs, and aligns function organization with the official qBittorrent API documentation.

---

### 🐛 **Fixed**

* Fixed a bug affecting **Resume Torrent** and **Pause Torrent** functions, ensuring they now work as expected.

---

### ✨ **Added**

* Added **IncreaseTorrentPriority**, **DecreaseTorrentPriority**, **MaximalTorrentPriority**, and **MinimalTorrentPriority** functions for fine-grained torrent queue control.  
* Added **SetTorrentCategory**, **GetAllCategories**, and **GetAllTags** functions for category and tag management.

---

### 🔧 **Changed**

* Reorganized the source code to follow the **same order as the official qBittorrent API documentation**, improving maintainability and readability.

---

### 📦 **Notes**

This update is fully backward-compatible.  
Users upgrading from previous versions do not need to change existing code unless they wish to use the new priority or category management APIs.

## 🚀 Release v0.0.6 — qBittorrent .NET Client Enhancement

**Release Date:** 2025-09-29

This release expands torrent management capabilities, refines API version handling, and introduces an API implementation progress document for better visibility.

---

### ✨ **Added**

* Added **API Implementation documentation** to track feature completion progress.  
* Added support for **retrieving torrent web seeds**.  
* Added the **Add Peers** API for manually adding peers to a torrent.  
* Added **Get torrent pieces’ states** and **Get torrent pieces’ hashes** functions.

---

### 🔧 **Changed**

* Reworked the **API version system** — `ApiVersion` has been converted from a *class* to a *static struct* for improved efficiency and reduced allocation overhead.  
* Renamed `EditTracker` → `EditTorrentTracker` for consistency.  
* Renamed `RemoveTrackers` → `RemoveTorrentTrackers` for clarity.  
* Renamed `GetTorrentContents` → `GetTorrentFiles` to better reflect qBittorrent’s API naming.

---

### 🧹 **Improved**

* Reduced unnecessary `new` allocations for API version references.  
* Improved internal consistency and readability in torrent-related service methods.

---

### 📦 **Notes**

This update is fully backward-compatible except for renamed tracker-related methods and the `GetTorrentFiles` function.  
If you previously used `EditTracker()` or `GetTorrentContents()`, please update them to `EditTorrentTracker()` and `GetTorrentFiles()` respectively.

## 🚀 Release v0.0.5 — qBittorrent .NET Client Update

**Release Date:** 2025-09-27

This release adds tracker management and shutdown control features, improves code consistency, and refines the public API design.

---

### ✨ **Added**

* Added support for **editing** and **removing trackers**.
* Added the ability to **shut down qBittorrent** remotely via the Web API.

---

### 🔧 **Changed**

* Removed the `Async` suffix from all function names for a cleaner API surface.

  > (All functions remain asynchronous internally.)
* Standardized and improved **XML documentation comments** across all public methods.

---

### 🧹 **Improved**

* API calls with no return data now use **void-returning methods**, matching the qBittorrent API behavior.
* General internal refactoring for better maintainability and clarity.

---

### 📦 **Notes**

This update is fully backward-compatible with previous versions, except for function name changes (removed `Async`).
If you were previously calling methods like `AddTorrentAsync()`, please update them to `AddTorrent()`.