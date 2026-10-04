using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SpireOfFactorio.SpireOfFactorioCode.Util;

namespace SpireOfFactorio.SpireOfFactorioCode.Cards.Engineer;

public sealed class Harvest : EngineerCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Scrap", 2m)];

    public Harvest() : base(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
    {

    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var scrap = CustomResources<Scrap>.Get(
            Owner.PlayerCombatState!);
        scrap.ModifyAmount(DynamicVars["Scrap"].IntValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Scrap"].UpgradeValueBy(1m);
    }

}
