using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using SpireOfFactorio.SpireOfFactorioCode.Util;

namespace SpireOfFactorio.SpireOfFactorioCode.Cards.Engineer;

public sealed class CargoDelivery : EngineerCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(20m, ValueProp.Move), new DynamicVar("CargoDeliveryWeakIncrease", 10m)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<WeakPower>()];

    public CargoDelivery()
        : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        CustomResources<Scrap>.SetCanonicalCost(this, 1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        decimal totalDamage = DynamicVars.Damage.BaseValue;
        if (cardPlay.Target.HasPower<WeakPower>())
        {
            totalDamage += decimal.Ceiling(totalDamage * ((DynamicVars["CargoDeliveryWeakIncrease"].BaseValue * cardPlay.Target.GetPowerAmount<WeakPower>()) / 100));
        }

        await DamageCmd.Attack(totalDamage).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
        DynamicVars.Damage.UpgradeValueBy(6m);
        DynamicVars["CargoDeliveryWeakIncrease"].UpgradeValueBy(5m);
    }
}
