using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace SpireOfFactorio.SpireOfFactorioCode.Relics;

public class RocketFuel : SpireOfFactorioRelic
{
    //public override RelicRarity Rarity => RelicRarity.Event;
    public override RelicRarity Rarity => RelicRarity.Event;

    private const string _energyCostKey = "EnergyCost";
    private const string _energyReturnKey = "EnergyReturn";

    private int _energyStacks = 0;

    public override bool ShowCounter => true;

    public override int DisplayAmount
    {
        get
        {
            return EnergyStacks;
        }
    }

    [SavedProperty]
    public int EnergyStacks
    {
        get
        {
            return _energyStacks;
        }
        set
        {
            AssertMutable();
            _energyStacks = value;
            InvokeDisplayAmountChanged();
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(1), new DynamicVar(_energyCostKey, 3), new DynamicVar(_energyReturnKey, 1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.ForEnergy(this)];

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side == base.Owner.Creature.Side)
        {
            decimal energyGained = decimal.Ceiling(EnergyStacks / 2);
            await PlayerCmd.GainEnergy(energyGained, base.Owner);
        }
    }

    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner == base.Owner && cardPlay.Resources.EnergyValue >= DynamicVars[_energyCostKey].BaseValue)
        {
            Flash();
            EnergyStacks++;
            //await PlayerCmd.GainEnergy(DynamicVars[_energyReturnKey].BaseValue, base.Owner);
        }
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        EnergyStacks = 0;
        return Task.CompletedTask;
    }

}
