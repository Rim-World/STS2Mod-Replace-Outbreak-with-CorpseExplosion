using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes;

namespace CorpseExplosionProbe;

/// <summary>
/// Test-only: makes the release build accept --scenario / --autoslay, which are otherwise
/// gated behind !NGame.IsReleaseGame(). Never ship this assembly.
/// </summary>
[HarmonyPatch(typeof(NGame), nameof(NGame.IsReleaseGame))]
public static class ReleaseModeUnlockPatch
{
    private static bool Prefix(ref bool __result)
    {
        __result = false;
        return false;
    }
}
