using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace CorpseExplosionReplacementMod.Patches;

/// <summary>
/// 0.113.1 给 CardModel 增加了 BigPortrait，图鉴大图改用它，而不是 Portrait。
/// 这里把 Outbreak 的 BigPortrait 也接到同一套安全纹理加载器上。
///
/// 该类故意不带 HarmonyPatch 特性：0.111.1 没有 BigPortrait，直接 PatchAll 会因目标缺失
/// 抛异常并中断后续补丁。ModEntry 先探测属性是否存在，再决定是否手动打这个补丁。
/// </summary>
public static class OutbreakBigPortraitPatch
{
    public static bool Prefix(CardModel __instance, ref Texture2D __result)
    {
        if (__instance is not Outbreak || !ModConfig.IsReplaceOutbreakEnabled)
        {
            return true;
        }

        __result = PortraitTextureLoader.Get();
        return false;
    }
}
