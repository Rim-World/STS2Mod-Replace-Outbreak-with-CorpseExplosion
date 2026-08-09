using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace CorpseExplosionReplacementMod.Patches;

/// <summary>
/// 卡牌形状守卫：确认当前 Outbreak 仍是受支持的 beta 形态
/// （Skill / AllEnemies / 基础费用 3）。
/// 通过读取构造器写入的私有字段判断，不受本 mod 的 getter 补丁影响；
/// 若未来 beta 版本再次改动 Outbreak，所有补丁会自动失效，避免破坏游戏。
/// </summary>
public static class OutbreakShapeGuard
{
    private static readonly FieldInfo? TargetTypeField =
        AccessTools.Field(typeof(CardModel), "<TargetType>k__BackingField");

    private static readonly FieldInfo? CanonicalCostField =
        AccessTools.Field(typeof(CardModel), "<CanonicalEnergyCost>k__BackingField");

    private static bool? _supported;

    private static bool _warned;

    public static bool IsSupported()
    {
        if (_supported.HasValue)
        {
            return _supported.Value;
        }

        if (TargetTypeField == null || CanonicalCostField == null)
        {
            MarkUnsupported("backing fields not found");
            return false;
        }

        bool ok = false;
        try
        {
            Outbreak card = ModelDb.Card<Outbreak>();
            ok = card.Type == CardType.Skill
                && TargetTypeField.GetValue(card) is TargetType targetType && targetType == TargetType.AllEnemies
                && CanonicalCostField.GetValue(card) is int canonicalCost && canonicalCost == 3;
            _supported = ok;
        }
        catch
        {
            // ModelDb 尚未就绪时暂不拦截，下次访问再评估
            return true;
        }

        if (!ok)
        {
            MarkUnsupported("Outbreak shape differs from the supported beta version");
        }

        return ok;
    }

    private static void MarkUnsupported(string reason)
    {
        _supported = false;
        if (!_warned)
        {
            _warned = true;
            Log.Warn($"{ModEntry.ModId}: Outbreak shape is not the supported beta version ({reason}); patches are disabled.");
        }
    }
}
