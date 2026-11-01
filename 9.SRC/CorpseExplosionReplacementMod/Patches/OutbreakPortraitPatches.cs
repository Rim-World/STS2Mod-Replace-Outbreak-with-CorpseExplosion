using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace CorpseExplosionReplacementMod.Patches;

/// <summary>
/// 把 Outbreak 的卡图三个来源全部替换为模组内 STS1 尸爆术卡图。
/// </summary>
[HarmonyPatch(typeof(CardModel), "PortraitPngPath", MethodType.Getter)]
public static class OutbreakPortraitPngPathPatch
{
    private static bool Prefix(CardModel __instance, ref string __result)
    {
        if (__instance is not Outbreak || !ModConfig.IsReplaceOutbreakEnabled)
        {
            return true;
        }

        __result = ModEntry.PortraitPng;
        return false;
    }
}

[HarmonyPatch(typeof(CardModel), "Portrait", MethodType.Getter)]
public static class OutbreakPortraitPatch
{
    private static bool Prefix(CardModel __instance, ref Texture2D __result)
    {
        if (__instance is not Outbreak || !ModConfig.IsReplaceOutbreakEnabled)
        {
            return true;
        }

        __result = PortraitTextureLoader.Get();
        return false;
    }
}

[HarmonyPatch(typeof(CardModel), "PortraitPath", MethodType.Getter)]
public static class OutbreakPortraitPathPatch
{
    private static bool Prefix(CardModel __instance, ref string __result)
    {
        if (__instance is not Outbreak || !ModConfig.IsReplaceOutbreakEnabled)
        {
            return true;
        }

        __result = ModEntry.PortraitPng;
        return false;
    }
}
