using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using SpireOfFactorio.SpireOfFactorioCode.Util;

namespace SpireOfFactorio.SpireOfFactorioCode.Cards.Engineer;

public sealed class Pallisades : EngineerCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(7m, ValueProp.Move)];

    public override bool GainsBlock => true;

    public Pallisades()
        : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.None)
    {
        CustomResources<Scrap>.SetXCost(this);

        var cost = CustomResources<Scrap>.Cost(this);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var scrapCost = CustomResources<Scrap>.Cost(this);
        if (scrapCost == null)
        {
            MainFile.Logger.Warn($"[Pallisades]: Something went wrong, could not find the CustomResource<Scrap> object that is supposed to be tied to this card.");
            return;
        }
        int xValue = scrapCost.ResolveXValue();
        decimal blockGained = xValue * DynamicVars.Block.BaseValue;
        decimal blockNextTurn = decimal.Ceiling(blockGained / 2);
        await CreatureCmd.GainBlock(base.Owner.Creature, blockGained, ValueProp.Move, cardPlay);
        await PowerCmd.Apply<BlockNextTurnPower>(choiceContext, base.Owner.Creature, blockNextTurn, base.Owner.Creature, cardPlay.Card);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
    }
}