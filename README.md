# STS2Mod-尸爆（Corpse Explosion Replacement）

把《杀戮尖塔 2》静默猎手的「毒性爆发」（Outbreak）整卡替换为《杀戮尖塔 1》的「尸爆术」（Corpse Explosion）。

Replaces The Silent's "Outbreak" card in Slay the Spire 2 with "Corpse Explosion" from Slay the Spire 1 — same slot, different card.

- 当前版本：`1.1.0`
- 模组 ID：`CorpseExplosionReplacementMod`
- 支持游戏：`0.111.1`（正式版）、`0.113.1`（beta）
- 最低游戏版本：`0.111.1`
- 开源地址：<https://github.com/Rim-World/STS2Mod-Replace-Outbreak-with-CorpseExplosion>

## 功能

| 维度 | 毒性爆发（原版） | 尸爆术（替换后） |
|---|---|---|
| 费用 | 3 | 2 |
| 目标 | 全体敌人 | 单个敌人 |
| 施毒 | 9（升级 +3） | 6（升级 +3） |
| 附加 | 立即触发毒 | 附加「尸爆」标记（倍率 1） |
| 死亡效果 | 无 | 目标死亡时，对全体敌人造成其最大生命值 × 倍率的伤害（可被格挡、不吃力量） |

同时替换：卡名、卡面文本、卡面图片、Buff 图标、Buff 动态文本，覆盖 15 种语言。

替换方式是**直接修改原 `Outbreak` 卡牌**，而非「生成新卡 + 禁用旧卡」。因此 mod 开启时历史记录中的「毒性爆发」都会以尸爆术形态显示，关闭后恢复原状。

## 兼容性

| 游戏版本 | 状态 | 说明 |
|---|---|---|
| `0.113.1`（beta） | 支持 | 死亡钩子、5 参 `Damage`、`CardModel.BigPortrait` 均已适配 |
| `0.111.1`（正式版） | 支持 | 使用同一份 DLL/PCK；实机回归待用户 |
| `0.111.0` 及更旧 | 不支持 | `min_game_version = 0.111.1` 由游戏拒绝加载 |
| `0.114` 及更新 | 未承诺 | 版本变化后需重新检查 API |

- 不依赖任何基础 mod，`dependencies` 为空。
- 启动时通过三道检查：manifest 的 `min_game_version`、运行期的卡牌形态守卫（技能牌 / 全体目标 / 基础费用 3）、可选 `BigPortrait` 补丁探测。
- 任一检查不满足时，对应补丁失效或整体退回原版行为。
- 不保证与盗版、修改版、移动版、冷门或次级 mod 的兼容性。
- 游戏新增的 `ind`、`ara` 语言没有 mod 译文，按游戏规则回退英文。

兼容性检查的 API 证据与测试结论见 [`3.DOCS/005-R-0.111.1与0.113.1兼容性检查.md`](3.DOCS/005-R-0.111.1与0.113.1兼容性检查.md)。

## 安装

1. 确认游戏为 `0.111.1` 或 `0.113.1`。
2. 把 `CorpseExplosionReplacementMod.dll`、`CorpseExplosionReplacementMod.pck`、`CorpseExplosionReplacementMod.json`（`.pdb` 可选）放进：
   `Slay the Spire 2/mods/CorpseExplosionReplacementMod/`
3. 启动游戏，在 mod 列表中确认本模组已启用。

发布包见 [`10.OUTPUT/OUTPUT.md`](10.OUTPUT/OUTPUT.md) 与 GitHub Releases。

## 构建

前置：.NET 9 SDK、Godot 4.5.1 mono、本机已安装 Slay the Spire 2。

```powershell
cd 9.SRC\CorpseExplosionReplacementMod
.\build.ps1
```

脚本会执行 `dotnet build` → Godot `--import` → `--export-pack` → 复制产物到游戏 mods 目录。脚本内的游戏路径与 Godot 路径为绝对路径，换机需先修改。

## 目录结构

| 目录 | 内容 |
|---|---|
| `1.PRD/` | 需求与验收，索引 `PRD.md` |
| `2.PLANS/` | 施工计划，索引 `PLANS.md` |
| `3.DOCS/` | 知识与调研报告，索引 `DOCS.md` |
| `4.ADR/` | 架构决策记录，索引 `ADR.md` |
| `5.SDD/` | 软件设计文档，索引 `SDD.md` |
| `6.SPEC/` | 对外规格，索引 `SPEC.md` |
| `7.ChangeLog/` | 分版本日志，索引 `ChangeLog.md` |
| `8.TEST/` | 测试脚本、探针与日志，索引 `TEST.md` |
| `9.SRC/` | 模组源码（Godot 工程 + C# 项目） |
| `10.OUTPUT/` | 构建产物与发布包，索引 `OUTPUT.md`（内容不入库） |
| `PRIVACY/` | 机密信息说明，索引 `PRIVACY.md`（永不入库） |
| `TMP/` | 临时与过程文件，索引 `TMP.md`（内容不入库） |
| `TOOLS/` | 工具与脚本，索引 `TOOLS.md` |
| `description/` | 创意工坊/发布文案（中英文） |
| `pics/` | 封面与素材图 |

## 文档索引

- 需求与验收：[`1.PRD/001-项目范围与验收.md`](1.PRD/001-项目范围与验收.md)
- 项目全貌与设计：[`5.SDD/001-总体设计.md`](5.SDD/001-总体设计.md)
- 立项决策与取舍：[`4.ADR/001-尸爆术替换mod立项.md`](4.ADR/001-尸爆术替换mod立项.md)
- 双版本兼容决策：[`4.ADR/002-兼容基线定为0.111.1与0.113.1.md`](4.ADR/002-兼容基线定为0.111.1与0.113.1.md)
- 双版本兼容检查：[`3.DOCS/005-R-0.111.1与0.113.1兼容性检查.md`](3.DOCS/005-R-0.111.1与0.113.1兼容性检查.md)
- 图鉴大图空白根因：[`3.DOCS/001-T-图鉴大图空白根因与修复.md`](3.DOCS/001-T-图鉴大图空白根因与修复.md)
- 15 语言译文与转换规则：[`3.DOCS/003-T-尸爆术15语言译文.md`](3.DOCS/003-T-尸爆术15语言译文.md)
- 版本日志：[`7.ChangeLog/003-1.1.0-2026-11-18.md`](7.ChangeLog/003-1.1.0-2026-11-18.md)、[`CHANGELOG.md`](CHANGELOG.md)

## 版本

当前 `1.1.0`（2026-11-18）。历史演进见 `CHANGELOG.md` 与 `7.ChangeLog/`。

## 致谢

- 本模组使用 Codex（桌面版）/ Deepseek Harness（WebUI）+ DeepSeek V4 Flash 完成。
- 感谢《杀戮尖塔 2》制作组为 mod 制作提供的便利。
- 感谢 Baselib 制作者提供的文档。
- 感谢 GitHub 项目 [@jiegec/STS2RevertAnthony](https://github.com/jiegec/STS2RevertAnthony) 提供的源码参考。

## 许可

MIT，见 [`LICENSE`](LICENSE)。

---

## English

Replaces the Silent's "Outbreak" card with "Corpse Explosion" from Slay the Spire 1 for Slay the Spire 2.

- Cost 3 → 2, target all enemies → single enemy, poison 9 → 6 (upgrade +3 keeps), "trigger poison immediately" removed, new "Corpse Explosion" debuff: when the target dies, deal damage equal to its Max HP × stacks to all enemies (blockable, unaffected by Strength).
- Card name, card text, card art, buff icon and buff smart text are replaced in 15 languages.
- The original `Outbreak` model is modified in place; no new card is created.
- Supported game versions: `0.111.1` (stable) and `0.113.1` (beta). `min_game_version` is `0.111.1`; older versions are rejected at load time.
- 0.113.1's `CardModel.BigPortrait` is patched conditionally; 0.111.1 skips that patch because the property does not exist.
- `ind` and `ara` are not translated; the game falls back to English.
- No dependencies. Build with .NET 9 + Godot 4.5.1 mono via `9.SRC/CorpseExplosionReplacementMod/build.ps1`.

MIT licensed.
