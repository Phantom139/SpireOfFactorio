using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using SpireOfFactorio.SpireOfFactorioCode.Powers;

namespace SpireOfFactorio.SpireOfFactorioCode.Cards.Engineer;

public sealed class ForceRedirection : EngineerCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(22m, ValueProp.Move), new BlockVar(1m, ValueProp.Move)];

    public ForceRedirection()
        : base(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);

        decimal totalStacks = 0;
        totalStacks += cardPlay.Target.GetPowerAmount<WeakPower>();
        totalStacks += cardPlay.Target.GetPowerAmount<VulnerablePower>();
        totalStacks += cardPlay.Target.GetPowerAmount<ScorchPower>();
        totalStacks += cardPlay.Target.GetPowerAmount<ShockPower>();

        decimal blockGained = DynamicVars.Block.BaseValue * totalStacks;
        await CreatureCmd.GainBlock(base.Owner.Creature, blockGained, ValueProp.Move, cardPlay);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(5m);
        base.EnergyCost.UpgradeBy(-1);
    }
}
