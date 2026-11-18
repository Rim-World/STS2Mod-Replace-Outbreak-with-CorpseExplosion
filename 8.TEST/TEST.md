# 测试文件索引

本目录存放开发过程中的测试工具与测试数据。被集成进源码和发布包的单独测试文件不在此登记。

## 目录约定

- 按游戏版本或 mod 版本建立子目录；本轮为 `1.1.0/`。
- `CI-CD/` 存放本地门禁脚本，不随版本清理。
- 每个版本最多登记 10 个小版本；满后新建 `NNN-测试文件登记.md`。

## 1.1.0

| 文件名 | 创建时间 | 是否清理 | 文件大小（KB） | 用途 | 引用和复用 |
|---|---|---|---|---|---|
| `CorpseExplosionProbe/CorpseExplosionProbe.csproj` | 2026-11-18 19:03 | 否 | 0.8 | 探针 mod 构建文件；引用 `sts2.dll`、`0Harmony.dll`、`GodotSharp.dll` | 是 |
| `CorpseExplosionProbe/ProbeConsoleCmd.cs` | 2026-11-18 19:21 | 否 | 3.1 | 注册 `ce_probe`，打印 Outbreak 属性与战斗生物血量/Power | 是 |
| `CorpseExplosionProbe/CheckConsoleCmd.cs` | 2026-11-18 19:19 | 否 | 1.5 | 注册 `ce_check`，断言 `combat-ended` / `combat-active` | 是 |
| `CorpseExplosionProbe/ReleaseModeUnlockPatch.cs` | 2026-11-18 19:03 | 否 | 0.5 | 测试专用：让发行版接受 `--scenario` | 是 |
| `CorpseExplosionProbe/CorpseExplosionProbe.json` | 2026-11-18 19:03 | 否 | 0.3 | 探针 mod 清单，`min_game_version=0.111.1` | 是 |
| `corpse-explosion-compat.scenario` | 2026-11-18 19:03 | 否 | 0.4 | 0.113.1 1 层尸爆场景：3 只 Corpse Slug，挂尸爆、击杀、探测血量 | 是 |
| `corpse-explosion-lethal.scenario` | 2026-11-18 19:19 | 否 | 0.3 | 0.113.1 2 层尸爆场景：断言剩余敌人死亡、战斗结束 | 是 |
| `run-compat.ps1` | 2026-11-18 19:16 | 否 | 3.2 | 检查游戏未运行、部署探针、启动场景、保存日志与退出码 | 是 |
| `logs/0.113.1-20261118-191748.engine.log` | 2026-11-18 19:18 | 否 | 220.1 | 1 层场景完整引擎日志；含 `ce_probe` 血量前后对照 | 是 |
| `logs/0.113.1-20261118-191748.stdout.log` | 2026-11-18 19:18 | 否 | 120.1 | 1 层场景标准输出 | 否 |
| `logs/0.113.1-20261118-191748.stderr.log` | 2026-11-18 19:18 | 否 | 102.1 | 1 层场景标准错误 | 否 |
| `logs/0.113.1-20261118-191748.summary.txt` | 2026-11-18 19:18 | 否 | 0.5 | 1 层场景退出码与 sentinel 摘要 | 是 |
| `logs/0.113.1-20261118-191933.engine.log` | 2026-11-18 19:20 | 否 | 194.7 | 2 层场景中间版本；探针仍调用 `GetFormattedText`，产生无关本地化告警 | 否 |
| `logs/0.113.1-20261118-191933.stdout.log` / `.stderr.log` / `.summary.txt` | 2026-11-18 19:20 | 否 | 118.4 / 78.2 / 0.5 | 同上 | 否 |
| `logs/0.113.1-20261118-192101.engine.log` | 2026-11-18 19:21 | 否 | 195.1 | 2 层最终场景完整引擎日志；`ce_check combat-ended` 通过 | 是 |
| `logs/0.113.1-20261118-192101.stdout.log` / `.stderr.log` / `.summary.txt` | 2026-11-18 19:21 | 否 | 118.4 / 78.6 / 0.5 | 同上 | 是 |
| `logs/0.113.1-20261118-192812.engine.log` | 2026-11-18 19:28 | 否 | 192.9 | 加入 `BigPortrait` 探针后的失败尝试；运行在进入战斗前被其他 mod 的 Steam 创意工坊更新干扰，未生成 sentinel | 否 |
| `logs/0.113.1-20261118-192930.engine.log` | 2026-11-18 19:30 | 否 | 194.1 | 最终场景完整引擎日志；`BigPortrait valid=True`、2 层尸爆全灭、`ce_check combat-ended` 通过 | 是 |
| `logs/0.113.1-20261118-192930.stdout.log` / `.stderr.log` / `.summary.txt` | 2026-11-18 19:30 | 否 | 118.4 / 78.7 / 0.5 | 同上 | 是 |

## 测试结果

| 检查 | 方法 | 结果 | 证据 |
|---|---|---|---|
| 0.113.1 启动与补丁应用 | 无头启动 v1.1.0 | 通过：`BigPortrait patch applied`、`AfterDeath hook applied` | `logs/0.113.1-20261118-192930.engine.log:1305-1307` |
| 0.113.1 `BigPortrait` | 探针读取 `Outbreak.BigPortrait` | 通过：`valid=True type=CompressedTexture2D` | `logs/0.113.1-20261118-192930.engine.log:1968` |
| 0.113.1 1 层尸爆伤害 | 3 只 Corpse Slug，给 25 HP 的 1 号挂 1 层，杀死 1 号 | 通过：存活 26/27 HP 的蛞蝓分别剩 1/2，各损失 25 | `logs/0.113.1-20261118-191748.engine.log:2215-2220` |
| 0.113.1 2 层尸爆致死 | 同上，挂 2 层 | 通过：剩余敌人全灭，战斗结束，`ce_check combat-ended` 通过 | `logs/0.113.1-20261118-192930.engine.log:2004-2010` |
| 0.111.1 编译 | `dotnet build -p:Sts2DataDir=TMP/1.1.0/compile-111.1` | 通过：0 警告 0 错误；引用缓存指向 0.111.1 `sts2.dll` | `TMP/1.1.0/build-main-111.1.log`、`TMP/1.1.0/compile-111.1-reference.txt` |
| 0.111.1 实机 | 用户切正式版 | 未执行 | 需用户按 AC-004 回归 |
| 0.113.1 图鉴大图 UI | 游戏内打开/翻页/升级勾选 | 未执行 | 无头场景不覆盖 UI 画面 |

## 测试约定

- 探针与场景只用于本地验证，不进入发布包；测试完成后已从游戏 `mods/` 移除。
- 每次运行单独保存完整日志，不覆盖旧日志；失败时先保存日志再重跑。
- 0.113.1 可在本机无头运行；0.111.1 只有留存程序集，运行验证由用户切分支完成。
- 2026-11-18 19:28 的一次尝试在进入战斗前中断，日志显示当时 Steam 创意工坊正在更新其他 mod；同一场景在 19:30 重跑通过。失败日志保留，不作为本 mod 的失败证据。
