using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpireOfFactorio.SpireOfFactorioCode.Cards.Engineer;

public sealed class Conductor : EngineerCard
{
    public const string _increaseKey = "Increase";

    public decimal _extraDamage;

    public decimal ExtraDamage
    {
        get
        {
            return _extraDamage;
        }
        set
        {
            AssertMutable();
            _extraDamage = value;
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(10m, ValueProp.Move), new DynamicVar("Increase", 5m), new PowerVar<VulnerablePower>(2m)];

    public Conductor()
        : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);
        await PowerCmd.Apply<VulnerablePower>(choiceContext, cardPlay.Target, DynamicVars.Vulnerable.BaseValue, base.Owner.Creature, cardPlay.Card);

        base.DynamicVars.Damage.BaseValue += base.DynamicVars["Increase"].BaseValue;
        ExtraDamage += base.DynamicVars["Increase"].BaseValue;
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);
        DynamicVars.Vulnerable.UpgradeValueBy(1m);
        DynamicVars["Increase"].UpgradeValueBy(2m);
    }

    protected override void AfterDowngraded()
    {
        AfterDowngraded();
        DynamicVars.Damage.BaseValue += ExtraDamage;
    }

}
