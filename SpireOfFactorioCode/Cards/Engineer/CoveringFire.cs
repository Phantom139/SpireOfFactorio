using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SpireOfFactorio.SpireOfFactorioCode.Powers;

namespace SpireOfFactorio.SpireOfFactorioCode.Cards.Engineer;

public sealed class CoveringFire : EngineerCard
{

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<CoveringFirePower>("CoveringFire", 25m)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            return [HoverTipFactory.FromPower<CoveringFirePower>()];
        }
    }

    public CoveringFire() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<CoveringFirePower>(choiceContext, base.Owner.Creature, DynamicVars["CoveringFire"].BaseValue, base.Owner.Creature, cardPlay.Card);
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
        DynamicVars["CoveringFire"].UpgradeValueBy(10m);
    }

}
