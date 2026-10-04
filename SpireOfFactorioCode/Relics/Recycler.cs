using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace SpireOfFactorio.SpireOfFactorioCode.Relics;

public class Recycler : SpireOfFactorioRelic
{
    public override RelicRarity Rarity => RelicRarity.Event;
    private const string _recycleKey = "Recycle";
    private const int _recycleCount = 3;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar(_recycleKey, _recycleCount)];

    private bool _isActivating;
    private int _cardsRecycled;

    public override bool ShowCounter => true;

    public override int DisplayAmount
    {
        get
        {
            if (!IsActivating)
            {
                return CardsRecycled;
            }

            return base.DynamicVars[_recycleKey].IntValue;
        }
    }

    private bool IsActivating
    {
        get
        {
            return _isActivating;
        }
        set
        {
            AssertMutable();
            _isActivating = value;
            InvokeDisplayAmountChanged();
        }
    }

    [SavedProperty]
    public int CardsRecycled
    {
        get
        {
            return _cardsRecycled;
        }
        set
        {
            AssertMutable();
            _cardsRecycled = value;
            InvokeDisplayAmountChanged();
        }
    }

    public override async Task AfterCardDiscarded(PlayerChoiceContext choiceContext, CardModel card)
    {
        if (card.Owner == base.Owner && base.Owner.Creature.Side == base.Owner.Creature.CombatState.CurrentSide)
        {
            base.Status = ((CardsRecycled == _recycleCount) ? RelicStatus.Active : RelicStatus.Normal);
            if (CardsRecycled >= _recycleCount)
            {
                await TaskHelper.RunSafely(DoActivateVisuals());
                await Cmd.Wait(0.01f);
                CardCmd.Upgrade(card);
                await Cmd.Wait(0.01f);
                if (PileType.Hand.GetPile(base.Owner).Cards.Count < 10)
                {
                    await CardPileCmd.Add(card, PileType.Hand, CardPilePosition.Top);
                }
                CardsRecycled = 0;
            }
            else
            {
                CardsRecycled++;
            }
        }
    }

    private async Task DoActivateVisuals()
    {
        IsActivating = true;
        Flash();
        await Cmd.Wait(1f);
        IsActivating = false;
        base.Status = RelicStatus.Normal;
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        base.Status = RelicStatus.Normal;
        return Task.CompletedTask;
    }

}
