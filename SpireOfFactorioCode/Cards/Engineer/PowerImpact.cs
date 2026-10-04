using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using SpireOfFactorio.SpireOfFactorioCode.Powers;

namespace SpireOfFactorio.SpireOfFactorioCode.Cards.Engineer;

public sealed class PowerImpact : EngineerCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(13m, ValueProp.Move), new DynamicVar("PowerImpactBlockBuff", 1m)];

    public PowerImpact()
        : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        decimal totalDamage = DynamicVars.Damage.BaseValue;
        totalDamage += (DynamicVars["PowerImpactBlockBuff"].BaseValue * Owner.Creature.Block);
        totalDamage += (DynamicVars["PowerImpactBlockBuff"].BaseValue * Owner.Creature.GetPowerAmount<PlatingPower>());
        totalDamage += (DynamicVars["PowerImpactBlockBuff"].BaseValue * Owner.Creature.GetPowerAmount<TitaniumPower>());

        await DamageCmd.Attack(totalDamage).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars["PowerImpactBlockBuff"].UpgradeValueBy(1m);
    }
}
