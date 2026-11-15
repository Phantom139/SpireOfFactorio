using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;


namespace SpireOfFactorio.SpireOfFactorioCode.Cards.Token;

[Pool(typeof(TokenCardPool))]
public sealed class Logistics : SpireOfFactorioCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(1)];

    public Logistics()
        : base(0, CardType.Skill, CardRarity.Token, TargetType.Self)
    {
    }

    public static async Task<IEnumerable<Logistics>> CreateInHand(Player owner, int amount, ICombatState combatState, Player? creator = null, bool upgraded = false)
    {
        IEnumerable<Logistics> cards = Create(owner, amount, combatState, upgraded);
        await CardPileCmd.AddGeneratedCardsToCombat(cards, PileType.Hand, creator ?? owner);
        return cards;
    }

    public static IEnumerable<Logistics> Create(Player owner, int amount, ICombatState combatState, bool upgraded = false)
    {
        List<Logistics> list = new List<Logistics>();
        for (int i = 0; i < amount; i++)
        {
            Logistics created = combatState.CreateCard<Logistics>(owner);
            if (upgraded)
            {
                CardCmd.Upgrade(created);
            }
            list.Add(created);
        }


        return list;
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        List<CardModel> drawn = (await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, base.Owner)).ToList();
        if (IsUpgraded)
        {
            foreach (CardModel card in drawn)
            {
                if (card.EnergyCost.Canonical != 0 && !card.EnergyCost.CostsX)
                {
                    card.EnergyCost.AddThisTurnOrUntilPlayed(-1);
                }
            }
        }
    }

    protected override void OnUpgrade()
    {

    }
}
