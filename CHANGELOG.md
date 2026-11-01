# Changelog

本文件遵循 [Keep a Changelog](https://keepachangelog.com/zh-CN/1.1.0/) 规范，版本号遵循 [Semantic Versioning](https://semver.org/lang/zh-CN/)。

分版本的详细记录（含开发反馈、技术细节、失败经验、提交编号）见 `5.ChangeLog/`。

## [未发布]

### 变更

- 仓库整理为 AGENTS 十目录结构：源码移入 `9.SRC/`，文档按 ADR / PLANS / DOCS / SDD / ChangeLog 分类，构建产物与发布包归入 `10.OUTPUT/`。
- 补全 `README.md`、`CHANGELOG.md`、`4.SDD/001-总体设计.md`、各目录索引与 `.gitignore`。
- 恢复远端仓库根目录的 `README.md`（移入 `9.SRC/CorpseExplosionReplacementMod/README.md`）与 `LICENSE`。
- 无代码改动。

## [1.0.0] - 2026-08-21

### 修复

- 图鉴中打开「尸爆术」大图时卡牌不可见（只剩暗色遮罩）：传递到 hover tip 的 pck 纹理被游戏 v0.111 的资源清理机制 Dispose，`NHoverTipSet.Init` 抛 `ObjectDisposedException` 并中断大图刷新。
- 战斗中尸爆标记的 Buff 图标异常（同一根因）。

### 变更

- mod 清单版本号 `1.0` → `1.0.0`，消除「不是合法语义化版本」的启动警告。
- 卡图与 Buff 图标加载器改为「有效性自检 + pck 重载 + DLL 内嵌 PNG 兜底」。
- 新增图鉴大图守护补丁，异常时降级为只渲染卡牌本体。

## [1.0] - 2026-08-09

### 新增

- 首个公开版本：`Outbreak` → 「尸爆术」整卡替换，含 15 语言文本、STS1 卡图与 Buff 图标。
- 自定义「尸爆」Debuff：目标死亡时对全体敌人造成最大生命值 × 倍率的普通伤害。
- 中英文 README、MIT 许可证、源码与素材入库。

### 修复

- 死亡爆炸只播特效不掉血：伤害来源改传 `null`（0.1.1）。
- 启动期 `DuplicateModelException`：移除多余的 `ModelDb.Inject`（0.1.0 内修复）。

### 已知问题

- `version` 字段写为 `1.0`，不是合法语义化版本（1.0.0 修复）。
- v0.111 下图鉴大图与 Buff 图标异常（1.0.0 修复）。

## [0.2.2] - 2026-08-08

### 变更

- `CreatureCmd.Damage` 改为运行时反射分派（7 参优先、6 参兜底），撤销条件编译，单一 DLL 兼容 107/110。

## [0.2.1] - 2026-08-08

### 变更

- 移除 `PlatformUtil.GetPlatformBranch()` 分支判断，门槛只保留 `min_game_version` 与卡牌形态守卫。

## [0.2.0] - 2026-08-07

### 变更

- 本地化键统一加 `RE_` 前缀，避免与其它 mod 的通用键冲突。
- 移除 RitsuLib 互操作与设置页，替换功能改为常开，保留形态守卫。

## [0.1.2] - 2026-08-07

### 新增

- 卡牌形态守卫 `OutbreakShapeGuard`（技能牌 / 全体目标 / 基础费用 3）。
- `min_game_version` 提升到 `0.110.0`。

## [0.1.1] - 2026-08-06

### 修复

- 尸爆伤害不结算：`CreatureCmd.Damage` 的 dealer 由已死亡的 Owner 改为 `null`。

## [0.1.0] - 2026-08-06

### 新增

- 首次实现：费用 3→2、目标全体→单体、施毒 9→6、新增尸爆标记、`OnPlay` 重写、卡图/文本/Buff 图标替换、15 语言本地化。
