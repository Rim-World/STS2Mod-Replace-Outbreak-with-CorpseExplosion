using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.DevConsole;
using MegaCrit.Sts2.Core.DevConsole.ConsoleCommands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Logging;

namespace CorpseExplosionProbe;

/// <summary>
/// Test-only assertion helper. The scenario fails if the expected combat state is not reached.
/// </summary>
public sealed class CheckConsoleCmd : AbstractConsoleCmd
{
    public override string CmdName => "ce_check";

    public override string Args => "<combat-ended|combat-active>";

    public override string Description => "Assert combat state for Corpse Explosion tests.";

    public override bool IsNetworked => false;

    public override CmdResult Process(Player? issuingPlayer, string[] args)
    {
        string expected = args.Length > 0 ? args[0].ToLowerInvariant() : "";
        bool active = CombatManager.Instance.IsInProgress;
        Log.Info($"[CEProbe] check expected={expected} combat_active={active}");

        return expected switch
        {
            "combat-ended" when !active => new CmdResult(true, "check passed: combat ended"),
            "combat-active" when active => new CmdResult(true, "check passed: combat active"),
            "combat-ended" => new CmdResult(false, "check failed: combat still active"),
            "combat-active" => new CmdResult(false, "check failed: combat already ended"),
            _ => new CmdResult(false, "usage: ce_check <combat-ended|combat-active>"),
        };
    }
}
