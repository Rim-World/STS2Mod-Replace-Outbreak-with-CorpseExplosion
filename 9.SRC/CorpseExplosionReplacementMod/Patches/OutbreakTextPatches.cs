using HarmonyLib;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace CorpseExplosionReplacementMod.Patches;

/// <summary>
/// 文本替换补丁：本地化文件新增 RE_CORPSE_EXPLOSION.title/.description 键
/// （模组专属前缀，避免键冲突），由这里切换到新键。
/// </summary>
[HarmonyPatch(typeof(CardModel), "TitleLocString", MethodType.Getter)]
public static class OutbreakTitleLocStringPatch
{
    private static void Postfix(CardModel __instance, ref LocString __result)
    {
        if (__instance is Outbreak && ModConfig.IsReplaceOutbreakEnabled)
        {
            __result = new LocString("cards", "RE_CORPSE_EXPLOSION.title");
        }
    }
}

[HarmonyPatch(typeof(CardModel), "Description", MethodType.Getter)]
public static class OutbreakDescriptionPatch
{
    private static void Postfix(CardModel __instance, ref LocString __result)
    {
        if (__instance is Outbreak && ModConfig.IsReplaceOutbreakEnabled)
        {
            __result = new LocString("cards", "RE_CORPSE_EXPLOSION.description");
        }
    }
}
