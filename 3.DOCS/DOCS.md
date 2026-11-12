# 知识文档索引

本目录存放可复用的知识与报告：任务执行中沉淀的结论、失败经验，以及用户要求出具的调研与审查报告。工程决策见 `1.ADR/`，施工计划见 `2.PLANS/`，设计描述见 `4.SDD/`。

## 分类约定

- `T`（task）：具体任务执行沉淀的知识与失败经验。
- `R`（report）：用户要求出具的报告（可行性研究、相似项目调研、审查报告等）。
- `E`（evidence）：用户要求研究的技术问题的关键证据陈列与汇总。

文件名格式 `NNN-<分类>-中文标题.md`，编号三位递增、按时间顺序在最前。

## 文档清单

| 编号 | 分类 | 标题 | 日期 | 说明 |
|---|---|---|---|---|
| 001 | T | [图鉴大图空白根因与修复](001-T-图鉴大图空白根因与修复.md) | 2026-08-21 | 游戏 v0.111 的 `AssetCache.UnloadMissedCacheAssets` 会 Dispose 未预缓存资源，毒化 mod pck 纹理，导致图鉴大图 `NHoverTipSet.Init` 抛 `ObjectDisposedException`；记录根因链、修复方案 A/B1 与可复用 API 备忘。原文件 `docs/research-corpse-explosion-codex-fix.md`。 |
| 002 | T | [存档损坏与版本切换研究](002-T-存档损坏与版本切换研究.md) | 2026-08-10 | 110→107 切换后「存档无法读取」的根因是游戏自身拒绝旧版本读取新 schema（v20 → v16 上限），与本 mod 写入无关；含日志证据与四套初步方案。五个拆分子项目各保留一份。原文件 `docs/research-save-corruption-version-switch.md`。 |
| 003 | T | [尸爆术 15 语言译文](003-T-尸爆术15语言译文.md) | 2026-08-06 | 逐语言的 STS1 原文、最终文本与转换说明（`!M!` → 动态变量、`NL` → 换行、关键词加 `[gold]`、`#b` → `[blue]{}[/blue]`），是本地化 JSON 的直接依据。原文件 `translation/corpse-explosion-translation.md`。 |
| 004 | R | [v0.113 失效评估](004-R-v0.113失效评估.md) | 2026-11-13 | v0.113.0 给 `CreatureCmd.Damage` 的 7 参重载追加第 8 个可选参数，mod 的两条反射查找全部落空（`null`），尸爆触发时抛 `NullReferenceException`，爆炸无伤害；含证据行号与修复方向 |
