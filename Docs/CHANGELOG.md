# Changelog

All notable changes to this project will be documented in this file.  

This format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),  and this project adheres to [Semantic Versioning](https://semver.org/).

## 📘 Versions

- [v0.0.9](#release-v009--torrent-api-completion--category-management)
- [v0.0.8](#release-v008--qbittorrent-net-client-tag-management--api-refinement)
- [v0.0.7](#release-v007--qbittorrent-net-client-refinement)
- [v0.0.6](#release-v006--qbittorrent-net-client-enhancement)
- [v0.0.5](#release-v005--qbittorrent-net-client-update)

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