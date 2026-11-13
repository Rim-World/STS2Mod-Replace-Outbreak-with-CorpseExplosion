using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;

namespace CorpseExplosionReplacementMod;

/// <summary>
/// STS1 Corpse Explosion Power（尸爆术标记）。
/// Amount 为爆炸倍率（本卡固定 1，可因重复施加/复制叠加）。
/// 目标死亡时（AfterDeath，且死亡未被阻止）对全体存活敌人造成 最大生命值 × Amount 的普通伤害
/// （可被格挡、不吃力量加成，与 STS1 DamageInfo.DamageType 一致）。
/// </summary>
public sealed class CorpseExplosionPower : PowerModel
{
    /// <summary>
    /// 0.107 / 0.110 / 0.111 / 0.113 都存在 5 参多目标重载：
    /// (ctx, IEnumerable&lt;Creature&gt;, decimal, ValueProp, Creature)。
    /// 后续版本在内部转发到更多参数的重载，本 mod 不直接引用那些版本细节。
    /// </summary>
    private static readonly MethodInfo? DamageFiveArg = typeof(CreatureCmd).GetMethod("Damage", new[]
    {
        typeof(PlayerChoiceContext), typeof(IEnumerable<Creature>), typeof(decimal),
        typeof(ValueProp), typeof(Creature),
    });

    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;

    /// <summary>
    /// 由 <see cref="Patches.CorpseExplosionDeathHookPatch"/> 调用。
    /// 0.113 的 AbstractModel.AfterDeath 增加了 CardModel? source 参数，
    /// 这里不再用 override，避免基类签名变化导致钩子静默失效。
    /// </summary>
    internal async Task ExplodeOnOwnerDeath(
        PlayerChoiceContext choiceContext,
        Creature creature,
        bool wasRemovalPrevented)
    {
        // 只响应自身持有者死亡；死亡被阻止（如 Fairy in a Bottle）时不触发爆炸。
        if (wasRemovalPrevented || creature != Owner)
        {
            return;
        }

        var targets = Owner.CombatState?.HittableEnemies
            .Where(e => e != Owner && e.IsAlive)
            .ToList();
        if (targets == null || targets.Count == 0)
        {
            return;
        }

        int damage = Owner.MaxHp * Amount;
        foreach (Creature target in targets)
        {
            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NFireBurstVfx.Create(target, 0.75f));
        }

        // dealer 必须为 null：CreatureCmd.Damage 会直接跳过“伤害来源已死亡”的结算
        // （返回空结果、不掉血），而死亡怪物本身正是尸爆的触发者。
        // 与毒伤等环境伤害一致，dealer 传 null，普通伤害、可格挡、不吃力量。
        await ApplyDamage(choiceContext, targets, damage);
    }

    private static async Task ApplyDamage(
        PlayerChoiceContext choiceContext,
        IEnumerable<Creature> targets,
        int damage)
    {
        if (DamageFiveArg == null)
        {
            Log.Error($"{ModEntry.ModId}: CreatureCmd.Damage 5-arg overload not found; corpse explosion skipped.");
            return;
        }

        try
        {
            var task = (Task<IEnumerable<DamageResult>>)DamageFiveArg.Invoke(null, new object?[]
            {
                choiceContext, targets, (decimal)damage, ValueProp.Unpowered, null,
            })!;
            await task;
        }
        catch (Exception e)
        {
            Log.Error($"{ModEntry.ModId}: corpse explosion damage failed: {e}");
        }
    }
}
