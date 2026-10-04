using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SpireOfFactorio.SpireOfFactorioCode.Powers;

namespace SpireOfFactorio.SpireOfFactorioCode.Cards.Engineer;

public sealed class Blueprint : EngineerCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<ReducedScrapCostPower>("ReducedScrapCost", 2m), new DynamicVar("Scrap", 1m)];

    public Blueprint() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<ReducedScrapCostPower>(choiceContext, base.Owner.Creature, DynamicVars["ReducedScrapCost"].BaseValue, base.Owner.Creature, cardPlay.Card);
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
        DynamicVars["ReducedScrapCost"].UpgradeValueBy(1m);
    }

}
