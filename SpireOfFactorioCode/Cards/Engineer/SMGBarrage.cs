using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpireOfFactorio.SpireOfFactorioCode.Cards.Engineer;

public sealed class SmgBarrage : EngineerCard
{
    protected override bool HasEnergyCostX => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(6m, ValueProp.Move), new DynamicVar("SMGBarrageXDoubleUpgrade", 5m)];

    public SmgBarrage()
        : base(0, CardType.Attack, CardRarity.Common, TargetType.AllEnemies)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int xValue = ResolveEnergyXValue();
        if (IsUpgraded)
        {
            if (xValue >= DynamicVars["SMGBarrageXDoubleUpgrade"].IntValue)
            {
                xValue *= 2;
            }
        }
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).WithHitCount(xValue).TargetingRandomOpponents(base.CombatState)
        .WithHitFx("vfx/vfx_attack_blunt")
        .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1m);
    }
}
