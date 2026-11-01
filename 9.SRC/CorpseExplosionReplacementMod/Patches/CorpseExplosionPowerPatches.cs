using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace CorpseExplosionReplacementMod.Patches;

/// <summary>
/// 自定义 CorpseExplosionPower 的文本与图标替换（名称/描述/智能描述 + 64/256 图标）。
/// </summary>
[HarmonyPatch(typeof(PowerModel), "Title", MethodType.Getter)]
public static class CorpseExplosionPowerTitlePatch
{
    private static void Postfix(PowerModel __instance, ref LocString __result)
    {
        if (__instance is CorpseExplosionPower && ModConfig.IsReplaceOutbreakEnabled)
        {
            __result = new LocString("powers", "RE_CORPSE_EXPLOSION_POWER.title");
        }
    }
}

[HarmonyPatch(typeof(PowerModel), "Description", MethodType.Getter)]
public static class CorpseExplosionPowerDescriptionPatch
{
    private static void Postfix(PowerModel __instance, ref LocString __result)
    {
        if (__instance is CorpseExplosionPower && ModConfig.IsReplaceOutbreakEnabled)
        {
            __result = new LocString("powers", "RE_CORPSE_EXPLOSION_POWER.description");
        }
    }
}

[HarmonyPatch(typeof(PowerModel), "SmartDescription", MethodType.Getter)]
public static class CorpseExplosionPowerSmartDescriptionPatch
{
    private static void Postfix(PowerModel __instance, ref LocString __result)
    {
        if (__instance is CorpseExplosionPower && ModConfig.IsReplaceOutbreakEnabled)
        {
            __result = new LocString("powers", "RE_CORPSE_EXPLOSION_POWER.smartDescription");
        }
    }
}

[HarmonyPatch(typeof(PowerModel), "PackedIconPath", MethodType.Getter)]
public static class CorpseExplosionPowerPackedIconPathPatch
{
    private static bool Prefix(PowerModel __instance, ref string __result)
    {
        if (__instance is not CorpseExplosionPower || !ModConfig.IsReplaceOutbreakEnabled)
        {
            return true;
        }

        __result = ModEntry.PowerIcon64Path;
        return false;
    }
}

[HarmonyPatch(typeof(PowerModel), "ResolvedBigIconPath", MethodType.Getter)]
public static class CorpseExplosionPowerBigIconPathPatch
{
    private static bool Prefix(PowerModel __instance, ref string __result)
    {
        if (__instance is not CorpseExplosionPower || !ModConfig.IsReplaceOutbreakEnabled)
        {
            return true;
        }

        __result = ModEntry.PowerIcon256Path;
        return false;
    }
}

[HarmonyPatch(typeof(PowerModel), "Icon", MethodType.Getter)]
public static class CorpseExplosionPowerIconPatch
{
    private static bool Prefix(PowerModel __instance, ref Texture2D __result)
    {
        if (__instance is not CorpseExplosionPower || !ModConfig.IsReplaceOutbreakEnabled)
        {
            return true;
        }

        __result = PowerIconTextureLoader.Get64();
        return false;
    }
}

[HarmonyPatch(typeof(PowerModel), "BigIcon", MethodType.Getter)]
public static class CorpseExplosionPowerBigIconPatch
{
    private static bool Prefix(PowerModel __instance, ref Texture2D __result)
    {
        if (__instance is not CorpseExplosionPower || !ModConfig.IsReplaceOutbreakEnabled)
        {
            return true;
        }

        __result = PowerIconTextureLoader.Get256();
        return false;
    }
}
