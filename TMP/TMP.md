# 临时与过程文件索引

本目录存放整理过程中的临时文件、缓存与旧仓库残留。**内容不入库**（根 `.gitignore` 的 `TMP/*` 只放行本文件），也不随发布包分发。

目录划分：

| 子目录 | 内容 |
|---|---|
| `01-旧Git仓库/` | 嵌套的旧 `.git` 克隆（整体移入，不删除） |
| `02-缓存与临时/` | 构建缓存、中间产物、过程文件 |
| `03-运行日志/` | 构建/运行日志 |
| `04-历史版本源码/` | 被新版本取代的旧源码快照（溯源用） |

## 文件清单

| 时间 | 文件/目录 | 用途 | 格式 | 大小 | 来源 |
|---|---|---|---|---|---|
| 2026-11-01 | `02-缓存与临时/CorpseExplosionReplacementMod-godot/` | Godot 编辑器缓存：导入的 `.ctex`/`.import` 元数据、`mono/temp` 下的 Debug DLL/PDB 与 NuGet 中间文件，共 47 个文件 | 二进制 + 文本（缓存目录） | 3.5 MB | 原合集 `src/CorpseExplosionReplacementMod/.godot/`（整理时从 `9.SRC/` 移出，源码目录不再携带缓存） |
| 2026-11-01 | `04-历史版本源码/CorpseExplosionReplacementMod-远程v1.0/`（**已于 2026-11-01 清理**） | 远端 `origin/main`（2026-08-09，版本 1.0）中被本地 v1.0.0 取代的 4 个文件：`PortraitTextureLoader.cs`、`PowerIconTextureLoader.cs`、`CorpseExplosionReplacementMod.json`、`CorpseExplosionReplacementMod/mod_image.png` | 文本 + PNG | 4.5 MB | `git show origin/main:<path>`（本地 mtime 2026-08-21 优于远端提交时间 2026-08-09，按「新的胜出」替换，旧版本留档溯源） |
| — | `01-旧Git仓库/` | 空。合集目录内不存在嵌套的旧 `.git`，无需归档 | 目录 | 0 | — |
| — | `03-运行日志/` | 空。合成阶段未产生独立构建日志 | 目录 | 0 | — |

## 清理建议

- `02-缓存与临时/CorpseExplosionReplacementMod-godot/` 可在下次构建时由 Godot 自动重建，确认 `9.SRC/` 源码完整后即可清理，可释放约 3.5 MB。
- 若保留，请注意其中的 `.dll`/`.pdb` 是历史构建中间产物，不代表当前发布版本。
- `01-旧Git仓库/` 与 `03-运行日志/` 当前为空；空目录不进入 git，本地可保留以便后续归档。
- 游戏 mods 目录下的 `.sts2-workshop-uploader/` 属于上传器运行痕迹，位于游戏安装目录，不在本仓库内；打包发布资产时必须排除。

## 2026-11-01 清理记录

- `TMP/01-旧Git仓库/`、`TMP/02-缓存与临时/` 与 `TMP/04-历史版本源码/` 的内容已全部移入 Windows 回收站。
- 清理依据：远端仓库已完整保留历史，构建缓存可再生；需要回滚时从远端 `git log` 或 `10.OUTPUT/` 取回。
