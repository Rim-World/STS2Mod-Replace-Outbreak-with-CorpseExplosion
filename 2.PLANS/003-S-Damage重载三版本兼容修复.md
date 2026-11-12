# 三版本兼容修复：`CreatureCmd.Damage` 多重载反射分派

- 日期：2026-11-13
- 状态：待实施
- 目标版本：v1.0.1
- 关联：`3.DOCS/004-R-v0.113失效评估.md`

## 背景

正式版当前为 0.107.x，官方计划把正式版内容切换到原 beta 0.111.x，beta 线已到 0.113.x。本 mod 在 0.111 与 0.113 上是「整卡替换」生效版本；0.107 的 `Outbreak` 是另一张卡（Power / 自身 / 1 费），形状守卫会关闭全部补丁，manifest 的 `min_game_version = 0.110.0` 也会让 0.107 直接不加载。修复目标是：0.111 与 0.113 都能正常触发尸爆伤害，0.107 保持「不加载或不生效、不报错」。

## 版本事实

`CreatureCmd.Damage` 的多目标 + `decimal` 重载：

| 游戏版本 | 参数 | 证据 |
|---|---|---|
| 0.107 | `(ctx, IEnumerable<Creature>, decimal, ValueProp, Creature?, CardModel?)` | `STS2-Reference/1.STS2-107Reference/decompiled/MegaCrit.Sts2.Core.Commands/CreatureCmd.cs:240` |
| 0.111 | 同上 + `CardPlay?`（7 参） | `STS2-Reference/2.STS2-111Reference/decompiled/MegaCrit.Sts2.Core.Commands/CreatureCmd.cs:258` |
| 0.113 | 同上 7 参 + `ModifyDamageHookType modifyDamageHookType = All`（8 参） | `STS2-Reference/3.STS2-113Reference/decompiled/MegaCrit.Sts2.Core.Commands/CreatureCmd.cs:263` |

0.113 还新增了第二组 8 参重载（第 2 参为 `IEnumerable<DamageTargetData>`，`:285`），按第 2 参类型可区分。

## 问题

`9.SRC/CorpseExplosionReplacementMod/CorpseExplosionPower.cs:31-40` 用精确类型数组查两个固定重载：7 参（110/111）与 6 参（107）。0.113 把 7 参换成 8 参，两次查找都返回 `null`；`ApplyDamage` 走到 `DamageSixArg!.Invoke`（`:94`），抛 `NullReferenceException`，爆炸不掉血。

## 方案

按「多目标 + `decimal`」特征扫描重载，按实际参数个数构造实参。

1. 新增静态解析方法，在 `typeof(CreatureCmd).GetMethods()` 中筛选：
   - 方法名 `Damage`；
   - 参数 1 为 `PlayerChoiceContext`；
   - 参数 2 的类型等于 `typeof(IEnumerable<Creature>)`；
   - 参数 3 为 `decimal`；
   - 参数 4 为 `ValueProp`。
   在命中项中取参数最多者（8 → 7 → 6），缓存 `MethodInfo`。
2. `ApplyDamage` 按 `MethodInfo.GetParameters()` 构造 `object?[]`：
   - 依次为 `choiceContext`、`targets`、`(decimal)damage`、`ValueProp.Unpowered`、`null`（dealer 必须为 null，保持现有语义）、`null`（cardSource）；
   - 存在 `CardPlay?` 参数时补 `null`；
   - 余下带默认值的参数填 `ParameterInfo.DefaultValue`（0.113 的 `ModifyDamageHookType.All`）。
3. 解析失败时记录一次 `Log.Error`，本次不造成伤害，不再走空引用路径。
4. 删除 `DamageSevenArg` / `DamageSixArg` 两个固定字段与旧分派分支。
5. 形状守卫与其它补丁不动。

## 改动清单

| 文件 | 改动 |
|---|---|
| `9.SRC/CorpseExplosionReplacementMod/CorpseExplosionPower.cs` | 重写重载解析与 `ApplyDamage` 参数构造 |
| `9.SRC/CorpseExplosionReplacementMod/CorpseExplosionReplacementMod.json` | 版本号 1.0.0 → 1.0.1；`min_game_version` 维持 0.110.0 |
| `5.ChangeLog/`、`CHANGELOG.md` | 记录 v1.0.1 |
| `3.DOCS/` | 追加修复记录（可并入 004 报告末尾） |

## 不做

- 不把 `min_game_version` 降到 0.107.0：0.107 的 `Outbreak` 是 Power / 自身 / 1 费，套用尸爆替换会破坏原卡。
- 不为 0.107 单独制作第二套替换逻辑。
- 不改数值（施毒 6/9、爆炸倍率 1）与本地化。

## 验收

| 版本 | 检查 | 通过标准 |
|---|---|---|
| 0.113 | 对敌人施放尸爆术并击杀 | 爆炸对全体敌人造成最大生命值伤害，可被格挡；日志无 `NullReferenceException` |
| 0.111 | 同上 | 行为与 v1.0.0 一致 |
| 0.107 | 启动 | 加载器按 `min_game_version` 拒绝加载，日志有明确拒绝记录，游戏正常 |

## 验证方法

1. 静态：对照三份反编译源码确认特征筛选在三个版本都能命中唯一重载（107 命中 6 参，111 命中 7 参，113 命中 8 参）。
2. 构建：以当前 0.113 `sts2.dll` 为基线，0 警告 0 错误。
3. 运行：0.113 实机击杀触发；0.111 无本地环境，标记未验证；0.107 仅验证加载器行为。

## 风险与回退

- 反射调用依赖参数默认值；若目标方法省略默认值参数，`DefaultValue` 可能为 `Missing`，需按枚举零值兜底。
- 回退为 v1.0.0 源码，0.111 仍可用。

## 待确认

1. 0.107 用户是否会安装本 mod；当前设计为不加载，需在创意工坊简介中写明。
2. 是否需要为 0.113 的 `ModifyDamageHookType` 显式传 `All`，而不是依赖默认值（两者等效，默认值更省事）。

## 施工记录回填

| 时间 | 实际施工内容 | 验收状态 |
|---|---|---|
| — | — | — |
