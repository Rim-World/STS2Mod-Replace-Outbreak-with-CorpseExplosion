using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace CorpseExplosionReplacementMod.Patches;

/// <summary>
/// 0.113 的 AbstractModel.AfterDeath 新增 CardModel? source 参数，派生类按旧签名写的
/// override 不再是 override。这里用基类 Prefix 承接：按参数名绑定，4 参与 5 参目标都能命中。
/// __instance 必须是声明类型 AbstractModel，不能写 PowerModel。
/// </summary>
[HarmonyPatch(typeof(AbstractModel), "AfterDeath")]
public static class CorpseExplosionDeathHookPatch
{
    private static bool Prefix(
        AbstractModel __instance,
        PlayerChoiceContext choiceContext,
        Creature creature,
        bool wasRemovalPrevented,
        ref Task __result)
    {
        if (__instance is not CorpseExplosionPower power)
        {
            return true;
        }

        __result = power.ExplodeOnOwnerDeath(choiceContext, creature, wasRemovalPrevented);
        return false;
    }
}
