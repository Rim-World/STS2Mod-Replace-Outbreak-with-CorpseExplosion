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
    /// 110+：CreatureCmd.Damage(..., Creature?, CardModel?, CardPlay?) 7 参；
    /// 107：无 CardPlay 的 6 参版本。其余参数一致，因此运行时按实际存在的重载反射分派，
    /// 一个 DLL 即可同时适配 107/110，切版本无需重新构建。
    /// </summary>
    private static readonly MethodInfo? DamageSevenArg = typeof(CreatureCmd).GetMethod("Damage", new[]
    {
        typeof(PlayerChoiceContext), typeof(IEnumerable<Creature>), typeof(decimal),
        typeof(ValueProp), typeof(Creature), typeof(CardModel), typeof(CardPlay),
    });

    private static readonly MethodInfo? DamageSixArg = typeof(CreatureCmd).GetMethod("Damage", new[]
    {
        typeof(PlayerChoiceContext), typeof(IEnumerable<Creature>), typeof(decimal),
        typeof(ValueProp), typeof(Creature), typeof(CardModel),
    });

    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterDeath(
        PlayerChoiceContext choiceContext,
        Creature creature,
        bool wasRemovalPrevented,
        float deathAnimLength)
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
        if (DamageSevenArg != null)
        {
            var task = (Task<IEnumerable<DamageResult>>)DamageSevenArg.Invoke(null, new object?[]
            {
                choiceContext, targets, (decimal)damage, ValueProp.Unpowered, null, null, null,
            })!;
            await task;
            return;
        }

        var task107 = (Task<IEnumerable<DamageResult>>)DamageSixArg!.Invoke(null, new object?[]
        {
            choiceContext, targets, (decimal)damage, ValueProp.Unpowered, null, null,
        })!;
        await task107;
    }
}
