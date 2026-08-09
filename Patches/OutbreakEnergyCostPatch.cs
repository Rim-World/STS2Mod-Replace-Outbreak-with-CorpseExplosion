using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace CorpseExplosionReplacementMod.Patches;

/// <summary>
/// 费用替换 3→2。
/// 本作启动顺序为“先加载 mod（ModLoaded）→ 后执行 ModelDb.Init”，所有卡牌规范实例
/// 都在补丁之后构造；CardModel.EnergyCost 懒加载时读取 CanonicalEnergyCost，
/// 因此补丁该 getter 即可让规范实例与后续克隆都以 2 费创建。
/// CardEnergyCost.Canonical 再兜底一层，保证任何既有对象读取 canonical 也是 2。
/// 注意：不能对规范实例调用 SetCustomBaseCost（AssertMutable 会抛异常），
/// 因此不采用“已缓存实例改写基础费用”的方案。
/// </summary>
[HarmonyPatch(typeof(CardModel), "CanonicalEnergyCost", MethodType.Getter)]
public static class OutbreakEnergyCostPatch
{
    private static readonly FieldInfo? EnergyCostField =
        AccessTools.Field(typeof(CardModel), "_energyCost");

    private static void Postfix(CardModel __instance, ref int __result)
    {
        if (__instance is Outbreak && ModConfig.IsReplaceOutbreakEnabled)
        {
            __result = 2;
        }
    }

    /// <summary>
    /// RitsuLib 实时开关切换后调用：清空 canonical Outbreak 的费用缓存，
    /// 让下一次 EnergyCost 访问按当前开关重建费用（开→2，关→原版费用）。
    /// 仅作用于图鉴/后续新实例；已实例化的手牌卡需重新创建才变价。
    /// </summary>
    public static void ResetOutbreakCostCaches()
    {
        try
        {
            if (EnergyCostField == null)
            {
                Log.Warn($"{ModEntry.ModId}: cannot find CardModel._energyCost for cache reset");
                return;
            }

            foreach (CardModel card in ModelDb.AllCards)
            {
                if (card is Outbreak)
                {
                    EnergyCostField.SetValue(card, null);
                }
            }
        }
        catch (Exception e)
        {
            Log.Warn($"{ModEntry.ModId}: failed to reset Outbreak cost caches: {e.Message}");
        }
    }
}

[HarmonyPatch(typeof(CardEnergyCost), "Canonical", MethodType.Getter)]
public static class OutbreakCardEnergyCostCanonicalPatch
{
    private static readonly FieldInfo CardField =
        AccessTools.Field(typeof(CardEnergyCost), "_card");

    private static void Postfix(CardEnergyCost __instance, ref int __result)
    {
        if (CardField.GetValue(__instance) is Outbreak && ModConfig.IsReplaceOutbreakEnabled)
        {
            __result = 2;
        }
    }
}
