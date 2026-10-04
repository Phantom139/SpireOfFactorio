using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpireOfFactorio.SpireOfFactorioCode.Powers;

public sealed class CropHarvestPower : SpireOfFactorioPower
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new HealVar(1m), new DynamicVar("Turn", 1m)];


    private int _turnsLeft = 0;

    [SavedProperty]
    public int TurnsLeft
    {
        get
        {
            return _turnsLeft;
        }
        set
        {
            AssertMutable();
            _turnsLeft = value;
            decimal oldDVar = DynamicVars["Turn"].BaseValue;

            DynamicVars["Turn"].UpgradeValueBy(_turnsLeft - oldDVar);
            InvokeDisplayAmountChanged();
        }
    }

    public override PowerType Type
    {
        get
        {
            return PowerType.Buff;
        }
    }

    public override PowerStackType StackType => PowerStackType.Counter;

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        TurnsLeft = 3;
        if (cardSource.DynamicVars.ContainsKey("Turns"))
        {
            TurnsLeft = cardSource.DynamicVars["Turns"].IntValue;
        }

        return Task.CompletedTask;
    }

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target == base.Owner)
        {
            if (result.UnblockedDamage > 0)
            {
                await PowerCmd.ModifyAmount(choiceContext, this, -result.UnblockedDamage, dealer, cardSource);
            }
        }
    }

    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side == CombatSide.Player)
        {
            if (base.Owner.IsPlayer)
            {
                TurnsLeft--;
                if (TurnsLeft <= 0)
                {
                    await CreatureCmd.Heal(base.Owner, (decimal)base.Amount);
                    await PowerCmd.Remove(this);
                }
            }
        }
    }

    public override async Task BeforeDeath(Creature creature)
    {
        if (creature.IsEnemy)
        {
            // Scan for valid enemies.
            foreach (Creature c in creature.CombatState.Enemies)
            {
                if (c == creature)
                {
                    continue;
                }
                if (c.IsDead)
                {
                    continue;
                }
                if (c.IsSecondaryEnemy)
                {
                    continue;
                }
                if (c.IsPrimaryEnemy)
                {
                    return;
                }
            }
            // If we reach this spot, that was the last enemy and combat is about to end, fire the heal.
            await CreatureCmd.Heal(base.Owner, (decimal)base.Amount);
            await PowerCmd.Remove(this);
        }

    }

}
