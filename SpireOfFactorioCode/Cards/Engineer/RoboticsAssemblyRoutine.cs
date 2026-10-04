using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace SpireOfFactorio.SpireOfFactorioCode.Cards.Engineer;

public sealed class RoboticsAssemblyRoutine : EngineerCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("RobotCards", 2m), new DynamicVar("Selections", 3m)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<ConstructionRobot>(), HoverTipFactory.FromCard<LogisticsRobot>()];

    public RoboticsAssemblyRoutine() : base(3, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
    }

    private CardModel CreateConstructionRobot()
    {
        return base.Owner.Creature.CombatState.CreateCard(
            ModelDb.Card<ConstructionRobot>(),
            base.Owner);
    }

    private CardModel CreateLogisticsRobot()
    {
        return base.Owner.Creature.CombatState.CreateCard(
            ModelDb.Card<LogisticsRobot>(),
            base.Owner);
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        List<CardModel> options = new();

        if (base.Owner.RunState.Players.Count > 1)
        {
            bool constructionGetsTwo =
                base.Owner.PlayerRng
                    .GetRng(MegaCrit.Sts2.Core.Entities.Rngs.PlayerRngType.Rewards)
                    .NextInt(2) == 0;

            if (constructionGetsTwo)
            {
                options.Add(CreateConstructionRobot());
                options.Add(CreateConstructionRobot());
                options.Add(CreateLogisticsRobot());
            }
            else
            {
                options.Add(CreateConstructionRobot());
                options.Add(CreateLogisticsRobot());
                options.Add(CreateLogisticsRobot());
            }
        }
        else
        {
            options.Add(CreateConstructionRobot());
            options.Add(CreateConstructionRobot());
            options.Add(CreateConstructionRobot());
        }

        if (IsUpgraded)
        {
            foreach (CardModel card in options)
            {
                CardCmd.Upgrade(card);
            }
        }

        CardPile playerHand = PileType.Hand.GetPile(base.Owner);

        List<CardModel> selection = (await CardSelectCmd.FromSimpleGrid(choiceContext, options, base.Owner, new CardSelectorPrefs(base.SelectionScreenPrompt, DynamicVars["RobotCards"].IntValue))).ToList();
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        foreach (CardModel option in selection)
        {
            CardPileAddResult? addedCard = await CardPileCmd.Add(option, playerHand, CardPilePosition.Top);
            if (addedCard != null)
            {
                addedCard.Value.cardAdded.SetToFreeThisTurn();
            }
        }
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }

}