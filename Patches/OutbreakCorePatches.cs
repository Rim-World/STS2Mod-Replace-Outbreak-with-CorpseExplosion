using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;

namespace CorpseExplosionReplacementMod.Patches;

/// <summary>
/// Outbreak 核心属性替换：目标全体→单体（AnyEnemy）、
/// 核心变量 毒9/升级12 → 毒6/升级9 + 尸爆标记1、HoverTip 毒 → 尸爆。
/// 卡牌类型仍是 Skill、稀有度仍是 Rare，无需改。
/// </summary>
[HarmonyPatch(typeof(CardModel), "TargetType", MethodType.Getter)]
public static class OutbreakTargetTypePatch
{
    private static void Postfix(CardModel __instance, ref TargetType __result)
    {
        if (__instance is Outbreak && ModConfig.IsReplaceOutbreakEnabled)
        {
            __result = TargetType.AnyEnemy;
        }
    }
}

[HarmonyPatch(typeof(Outbreak), "CanonicalVars", MethodType.Getter)]
public static class OutbreakCanonicalVarsPatch
{
    private static void Postfix(Outbreak __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (ModConfig.IsReplaceOutbreakEnabled)
        {
            __result = new DynamicVar[]
            {
                new PowerVar<PoisonPower>(6m),
                new PowerVar<CorpseExplosionPower>(1m),
            };
        }
    }
}

[HarmonyPatch(typeof(Outbreak), "ExtraHoverTips", MethodType.Getter)]
public static class OutbreakExtraHoverTipsPatch
{
    private static void Postfix(Outbreak __instance, ref IEnumerable<IHoverTip> __result)
    {
        if (ModConfig.IsReplaceOutbreakEnabled)
        {
            __result = new IHoverTip[] { HoverTipFactory.FromPower<CorpseExplosionPower>() };
        }
    }
}
