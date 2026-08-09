using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace CorpseExplosionReplacementMod.Patches;

/// <summary>
/// 原版 Outbreak.OnPlay：对全体施毒 9（升级 12）并逐个立即触发毒。
/// 替换为 Corpse Explosion：对单体目标施毒 6（升级 9）并施加“尸爆”标记（倍率 1），不立即触发毒。
/// </summary>
[HarmonyPatch(typeof(Outbreak), "OnPlay")]
public static class OutbreakOnPlayPatch
{
    private static bool Prefix(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay,
        Outbreak __instance,
        ref Task __result)
    {
        if (!ModConfig.IsReplaceOutbreakEnabled)
        {
            return true;
        }

        __result = CorpseExplosionOnPlay(choiceContext, cardPlay, __instance);
        return false;
    }

    private static async Task CorpseExplosionOnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay,
        Outbreak instance)
    {
        await CreatureCmd.TriggerAnim(instance.Owner.Creature, "Cast", instance.Owner.Character.CastAnimDelay);
        if (cardPlay.Target == null)
        {
            return;
        }

        NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NPoisonImpactVfx.Create(cardPlay.Target));

        await PowerCmd.Apply<PoisonPower>(
            choiceContext,
            cardPlay.Target,
            instance.DynamicVars.Poison.BaseValue,
            instance.Owner.Creature,
            instance);

        await PowerCmd.Apply<CorpseExplosionPower>(
            choiceContext,
            cardPlay.Target,
            instance.DynamicVars["CorpseExplosionPower"].BaseValue,
            instance.Owner.Creature,
            instance);
    }
}
