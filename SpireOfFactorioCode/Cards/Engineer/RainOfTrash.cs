using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SpireOfFactorio.SpireOfFactorioCode.Util;

namespace SpireOfFactorio.SpireOfFactorioCode.Cards.Engineer;

public sealed class RainOfTrash : EngineerCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(9m, ValueProp.Move)];

    public RainOfTrash()
        : base(0, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
    {
        CustomResources<Scrap>.SetXCost(this);

        var cost = CustomResources<Scrap>.Cost(this);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var scrapCost = CustomResources<Scrap>.Cost(this);
        if (scrapCost == null)
        {
            MainFile.Logger.Warn($"[RainOfTrash]: Something went wrong, could not find the CustomResource<Scrap> object that is supposed to be tied to this card.");
            return;
        }
        int xValue = scrapCost.ResolveXValue();
        //MainFile.Logger.Info($"[RainOfTrash]: Play: Resolved X {xValue.ToString()}");
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).WithHitCount(xValue).TargetingAllOpponents(base.CombatState)
        .WithHitFx("vfx/vfx_attack_blunt")
        .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);
    }
}