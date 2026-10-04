using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SpireOfFactorio.SpireOfFactorioCode.Util;

namespace SpireOfFactorio.SpireOfFactorioCode.Cards.Engineer;

public sealed class ElectricFurnace : EngineerCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Scrap", 4m)];

    public ElectricFurnace() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {

    }

    protected override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var scrap = CustomResources<Scrap>.Get(
            Owner.PlayerCombatState!);
        scrap.ModifyAmount(DynamicVars["Scrap"].IntValue);
        return Task.CompletedTask;
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Scrap"].UpgradeValueBy(3m);
    }

}
