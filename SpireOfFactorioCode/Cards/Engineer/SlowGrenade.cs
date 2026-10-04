using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using SpireOfFactorio.SpireOfFactorioCode.Powers;
using SpireOfFactorio.SpireOfFactorioCode.Util;

namespace SpireOfFactorio.SpireOfFactorioCode.Cards.Engineer;

public sealed class SlowGrenade : EngineerCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<StrengthPower>(2m), new PowerVar<DexterityPower>(2m), new PowerVar<ShockPower>("Shock", 10m)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<ShockPower>()];

    public SlowGrenade()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        CustomResources<Scrap>.SetCanonicalCost(this, 1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await PowerCmd.Apply<StrengthPower>(choiceContext, cardPlay.Target, -DynamicVars.Strength.BaseValue, base.Owner.Creature, cardPlay.Card);
        await PowerCmd.Apply<DexterityPower>(choiceContext, cardPlay.Target, -DynamicVars.Dexterity.BaseValue, base.Owner.Creature, cardPlay.Card);
        foreach (Creature hittableEnemy in base.CombatState.HittableEnemies)
        {
            await PowerCmd.Apply<ShockPower>(choiceContext, hittableEnemy, DynamicVars["Shock"].BaseValue, base.Owner.Creature, cardPlay.Card);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Strength.UpgradeValueBy(1m);
        DynamicVars.Dexterity.UpgradeValueBy(1m);
        DynamicVars["Shock"].UpgradeValueBy(5m);
    }
}
