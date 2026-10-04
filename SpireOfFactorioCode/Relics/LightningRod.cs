using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using SpireOfFactorio.Libraries.ConstructLib.Construct;
using SpireOfFactorio.SpireOfFactorioCode.Characters;
using SpireOfFactorio.SpireOfFactorioCode.Powers;

namespace SpireOfFactorio.SpireOfFactorioCode.Relics;

[Pool(typeof(EngineerRelicPool))]
public class LightningRod : SpireOfFactorioRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<OverclockPower>("Overclock", 1), new DynamicVar("Turns", _turnCount)];


    private const int _turnCount = 3;
    private bool _isActivating;
    private int _turnsPassed;

    public override bool ShowCounter => true;

    public override int DisplayAmount
    {
        get
        {
            if (!IsActivating)
            {
                return TurnsPassed;
            }

            return _turnsPassed;
        }
    }

    private bool IsActivating
    {
        get
        {
            return _isActivating;
        }
        set
        {
            AssertMutable();
            _isActivating = value;
            InvokeDisplayAmountChanged();
        }
    }

    [SavedProperty]
    public int TurnsPassed
    {
        get
        {
            return _turnsPassed;
        }
        set
        {
            AssertMutable();
            _turnsPassed = value;
            InvokeDisplayAmountChanged();
        }
    }

    private async Task DoActivateVisuals()
    {
        IsActivating = true;
        Flash();
        await Cmd.Wait(1f);
        IsActivating = false;
        base.Status = RelicStatus.Normal;
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        base.Status = RelicStatus.Normal;
        return Task.CompletedTask;
    }

    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (base.Owner.Creature.Side == base.Owner.Creature.CombatState.CurrentSide)
        {
            TurnsPassed++;
            base.Status = ((TurnsPassed > _turnCount) ? RelicStatus.Active : RelicStatus.Normal);
            if (TurnsPassed > _turnCount)
            {
                await TaskHelper.RunSafely(DoActivateVisuals());
                await Cmd.Wait(0.01f);
                var pets = base.Owner.Creature.Pets;
                foreach (Creature pet in pets)
                {
                    if (pet.Monster is ConstructModel)
                    {
                        await PowerCmd.Apply<OverclockPower>(choiceContext, pet, DynamicVars["Overclock"].BaseValue, base.Owner.Creature, null);
                    }
                }
                await Cmd.Wait(0.01f);
                TurnsPassed = 0;
            }
        }
    }

}
