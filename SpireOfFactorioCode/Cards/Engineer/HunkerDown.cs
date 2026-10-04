using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SpireOfFactorio.Libraries.ConstructLib.Construct;
using SpireOfFactorio.Libraries.ConstructLib.Targeting;
using SpireOfFactorio.SpireOfFactorioCode.Powers;

namespace SpireOfFactorio.SpireOfFactorioCode.Cards.Engineer;

public sealed class HunkerDown : EngineerCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<DisabledPower>("Disabled", 2m), new BlockVar(20, ValueProp.Move)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<DisabledPower>()];

    public HunkerDown()
        : base(2, CardType.Skill, CardRarity.Uncommon, ConstructTargetTypes.AllConstructsIOwn)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var pets = base.Owner.Creature.Pets;
        foreach (Creature pet in pets)
        {
            if (pet.Monster is ConstructModel)
            {
                await CreatureCmd.GainBlock(pet, DynamicVars.Block, cardPlay);
                await PowerCmd.Apply<DisabledPower>(choiceContext, pet, DynamicVars["Disabled"].BaseValue, base.Owner.Creature, cardPlay.Card);
            }
        }
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
        DynamicVars["Disabled"].UpgradeValueBy(-1m);
        DynamicVars.Block.UpgradeValueBy(5m);
    }
}
