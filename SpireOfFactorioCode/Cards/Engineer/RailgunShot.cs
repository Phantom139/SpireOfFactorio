using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpireOfFactorio.SpireOfFactorioCode.Cards.Engineer;

public sealed class RailgunShot : EngineerCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(25m, ValueProp.Unblockable), new DynamicVar("RailgunExtra", 15m)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<IntangiblePower>()];

    //
    // Summary:
    //     The only difference between the starting Strike cards are portrait, attack vfx,
    //     and color.
    public RailgunShot()
        : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        bool willDealBonusDamage = await RemoveBonusDamagePowers(cardPlay.Target) > 0;

        if (willDealBonusDamage)
        {
            decimal totalHit = decimal.Ceiling(DynamicVars.Damage.BaseValue + (DynamicVars["RailgunExtra"].BaseValue));
            AttackCommand attack = DamageCmd.Attack(totalHit).FromCard(this).Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash");
            Extensions.AttackCommandExtensions.Unblockable(attack);
            await attack.Execute(choiceContext);
        }
        else
        {
            AttackCommand attack = DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash");
            Extensions.AttackCommandExtensions.Unblockable(attack);
            await attack.Execute(choiceContext);
        }
        await CreatureCmd.LoseBlock(cardPlay.Target, cardPlay.Target.Block);

    }

    private async Task<int> RemoveBonusDamagePowers(Creature target)
    {
        int removedCount = 0;

        if (target.HasPower<ArtifactPower>())
        {
            await PowerCmd.Remove<ArtifactPower>(target);
            removedCount++;
        }

        if (target.HasPower<BurrowedPower>())
        {
            // We do a second call here to trigger the stun effect
            await CreatureCmd.LoseBlock(target, target.Block);
            await PowerCmd.Remove<BurrowedPower>(target);
            removedCount++;
        }

        if (target.HasPower<HardenedShellPower>())
        {
            await PowerCmd.Remove<HardenedShellPower>(target);
            removedCount++;
        }

        if (target.HasPower<HardToKillPower>())
        {
            await PowerCmd.Remove<HardToKillPower>(target);
            removedCount++;
        }

        if (target.HasPower<PlatingPower>())
        {
            await PowerCmd.Remove<PlatingPower>(target);
            removedCount++;
        }

        if (target.HasPower<SlipperyPower>())
        {
            await PowerCmd.Remove<SlipperyPower>(target);
            removedCount++;
        }

        return removedCount;
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
        DynamicVars.Damage.UpgradeValueBy(5m);
        DynamicVars["RailgunExtra"].UpgradeValueBy(5m);
    }
}
