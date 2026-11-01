# 工具与脚本索引

本目录存放项目使用的工具，以及独立使用的自写脚本（放 `scripts/`）。工具各自占一个子目录，不暴露内部文件。

## 当前状态

本目录**目前没有随项目收纳的工具或自写脚本**。模组的构建脚本 `build.ps1` 是 Godot 工程的组成部分，随源码留在 `9.SRC/CorpseExplosionReplacementMod/build.ps1`，不复制到此处，避免两份脚本分叉。

## 外部依赖（未收入本目录）

| 时间 | 工具/脚本 | 用途 | 版本 | 大小 | 来源 | 安装方式/依赖 |
|---|---|---|---|---|---|---|
| — | .NET SDK | 编译 `net9.0` 程序集 | 9.x（具体版本未记录） | 未记录 | Microsoft 官方 | 全局安装，`dotnet build` |
| — | Godot Engine（mono） | 资源导入与 `--export-pack` 打包 pck | 4.5.1-stable mono win64 | 未记录 | Godot 官方 | 免安装，本机路径 `C:\Godot&GDRE\Godot_v4.5.1-stable_mono_win64\` |
| — | Slay the Spire 2 本体 | 提供 `sts2.dll`、`0Harmony.dll` 引用与 mods 安装目录 | ≥ 0.110.0（构建时 0.111.0） | 未记录 | Steam | 本机路径 `C:\Game\Steam\steamapps\common\Slay the Spire 2\` |
| — | Harmony（`0Harmony.dll`） | 运行期方法补丁 | 随游戏分发（版本未记录） | 未记录 | 游戏 `data_sts2_windows_x86_64/` | 由 csproj `Reference` 引用，`Private=false` |
| — | Git / GitHub CLI（`gh`） | 版本管理与 Release 发布 | 未记录 | 未记录 | 官方 | 全局安装；`gh` 已登录 Rim-World 账号 |

## 约定

- 新增工具：建独立同名子目录，登记「时间 / 工具 / 用途 / 版本 / 大小 / 来源 / 安装方式与依赖」。
- 新增独立脚本：放 `scripts/`，并在 `scripts/scripts.md` 登记用途与依赖。
- 工具运行产生的配置与数据留在工具目录内，不人为搬动。
