using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Saves.Runs;
using SpireOfFactorio.SpireOfFactorioCode.Util;

namespace SpireOfFactorio.SpireOfFactorioCode.Powers;

public sealed class ForgeFormPower : SpireOfFactorioPower
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Scrap", 3m)];

    public override PowerType Type
    {
        get
        {
            return PowerType.Buff;
        }
    }

    public override PowerStackType StackType => PowerStackType.Single;

    private int _scrapSpent = 0;

    [SavedProperty]
    public int ScrapSpent
    {
        get
        {
            return _scrapSpent;
        }
        set
        {
            AssertMutable();
            _scrapSpent = value;
        }
    }

    private bool _firedThisTurn = false;

    [SavedProperty]
    public bool FiredThisTurn
    {
        get
        {
            return _firedThisTurn;
        }
        set
        {
            AssertMutable();
            _firedThisTurn = value;
        }
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != base.Owner.Player)
        {
            return;
        }
        ScrapSpent = 0;
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side == CombatSide.Player)
        {
            FiredThisTurn = false;
        }
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (!Owner.IsPlayer)
        {
            return;
        }
        if (cardPlay.Card.Owner != base.Owner.Player)
        {
            return;
        }
        var scrapCost = CustomResources<Scrap>.Cost(cardPlay.Card);
        if (scrapCost == null)
        {
            return;
        }
        ScrapSpent += scrapCost.GetWithModifiers(CostModifiers.All);
        if (ScrapSpent >= 3)
        {
            if (!FiredThisTurn)
            {
                Flash();
                //List<CardModel> exhaustedCards = PileType.Exhaust.GetPile(Owner.Player).Cards.ToList();
                IEnumerable<CardModel> enumerable = PileType.Exhaust.GetPile(Owner.Player).Cards.TakeRandom(1, Owner.Player.RunState.Rng.CombatCardSelection);
                CardModel card = enumerable.FirstOrDefault();
                if (card != null)
                {
                    await CardCmd.AutoPlay(choiceContext, card, null);
                    await CardCmd.Exhaust(choiceContext, card, false, true);
                    FiredThisTurn = true;
                }
            }
        }
    }

}
