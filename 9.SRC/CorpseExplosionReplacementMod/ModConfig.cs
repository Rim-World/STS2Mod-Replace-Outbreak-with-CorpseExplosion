using CorpseExplosionReplacementMod.Patches;

namespace CorpseExplosionReplacementMod;

/// <summary>
/// 替换功能固定开启（已移除 RitsuLib 实时开关）。
/// 叠加 OutbreakShapeGuard：未来 beta 若改变 Outbreak 形态，所有补丁自动失效。
/// </summary>
public static class ModConfig
{
    public static bool IsReplaceOutbreakEnabled => OutbreakShapeGuard.IsSupported();
}
