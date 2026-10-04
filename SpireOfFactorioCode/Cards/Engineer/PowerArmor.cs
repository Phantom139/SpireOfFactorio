using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using SpireOfFactorio.SpireOfFactorioCode.Powers;
using SpireOfFactorio.SpireOfFactorioCode.Util;

namespace SpireOfFactorio.SpireOfFactorioCode.Cards.Engineer;

public sealed class PowerArmor : EngineerCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<StrengthPower>(1m), new PowerVar<DexterityPower>(1m), new PowerVar<TitaniumPower>(3m)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<TitaniumPower>()];

    public PowerArmor() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        CustomResources<Scrap>.SetCanonicalCost(this, 1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<StrengthPower>(choiceContext, base.Owner.Creature, DynamicVars.Strength.BaseValue, base.Owner.Creature, cardPlay.Card);
        await PowerCmd.Apply<DexterityPower>(choiceContext, base.Owner.Creature, DynamicVars.Dexterity.BaseValue, base.Owner.Creature, cardPlay.Card);
        await PowerCmd.Apply<TitaniumPower>(choiceContext, base.Owner.Creature, DynamicVars["TitaniumPower"].BaseValue, base.Owner.Creature, cardPlay.Card);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Strength.UpgradeValueBy(2m);
        DynamicVars.Dexterity.UpgradeValueBy(2m);
        DynamicVars["TitaniumPower"].UpgradeValueBy(2m);
    }


}
