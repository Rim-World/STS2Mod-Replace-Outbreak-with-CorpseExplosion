# 测试文件索引

本目录存放开发过程中的测试工具与测试数据。被集成进源码和发布包的单独测试文件不在此登记。

## 目录约定

- 按游戏版本或 mod 版本建立子目录；本轮为 `1.1.0/`。
- `CI-CD/` 存放本地门禁脚本，不随版本清理。
- 每个版本最多登记 10 个小版本；满后新建 `NNN-测试文件登记.md`。
- 含 Steam 账号 ID 与本机路径的完整日志不进入 git，只保留在 `TMP/1.1.0/test-logs/`；`8.TEST/` 只提交脱敏摘录。

## 1.1.0

| 文件名 | 创建时间 | 是否清理 | 文件大小（KB） | 用途 | 引用和复用 |
|---|---|---|---|---|---|
| `CorpseExplosionProbe/CorpseExplosionProbe.csproj` | 2026-11-18 19:03 | 否 | 0.8 | 探针 mod 构建文件；引用 `sts2.dll`、`0Harmony.dll`、`GodotSharp.dll` | 是 |
| `CorpseExplosionProbe/ProbeConsoleCmd.cs` | 2026-11-18 19:21 | 否 | 3.1 | 注册 `ce_probe`，打印 Outbreak 属性、`BigPortrait` 与战斗生物血量/Power | 是 |
| `CorpseExplosionProbe/CheckConsoleCmd.cs` | 2026-11-18 19:19 | 否 | 1.5 | 注册 `ce_check`，断言 `combat-ended` / `combat-active` | 是 |
| `CorpseExplosionProbe/ReleaseModeUnlockPatch.cs` | 2026-11-18 19:03 | 否 | 0.5 | 测试专用：让发行版接受 `--scenario` | 是 |
| `CorpseExplosionProbe/CorpseExplosionProbe.json` | 2026-11-18 19:03 | 否 | 0.3 | 探针 mod 清单，`min_game_version=0.111.1` | 是 |
| `corpse-explosion-compat.scenario` | 2026-11-18 19:03 | 否 | 0.4 | 0.113.1 1 层尸爆场景：3 只 Corpse Slug，挂尸爆、击杀、探测血量 | 是 |
| `corpse-explosion-lethal.scenario` | 2026-11-18 19:19 | 否 | 0.3 | 0.113.1 2 层尸爆场景：断言剩余敌人死亡、战斗结束 | 是 |
| `run-compat.ps1` | 2026-11-18 19:16 | 否 | 3.2 | 检查游戏未运行、部署探针、启动场景、保存日志与退出码 | 是 |
| `logs/0.113.1-compat-evidence.log` | 2026-11-18 19:38 | 否 | 2.5 | 从完整日志提取的脱敏证据：补丁应用、1 层伤害、2 层致死、`BigPortrait` 有效 | 是 |

## 本地完整日志（不入库）

完整日志位于 `TMP/1.1.0/test-logs/`，因为其中含 Steam 账号 ID 与本机绝对路径。文件清单与时间见 `TMP/TMP.md`。保留的轮次：

| 文件 | 结果 |
|---|---|
| `0.113.1-20261118-191748.engine.log` | 1 层尸爆，26/27 HP 的敌人各损失 25，剩 1/2 |
| `0.113.1-20261118-192101.engine.log` | 2 层尸爆，剩余敌人全灭，战斗结束 |
| `0.113.1-20261118-192930.engine.log` | 最终轮：`BigPortrait valid=True`、2 层全灭、`ce_check` 通过 |
| `0.113.1-20261118-192812.engine.log` | 失败尝试：Steam 创意工坊更新其他 mod，进入战斗前中断 |
| `0.113.1-20261118-191933.engine.log` | 探针早期版本，含无关本地化告警 |

## 测试结果

| 检查 | 方法 | 结果 | 证据 |
|---|---|---|---|
| 0.113.1 启动与补丁应用 | 无头启动 v1.1.0 | 通过：`BigPortrait patch applied`、`AfterDeath hook applied` | `logs/0.113.1-compat-evidence.log` |
| 0.113.1 `BigPortrait` | 探针读取 `Outbreak.BigPortrait` | 通过：`valid=True type=CompressedTexture2D` | `logs/0.113.1-compat-evidence.log` |
| 0.113.1 1 层尸爆伤害 | 3 只 Corpse Slug，给 25 HP 的 1 号挂 1 层，杀死 1 号 | 通过：存活 26/27 HP 的蛞蝓分别剩 1/2，各损失 25 | `logs/0.113.1-compat-evidence.log` |
| 0.113.1 2 层尸爆致死 | 同上，挂 2 层 | 通过：剩余敌人全灭，战斗结束，`ce_check combat-ended` 通过 | `logs/0.113.1-compat-evidence.log` |
| 0.111.1 编译 | `dotnet build -p:Sts2DataDir=TMP/1.1.0/compile-111.1` | 通过：0 警告 0 错误；引用缓存指向 0.111.1 `sts2.dll` | `TMP/1.1.0/build-main-111.1.log`、`TMP/1.1.0/compile-111.1-reference.txt` |
| 0.111.1 实机 | 用户切正式版 | 未执行 | 需用户按 AC-004 回归 |
| 0.113.1 图鉴大图 UI | 游戏内打开/翻页/升级勾选 | 未执行 | 无头场景不覆盖 UI 画面 |

## 测试约定

- 探针与场景只用于本地验证，不进入发布包；测试完成后已从游戏 `mods/` 移除。
- 每次运行单独保存完整日志，不覆盖旧日志；失败时先保存日志再重跑。
- 0.113.1 可在本机无头运行；0.111.1 只有留存程序集，运行验证由用户切分支完成。
- 2026-11-18 19:28 的一次尝试在进入战斗前中断，日志显示当时 Steam 创意工坊正在更新其他 mod；同一场景在 19:30 重跑通过。失败日志保留，不作为本 mod 的失败证据。
