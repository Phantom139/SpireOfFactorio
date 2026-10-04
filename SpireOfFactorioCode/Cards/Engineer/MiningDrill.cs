using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SpireOfFactorio.SpireOfFactorioCode.Powers;

namespace SpireOfFactorio.SpireOfFactorioCode.Cards.Engineer;

public sealed class MiningDrill : EngineerCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<MiningDrillPower>("MiningDrill", 1m), new DynamicVar("Scrap", 1m)];

    public MiningDrill() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self)
    {

    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<MiningDrillPower>(choiceContext, base.Owner.Creature, DynamicVars["MiningDrill"].BaseValue, base.Owner.Creature, cardPlay.Card);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["MiningDrill"].UpgradeValueBy(1m);
    }


}
