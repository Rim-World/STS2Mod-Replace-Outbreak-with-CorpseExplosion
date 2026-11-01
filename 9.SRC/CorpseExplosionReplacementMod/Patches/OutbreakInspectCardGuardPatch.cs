using System;
using System.Collections.Generic;
using System.Reflection;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens;

namespace CorpseExplosionReplacementMod.Patches;

/// <summary>
/// 图鉴大图守护补丁（方案 B1，改进版）：
/// 替换 NInspectCardScreen.SetCard（private 方法，当前无其他 mod 补丁，无绕过
/// 第三方补丁链的风险），复制原逻辑；其中对 UpdateCardDisplay 的调用仍走原方法
/// （保留其既有补丁链），并包 try/catch。
///
/// 背景：游戏 v0.111 的资源清理机制（AssetCache.UnloadMissedCacheAssets）会 Dispose
/// 未预缓存的纹理，导致 hover tip 图标 SetTexture 抛 ObjectDisposedException
/// （日志：NHoverTipSet.Init 内 CompressedTexture2D），若异常中断 UpdateCardDisplay，
/// 图鉴大图打开流程中断、卡牌停留在透明状态（只看到暗色遮罩）。
/// 本补丁保证：即使 hover tips 创建再次失败，大图打开流程不中断；
/// 若卡牌渲染也失败，再兜底仅渲染卡牌本体（无 hover tips）。
/// </summary>
[HarmonyPatch(typeof(NInspectCardScreen), "SetCard")]
public static class OutbreakInspectCardGuardPatch
{
    private static readonly FieldInfo CardsField = AccessTools.Field(typeof(NInspectCardScreen), "_cards")!;
    private static readonly FieldInfo IndexField = AccessTools.Field(typeof(NInspectCardScreen), "_index")!;
    private static readonly FieldInfo CardField = AccessTools.Field(typeof(NInspectCardScreen), "_card")!;
    private static readonly FieldInfo LeftButtonField = AccessTools.Field(typeof(NInspectCardScreen), "_leftButton")!;
    private static readonly FieldInfo RightButtonField = AccessTools.Field(typeof(NInspectCardScreen), "_rightButton")!;
    private static readonly FieldInfo UpgradeTickboxField = AccessTools.Field(typeof(NInspectCardScreen), "_upgradeTickbox")!;
    private static readonly FieldInfo ViewAllUpgradedField = AccessTools.Field(typeof(NInspectCardScreen), "_viewAllUpgraded")!;
    private static readonly MethodInfo UpdateCardDisplayMethod = AccessTools.Method(typeof(NInspectCardScreen), "UpdateCardDisplay")!;

    private static bool Prefix(NInspectCardScreen __instance, int index)
    {
        try
        {
            var cards = (List<CardModel>?)CardsField.GetValue(__instance);
            if (cards == null || cards.Count == 0)
            {
                return false;
            }

            int clamped = Math.Clamp(index, 0, cards.Count - 1);
            IndexField.SetValue(__instance, clamped);

            var leftButton = (NButton)LeftButtonField.GetValue(__instance)!;
            var rightButton = (NButton)RightButtonField.GetValue(__instance)!;
            var upgradeTickbox = (NTickbox)UpgradeTickboxField.GetValue(__instance)!;

            leftButton.Visible = clamped > 0;
            leftButton.MouseFilter = leftButton.Visible ? Control.MouseFilterEnum.Stop : Control.MouseFilterEnum.Ignore;
            rightButton.Visible = clamped < cards.Count - 1;
            rightButton.MouseFilter = rightButton.Visible ? Control.MouseFilterEnum.Stop : Control.MouseFilterEnum.Ignore;

            CardModel card = cards[clamped];
            bool upgradable = card.MaxUpgradeLevel > 0;
            upgradeTickbox.Visible = upgradable;
            upgradeTickbox.MouseFilter = upgradable ? Control.MouseFilterEnum.Stop : Control.MouseFilterEnum.Ignore;
            bool viewAllUpgraded = (bool)(ViewAllUpgradedField.GetValue(__instance) ?? false);
            upgradeTickbox.IsTicked = card.IsUpgraded || viewAllUpgraded;

            // 仍走原方法（保留其他 mod 的补丁链），仅吞掉异常避免中断大图打开流程
            try
            {
                UpdateCardDisplayMethod?.Invoke(__instance, null);
            }
            catch (Exception e)
            {
                Log.Warn($"{ModEntry.ModId}: card inspect hover tip display failed, showing card without tips: {e.Message}");
                TryRenderCardOnly(__instance);
            }
        }
        catch (Exception e)
        {
            Log.Error($"{ModEntry.ModId}: SetCard guard failed: {e}");
        }

        return false;
    }

    /// <summary>
    /// 兜底：仅渲染卡牌本体（跳过 hover tips 等易受资源清理影响的环节）。
    /// </summary>
    private static void TryRenderCardOnly(NInspectCardScreen screen)
    {
        try
        {
            var cards = (List<CardModel>?)CardsField.GetValue(screen);
            if (cards == null || cards.Count == 0)
            {
                return;
            }

            int index = (int)(IndexField.GetValue(screen) ?? 0);
            var cardModel = (CardModel)cards[Math.Clamp(index, 0, cards.Count - 1)].MutableClone();
            var cardNode = (NCard)CardField.GetValue(screen)!;
            cardNode.Model = cardModel;
            cardNode.UpdateVisuals(PileType.None, CardPreviewMode.Normal);
        }
        catch (Exception e2)
        {
            Log.Error($"{ModEntry.ModId}: SetCard fallback render failed: {e2.Message}");
        }
    }
}
