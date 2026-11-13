# 三版本兼容修复：死亡钩子与 `Damage` 重载

- 日期：2026-11-13
- 状态：已完成（构建、部署、0.113 启动验证；实机效果待用户验收）
- 目标版本：v1.0.1
- 关联：`3.DOCS/004-R-v0.113失效评估.md`
- 说明：文件名沿用首版计划；内容已扩展到死亡钩子，二者必须一起修。

## 背景

正式版当前为 0.107.x，官方计划把正式版内容切换到原 beta 0.111.x，beta 线已到 0.113.x。manifest `min_game_version = 0.110.0`：0.107 由加载器拒绝，0.110 / 0.111 / 0.113 均可加载。0.110 / 0.111 / 0.113 的 `Outbreak` 都是 Skill / 全体 / 3 费，形状守卫通过。

## 版本事实

### 死亡钩子

| 成员 | 0.107 / 0.110 / 0.111 | 0.113 | 证据 |
|---|---|---|---|
| `AbstractModel.AfterDeath` | `(PlayerChoiceContext, Creature, bool, float)` | `(PlayerChoiceContext, Creature, CardModel?, bool, float)` | 111 `AbstractModel.cs:668`；113 `:709` |
| `Hook.AfterDeath` | 4 参分发 | 增加 `source` 参数 | 111 `Hook.cs:462`；113 `:501` |
| 分发方式 | `model.AfterDeath(...)` 虚调用 | 同 | 上述两处 |

`Hook.AfterDeath` 在 `RemoveAllPowersAfterDeath()` 之前执行，Power 仍挂在 `Owner` 上，`Amount` / `Owner.MaxHp` 可读；`wasRemovalPrevented` 的语义三版本一致。

### `CreatureCmd.Damage`

| 版本 | 多目标 `decimal` 重载 | 证据 |
|---|---|---|
| 0.107 | 5 参 `(ctx, IEnumerable<Creature>, decimal, ValueProp, Creature)` → 转发 6 参 | `CreatureCmd.cs:141` |
| 0.110 / 0.111 | 5 参 → 转发 7 参 | `:147` |
| 0.113 | 5 参 → 转发 7 参 → 转发 8 参（默认 `ModifyDamageHookType.All`） | `:148`；`:263`；`:285` |

5 参重载在三个版本都存在，语义固定为 `dealer` + `cardSource = null` + `cardPlay = null`，正是本 mod 需要的调用（dealer 传 `null`：方法体没有空检查，可直接传）。

## 问题

### 问题 1：0.113 上死亡钩子完全失效（主因）

`CorpseExplosionPower.AfterDeath` 按 0.111 的 4 参签名声明 `override`。0.113 的基类方法变成 5 参后，该成员不再构成 override。

已验证（最小 .NET 9 复现）：孤立 override 不会引发 `TypeLoadException`，类型可正常加载；虚调用直接走基类空实现。因此 0.113 上尸爆标记仍在、毒伤正常，但目标死亡时**爆炸不会被触发**，也不会走 `Damage` 反射，原报告中的 `NullReferenceException` 不会出现。

### 问题 2：`Damage` 重载查找在 0.113 失效

`CorpseExplosionPower.cs:31-40` 用精确类型数组查 7 参与 6 参重载；0.113 只有 8 参版本，两次查找均返回 `null`。若钩子修好而此处不修，会在 `DamageSixArg!.Invoke` 处抛 `NullReferenceException`。

## 方案

### 修复 1：用 Harmony 替身承接死亡钩子

1. 删除 `CorpseExplosionPower.AfterDeath` 的 `override`，把方法体原样移到 `internal async Task ExplodeOnOwnerDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented)`，保留「死亡被阻止 / 非本人」判定。
2. 新增 `Patches/CorpseExplosionDeathHookPatch.cs`：

```csharp
[HarmonyPatch(typeof(AbstractModel), "AfterDeath")]
public static class CorpseExplosionDeathHookPatch
{
    private static bool Prefix(AbstractModel __instance, PlayerChoiceContext choiceContext,
        Creature creature, bool wasRemovalPrevented, ref Task __result)
    {
        if (__instance is not CorpseExplosionPower power) return true;
        __result = power.ExplodeOnOwnerDeath(choiceContext, creature, wasRemovalPrevented);
        return false;
    }
}
```

3. `__instance` 必须声明为目标方法的声明类型 `AbstractModel`，不能写 `PowerModel`。
4. 参数只取三版本都存在的名字（`choiceContext`、`creature`、`wasRemovalPrevented`），Harmony 按名绑定，因此同一 Prefix 可挂 4 参与 5 参两种目标；`source` 与 `deathAnimLength` 省略即可。
5. 已用最小 Harmony 实验验证：4 参目标、5 参目标都能命中同一 Prefix；未 override 的派生类型走基类 Prefix，override 的派生类型不被拦截。
6. 补丁类用 `public static`，与现有 `Patches/*` 一致。
7. `ModEntry.ModLoaded` 的 `PatchAll` 成功日志后追加一条 `Log.Info($"{ModId}: AbstractModel.AfterDeath hook applied")`；若未来 `AfterDeath` 找不到，`PatchAll` 会抛 `HarmonyException`，外层 `catch` 记 ERROR，日志可区分。

### 修复 2：`Damage` 收敛为单一 5 参查找

1. 只保留一个字段：

```csharp
private static readonly MethodInfo? DamageFiveArg = typeof(CreatureCmd).GetMethod(
    "Damage",
    new[]
    {
        typeof(PlayerChoiceContext), typeof(IEnumerable<Creature>), typeof(decimal),
        typeof(ValueProp), typeof(Creature),
    });
```

2. `ApplyDamage` 调用该重载：实参 `choiceContext`、`targets`、`(decimal)damage`、`ValueProp.Unpowered`、`null`（dealer）。
3. 解析失败或 `Invoke` / `await` 抛异常时 `Log.Error` 一次并跳过本次伤害，异常不得冒泡到死亡结算链。
4. 删除 `DamageSevenArg` / `DamageSixArg` 与旧分派分支；补 `using MegaCrit.Sts2.Core.Logging;`。
5. 不再需要 `ParameterInfo.DefaultValue`、`ModifyDamageHookType` 或「取参数最多者」逻辑；`IEnumerable<DamageTargetData>` 重载也不会被命中。

## 改动清单

| 文件 | 改动 |
|---|---|
| `9.SRC/CorpseExplosionReplacementMod/CorpseExplosionPower.cs` | 删除 `AfterDeath` override；方法体改为 `ExplodeOnOwnerDeath`；`Damage` 收敛为 5 参查找 + 异常兜底 |
| `9.SRC/CorpseExplosionReplacementMod/Patches/CorpseExplosionDeathHookPatch.cs` | 新增死亡钩子 Prefix |
| `9.SRC/CorpseExplosionReplacementMod/ModEntry.cs` | 追加 hook 应用日志 |
| `9.SRC/CorpseExplosionReplacementMod/CorpseExplosionReplacementMod.json` | 版本号 1.0.0 → 1.0.1；`min_game_version` 维持 0.110.0 |
| `5.ChangeLog/`、`CHANGELOG.md` | 记录 v1.0.1 |
| `3.DOCS/004-R-v0.113失效评估.md` | 补「0.113 实际为钩子失效」的修正说明 |

## 不做

- 不把 `min_game_version` 降到 0.107.0：0.107 的 `Outbreak` 是 Power / 自身 / 1 费，套用替换会破坏原卡。
- 不为 0.107 另做一套替换逻辑。
- 不改数值（施毒 6/9、爆炸倍率 1）、卡图与本地化。
- 不新增对 0.113 独有成员的直接引用。

## 验收

**前置：确认实际加载的是新构建。** 本地 `mods/CorpseExplosionReplacementMod` 与创意工坊 `3780352678` 同时存在，设置里通常只启用其一。验收前检查日志中 `Loading assembly DLL ...CorpseExplosionReplacementMod.dll` 的路径，确认命中新构建。

| 版本 | 检查 | 通过标准 |
|---|---|---|
| 0.113 | 施放尸爆术并击杀标记目标 | 爆炸对全体敌人造成最大生命值伤害，可被格挡；日志无异常；出现 hook 应用日志 |
| 0.111 | 同上 | 行为与 v1.0.0 一致 |
| 0.110 | 同上 | 同上（manifest 允许 0.110，反编译显示形状一致） |
| 0.107 | 启动 | 加载器按 `min_game_version` 拒绝加载，日志有明确拒绝记录 |

## 验证方法

1. 静态：确认三版本都存在 5 参 `Damage` 且转发链最终带 `dealer`、`cardSource = null`、`cardPlay = null`；确认 `AbstractModel.AfterDeath` 三版本均存在且 Prefix 参数名都能找到。
2. 构建：以当前 0.113 `sts2.dll` 为基线，0 警告 0 错误。
3. 运行：0.113 实机击杀触发；0.111 / 0.110 无本地环境，标记未验证；0.107 仅验证加载器行为。

## 风险与回退

- 基类 Prefix 会对每个死亡事件的每个监听模型求值一次，`is` 判定开销可忽略；若未来其它 mod 也用 Prefix 抢占同一方法并返回 `false`，本补丁可能不执行，需看 hook 日志。
- 已确认没有已装 mod 直接 Patch `AbstractModel.AfterDeath`；`Loadout` 等只补具体 Power 的 override，不冲突。
- `PatchAll` 在任一补丁类抛异常时会中断后续补丁类；因此新增 hook 的日志是验收信号之一。
- 回退为 v1.0.0 源码，0.111 仍可用；0.113 不可用。

## 已确认

1. 0.107 由 `min_game_version = 0.110.0` 拒绝加载；不在创意工坊简介补充说明（2026-11-13 用户决定）。
2. `Damage` 走 5 参重载，`ModifyDamageHookType` 使用游戏默认值（0.113 为 `All`），不硬编码。

## 施工记录回填

| 时间 | 实际施工内容 | 验收状态 |
|---|---|---|
| 2026-11-13 09:17 | 新增 `CorpseExplosionDeathHookPatch`；`CorpseExplosionPower` 改为 `ExplodeOnOwnerDeath`；`Damage` 收敛为 5 参查找并加异常兜底；manifest 1.0.0 → 1.0.1；构建覆盖安装到 `mods/CorpseExplosionReplacementMod` | 构建通过；0.113 启动日志出现 hook 应用记录，无异常；实机待用户验收 |
