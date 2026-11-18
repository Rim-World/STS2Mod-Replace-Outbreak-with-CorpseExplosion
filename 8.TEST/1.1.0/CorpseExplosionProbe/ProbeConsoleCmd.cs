using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.DevConsole;
using MegaCrit.Sts2.Core.DevConsole.ConsoleCommands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Runs;

namespace CorpseExplosionProbe;

/// <summary>
/// Test-only console command. It prints the current Outbreak model and combat HP/powers
/// so a scenario log can prove whether CorpseExplosionPower dealt damage on death.
/// </summary>
public sealed class ProbeConsoleCmd : AbstractConsoleCmd
{
    public override string CmdName => "ce_probe";

    public override string Args => "";

    public override string Description => "Dump Outbreak card and combat state for Corpse Explosion tests.";

    public override bool IsNetworked => false;

    public override CmdResult Process(Player? issuingPlayer, string[] args)
    {
        try
        {
            Outbreak card = ModelDb.Card<Outbreak>();
            string vars = string.Join(
                ",",
                card.DynamicVars.Select(kv => $"{kv.Key}={kv.Value.BaseValue}"));
            Log.Info(
                $"[CEProbe] card title={card.TitleLocString.GetFormattedText()} " +
                $"cost={card.EnergyCost.Canonical} target={card.TargetType} vars=[{vars}]");
            Log.Info(
                $"[CEProbe] desc={card.Description.GetRawText().Replace("\n", " | ")}");
            try
            {
                Texture2D bigPortrait = card.BigPortrait;
                Log.Info(
                    $"[CEProbe] bigPortrait valid={GodotObject.IsInstanceValid(bigPortrait)} " +
                    $"type={bigPortrait?.GetType().Name}");
            }
            catch (Exception e)
            {
                Log.Error($"[CEProbe] bigPortrait probe failed: {e}");
            }
        }
        catch (Exception e)
        {
            Log.Error($"[CEProbe] card probe failed: {e}");
        }

        if (!RunManager.Instance.IsInProgress)
        {
            Log.Info("[CEProbe] run=not-in-progress");
            return new CmdResult(true, "probe logged");
        }

        if (!CombatManager.Instance.IsInProgress)
        {
            Log.Info("[CEProbe] combat=not-in-progress");
            return new CmdResult(true, "probe logged");
        }

        ICombatState? state = CombatManager.Instance.DebugOnlyGetState();
        if (state == null)
        {
            Log.Info("[CEProbe] combat=null");
            return new CmdResult(true, "probe logged");
        }

        Log.Info($"[CEProbe] combat=in-progress creatures={state.Creatures.Count} enemies={state.Enemies.Count}");
        for (int i = 0; i < state.Creatures.Count; i++)
        {
            Creature creature = state.Creatures[i];
            string powers = string.Join(
                ",",
                creature.Powers.Select(p => $"{p.Id.Entry}:{p.Amount}"));
            Log.Info(
                $"[CEProbe] creature[{i}] name={creature.Name} side={creature.Side} " +
                $"hp={creature.CurrentHp}/{creature.MaxHp} alive={creature.IsAlive} powers=[{powers}]");
        }

        for (int i = 0; i < state.Enemies.Count; i++)
        {
            Creature enemy = state.Enemies[i];
            Log.Info(
                $"[CEProbe] enemy[{i}] name={enemy.Name} hp={enemy.CurrentHp}/{enemy.MaxHp} " +
                $"alive={enemy.IsAlive}");
        }

        return new CmdResult(true, "probe logged");
    }
}
