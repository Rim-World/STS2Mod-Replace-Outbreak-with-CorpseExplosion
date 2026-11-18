using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using CorpseExplosionReplacementMod.Patches;

namespace CorpseExplosionReplacementMod;

[ModInitializer("ModLoaded")]
public static class ModEntry
{
    public const string ModId = "CorpseExplosionReplacementMod";

    public static readonly string PortraitPng =
        $"res://{ModId}/images/card_portraits/big/corpse_explosion.png";

    public static readonly string PowerIcon64Path =
        $"res://{ModId}/images/powers/corpse_explosion.png";

    public static readonly string PowerIcon256Path =
        $"res://{ModId}/images/powers/big/corpse_explosion.png";

    private static Harmony? _harmony;

    public static void ModLoaded()
    {
        try
        {
            Log.Info($"{ModId}: loading...");

            _harmony = new Harmony(ModId);
            _harmony.PatchAll(Assembly.GetExecutingAssembly());
            PatchOptionalBigPortrait(_harmony);
            Log.Info($"{ModId}: Harmony patches applied (Outbreak -> Corpse Explosion)");
            Log.Info($"{ModId}: AbstractModel.AfterDeath hook applied");
        }
        catch (Exception e)
        {
            Log.Error($"{ModId}: failed to apply patches: {e}");
        }
    }

    /// <summary>
    /// 0.111.1 没有 CardModel.BigPortrait，0.113.1 才加入；不能放进 PatchAll。
    /// 属性缺失时跳过，属性存在时把 Outbreak 的高清卡图接到安全加载器。
    /// </summary>
    private static void PatchOptionalBigPortrait(Harmony harmony)
    {
        MethodInfo? bigPortraitGetter = AccessTools.PropertyGetter(typeof(CardModel), "BigPortrait");
        if (bigPortraitGetter == null)
        {
            Log.Info($"{ModId}: CardModel.BigPortrait not found; optional inspect-portrait patch skipped");
            return;
        }

        harmony.Patch(
            bigPortraitGetter,
            prefix: new HarmonyMethod(
                typeof(OutbreakBigPortraitPatch),
                nameof(OutbreakBigPortraitPatch.Prefix)));

        Log.Info($"{ModId}: CardModel.BigPortrait patch applied");
    }
}
