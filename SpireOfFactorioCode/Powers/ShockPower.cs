using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Saves.Runs;
using SpireOfFactorio.SpireOfFactorioCode.Relics;

namespace SpireOfFactorio.SpireOfFactorioCode.Powers;

public sealed class ShockPower : SpireOfFactorioPower
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("StunPercent", 0m)];

    public override PowerType Type
    {
        get
        {
            return PowerType.Debuff;
        }
    }

    private int _unalteredTurns = 0;
    private int _stacksLastTurn = 0;


    [SavedProperty]
    public int UnalteredTurns
    {
        get
        {
            return _unalteredTurns;
        }
        set
        {
            AssertMutable();
            _unalteredTurns = value;
            InvokeDisplayAmountChanged();
        }
    }

    [SavedProperty]
    public int StacksLastTurn
    {
        get
        {
            return _stacksLastTurn;
        }
        set
        {
            AssertMutable();
            _stacksLastTurn = value;
            InvokeDisplayAmountChanged();
        }
    }

    public override PowerStackType StackType => PowerStackType.Counter;

    public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;

    public override Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power is not ShockPower)
        {
            return Task.CompletedTask;
        }
        int playerCountIncrease = base.Owner.CombatState.Players.Count - 1;
        int factor = (playerCountIncrease > 0 ? 10 * playerCountIncrease : 0) + 100;

        int stacksBefore = base.Amount - (int)amount;
        decimal percentBefore = ((decimal)stacksBefore / (decimal)factor) * 100m;
        decimal percentNow = ((decimal)base.Amount / (decimal)factor) * 100m;

        DynamicVars["StunPercent"].UpgradeValueBy(percentNow - percentBefore);

        MainFile.Logger.Info($"[ShockPower] Value changes to {base.Amount} from {stacksBefore} [Delta: {amount}]. Percent change: {percentBefore} to {percentNow}, calling UpgradeValueBy with {percentNow - percentBefore}");

        return Task.CompletedTask;
    }

    public override Task AfterApplied(
        Creature? applier,
        CardModel? cardSource)
    {
        UnalteredTurns = 0;
        StacksLastTurn = base.Amount;

        return Task.CompletedTask;
    }

    public override async Task BeforeSideTurnStart(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (!participants.Contains(base.Owner))
        {
            return;
        }

        if (!base.Owner.IsAlive)
        {
            return;
        }

        // Additional Shock was applied since the previous turn.
        if (base.Amount > StacksLastTurn)
        {
            UnalteredTurns = 0;
        }
        else
        {
            UnalteredTurns++;
        }

        StacksLastTurn = base.Amount;

        int turnThreshold = 2;
        // Check if any players have the Fulgoran Tesla Cell
        IEnumerable<Creature> source = from c in base.Owner.CombatState.GetOpponentsOf(base.Owner)
                                       where c.IsAlive
                                       select c;
        int teslaCellCount = source.Sum(a => a.Player is not null && a.Player.Relics.Contains(ModelDb.Relic<FulgoranTeslaCell>()) ? 1 : 0);
        if (teslaCellCount > 0)
        {
            turnThreshold += 2;
        }

        if (UnalteredTurns >= turnThreshold)
        {
            await PowerCmd.Remove<ShockPower>(base.Owner);
            return;
        }

        if (base.Owner.Monster != null)
        {
            int playerCountIncrease = base.Owner.CombatState.Players.Count - 1;
            int factor = playerCountIncrease > 0 ? 10 * playerCountIncrease : 0;

            int roll = base.Owner.Monster.Rng.NextInt(100 + factor);

            MainFile.Logger.Info(
                $"[ShockPower] {base.Owner.Name} [{base.Amount}], Rolled {roll}");

            if (base.Amount >= roll)
            {
                if (!base.Owner.HasPower<ShockResistPower>())
                {
                    await CreatureCmd.Stun(base.Owner);
                    await PowerCmd.Remove<ShockPower>(base.Owner);
                    await PowerCmd.Apply<ShockResistPower>(new ThrowingPlayerChoiceContext(), base.Owner, ShockResistPower.DefaultTurnCount, base.Owner, null);
                }
            }
        }
        else
        {
            MainFile.Logger.Warn(
                $"[ShockPower] base.Owner.Monster was 'null'.");
        }
    }

}
