# 构建产物与发布包索引

本目录存放该模组的编译产物、发布包与历史备份。**内容不入库**（见根 `.gitignore` 的 `10.OUTPUT/*` 规则），只有本索引 `output.md` 会被提交。

目录划分：

| 子目录 | 内容 |
|---|---|
| `01-构建产物/` | 历次构建产出的 DLL / PCK / PDB / manifest，按打包时间归档 |
| `02-发布包/` | 对外发布的 zip（Release 资产原件） |
| `03-备份/` | 构建前对已安装版本的备份，用于回滚 |

## 产物清单

| 时间 | 名称 | 版本号 | 大小 | 说明 |
|---|---|---|---|---|
| 2026-08-06 19:08 | `01-构建产物/CorpseExplosionReplacementMod v1（支持ritsulib，打包后mod，无法运行）/` | 0.1.0 | 1.5 MB（4 文件） | 首个构建，带 RitsuLib 设置页；启动期因重复注册 `CorpseExplosionPower` 崩溃，不能运行 |
| 2026-08-06 19:44 | `01-构建产物/CorpseExplosionReplacementMod v2（可运行但效果未触发，打包后mod）/` | 0.1.0 | 1.5 MB（4 文件） | 可启动，但尸爆死亡伤害不结算 |
| 2026-08-06 20:02 | `01-构建产物/CorpseExplosionReplacementMod v3（正常运行，打包后mod）/` | 0.1.1 | 1.5 MB（4 文件） | 修复 dealer 为空，爆炸伤害正常结算 |
| 2026-08-07 20:46 | `01-构建产物/CorpseExplosionReplacementMod v4（检测版本，修复费用，打包后mod）/` | 0.1.2 | 1.5 MB（4 文件） | 加入形态守卫与 `min_game_version=0.110.0`，修复费用缓存 |
| 2026-08-08 04:54 | `01-构建产物/CorpseExplosionReplacementMod v5（修改本地化键，屏蔽107，打包后mod）/` | 0.2.0 | 1.5 MB（4 文件） | 本地化键改 `RE_` 前缀，移除 RitsuLib |
| 2026-08-09 04:29 | `01-构建产物/CorpseExplosionReplacementMod v6（剔除冗余资源，打包后mod）/` | 0.2.2 | 1.8 MB（4 文件） | 剔除冗余资源，反射分派 `CreatureCmd.Damage` |
| 2026-08-09 16:40 | `01-构建产物/CorpseExplosionReplacementMod 正式版v1.0/` | 1.0 | 1.8 MB（4 文件） | 发布用构建，对应 GitHub Release v1.0 |
| 2026-08-09 22:56 | `02-发布包/CorpseExplosionReplacementModSourceCode.zip` | 1.0 | 5.2 MB | Release 源码资产 |
| 2026-08-09 22:59 | `02-发布包/CorpseExplosionReplacementMod v1.0.zip` | 1.0 | 1.7 MB | Release 模组资产（DLL/PCK/JSON/PDB） |
| 2026-08-21 19:41 | `03-备份/CorpseExplosionReplacementMod-20260821/` | 1.0 | 1.8 MB（4 文件） | 图鉴修复前的已安装版本备份，回滚用 |
| 2026-11-01 20:36 | `02-发布包/CorpseExplosionReplacementMod.v1.0.0.zip` | 1.0.0 | 1.82 MB（4 文件） | 由 `build.ps1` 重新构建；dll 856576 B、pck 978512 B、pdb 19884 B，已替换 GitHub Release v1.0.0 的资产 |
| 2026-11-01 20:36 | `02-发布包/CorpseExplosionReplacementModSourceCode.zip` | 1.0.0 | 1.95 MB（78 条目） | 同上，源码包；排除 `.godot/`、`bin/`、`obj/` 与二进制产物 |
| 2026-11-13 09:17 | `01-构建产物/CorpseExplosionReplacementMod v7（0.113兼容，打包后mod）/` | 1.0.1 | 1.8 MB（dll 857,088 / pck 978,560 / json 723 / pdb 20,128 B） | 0.113 兼容修复构建：死亡钩子 Prefix + 5 参 Damage；同时已覆盖安装到游戏 `mods/CorpseExplosionReplacementMod/`；SHA-256 `31300894…98b445`（DLL）、`6a59c147…91d7ba`（PCK） |

## 说明

- 时间取自源目录中各目录/文件的时间戳；`10.OUTPUT/` 内的副本在整理时统一复制，文件系统时间已刷新为整理时间 2026-11-01。
- GitHub Release v1.0 上传时的资产名为 `CorpseExplosionReplacementMod.v1.0.zip`，与本地文件名只差分隔符（空格 → 点），内容一致；源码资产名相同。
- 版本号以对应构建的 `CorpseExplosionReplacementMod.json` 的 `version` 字段为准。
- 游戏当前安装目录 `C:\Game\Steam\steamapps\common\Slay the Spire 2\mods\CorpseExplosionReplacementMod\` 中的产物为 1.0.0（2026-08-21 构建，`version` 已规范化为 `1.0.0`），未归档到本目录——原始备份见 `03-备份/`。
- `01-构建产物/` 保留全部历史构建，便于对照问题；确认无用后可自行清理以节省空间（约 12 MB）。
| 2026-11-13 10:21 | `02-发布包/CorpseExplosionReplacementMod.v1.0.1.zip` | 1.0.1 | 1.82 MB（1,817,811 B） | GitHub Release v1.0.1 安装包；dll + pck + json + pdb |
| 2026-11-13 10:21 | `02-发布包/CorpseExplosionReplacementModSourceCode.v1.0.1.zip` | 1.0.1 | 1.95 MB（1,951,322 B） | v1.0.1 源码包；排除 `.godot/`、`bin/`、`obj/` 与二进制产物 |
| 2026-11-13 10:21 | GitHub Release `v1.0.1` | 1.0.1 | 两个资产 | <https://github.com/Rim-World/STS2Mod-Replace-Outbreak-with-CorpseExplosion/releases/tag/v1.0.1> |
