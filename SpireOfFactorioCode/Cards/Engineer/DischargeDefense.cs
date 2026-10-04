using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SpireOfFactorio.SpireOfFactorioCode.Powers;

namespace SpireOfFactorio.SpireOfFactorioCode.Cards.Engineer;

public sealed class DischargeDefense : EngineerCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<DischargeDefensePower>("DischargeDefense", 10m)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<ShockPower>()];

    public DischargeDefense() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {

    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<DischargeDefensePower>(choiceContext, base.Owner.Creature, DynamicVars["DischargeDefense"].BaseValue, base.Owner.Creature, cardPlay.Card);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["DischargeDefense"].UpgradeValueBy(5m);
    }

}