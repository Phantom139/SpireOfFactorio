using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using SpireOfFactorio.SpireOfFactorioCode.Relics;

namespace SpireOfFactorio.SpireOfFactorioCode.Events;

public class TheRail : SpireOfFactorioEvent
{
    private const int _donationCost = 25;
    private const int _startingSuccessChance = 40;
    private const int _initialHPLoss = 8;
    private int _numberOfAttempts;
    //private RelicModel reward;

    private const string _TopFactorioRelic = "TopFactorioRelic";
    private const string _MiddleFactorioRelic = "MiddleFactorioRelic";
    private const string _BottomFactorioRelic = "BottomFactorioRelic";

    public List<RelicModel> factorioRelics = [
        ModelDb.Relic<LongHandedInserter>(),
        ModelDb.Relic<RocketFuel>(),
        ModelDb.Relic<Bioflux>(),
        ModelDb.Relic<Recycler>(),
        ModelDb.Relic<SpeedModule>(),
        ModelDb.Relic<UtilitySciencePack>(),
        ModelDb.Relic<ProductivityModule>(),
        ModelDb.Relic<PortableFissionReactor>(),
        ModelDb.Relic<PortableEnergyShield>(),
        ModelDb.Relic<EfficiencyModule>()];

    private IReadOnlyList<RelicModel>? _selectableRelics;
    private IReadOnlyList<RelicModel> selectableRelics
    {
        get
        {
            AssertMutable();
            if (_selectableRelics == null)
            {
                // Filter out any Factorio relics we have already obtained this run
                factorioRelics = factorioRelics.Where(r => !base.Owner.Relics.Any(o => o.Id == r.Id)).ToList();

                _selectableRelics = factorioRelics.StableShuffle(base.Rng).Take(3).ToList();
            }

            return _selectableRelics;
        }
    }

    private int numberOfAttempts
    {
        get
        {
            return _numberOfAttempts;
        }
        set
        {
            AssertMutable();
            _numberOfAttempts = value;
        }
    }

    private int _current_success_chance;
    private int current_success_chance
    {
        get
        {
            return _current_success_chance;
        }
        set
        {
            AssertMutable();
            _current_success_chance = value;
        }
    }

    private int CurrentHpLoss => _initialHPLoss; // + (5 * numberOfAttempts);
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[5]
    {
        new DynamicVar("HpLoss", CurrentHpLoss),
        new GoldVar("OfferingCost", _donationCost),
        new StringVar(_TopFactorioRelic),
        new StringVar(_MiddleFactorioRelic),
        new StringVar(_BottomFactorioRelic)
    };

    public override bool IsAllowed(IRunState runState)
    {
        return true;
    }

    private string GetTrainStrikeSuffix(int strikeCount)
    {
        if (strikeCount > 2)
        {
            return "LOOP";
        }

        return strikeCount.ToString();
    }

    public override void CalculateVars()
    {
        ((StringVar)base.DynamicVars["TopFactorioRelic"]).StringValue = selectableRelics[0].Title.GetFormattedText();
        ((StringVar)base.DynamicVars["MiddleFactorioRelic"]).StringValue = selectableRelics[1].Title.GetFormattedText();
        ((StringVar)base.DynamicVars["BottomFactorioRelic"]).StringValue = selectableRelics[2].Title.GetFormattedText();
    }

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        current_success_chance = _startingSuccessChance;

        List<EventOption> list = new List<EventOption>();
        list.Add(new EventOption(this, TryCross, "THE_RAIL.pages.INITIAL.options.CROSS_0").ThatDoesDamage(CurrentHpLoss));
        if (base.Owner.Gold >= _donationCost)
        {
            list.Add(new EventOption(this, Donate, "THE_RAIL.pages.DONATE.options"));
        }
        else
        {
            list.Add(new EventOption(this, Donate, "THE_RAIL.pages.DONATE_FAIL.options"));
        }
        list.Add(new EventOption(this, Resist, "THE_RAIL.pages.INITIAL.options.RESIST"));
        return list;
    }

    private async Task Donate()
    {
        await PlayerCmd.LoseGold(_donationCost, base.Owner);
        current_success_chance += 10;
        string currentStrikeCountSuffix = GetTrainStrikeSuffix(numberOfAttempts - 1);
        string nextStrikeCountSuffix = GetTrainStrikeSuffix(numberOfAttempts);
        string textKey;
        if (numberOfAttempts == 0)
        {
            textKey = "THE_RAIL.pages.INITIAL.options.CROSS_0";
        }
        else
        {
            textKey = "THE_RAIL.pages.CROSS_" + currentStrikeCountSuffix + ".options.CROSS_" + nextStrikeCountSuffix;
        }

        List<EventOption> list = new List<EventOption>();
        list.Add(new EventOption(this, TryCross, textKey).ThatDoesDamage(CurrentHpLoss));
        if (base.Owner.Gold >= _donationCost)
        {
            list.Add(new EventOption(this, Donate, "THE_RAIL.pages.DONATE.options"));
        }
        else
        {
            list.Add(new EventOption(this, Donate, "THE_RAIL.pages.DONATE_FAIL.options"));
        }
        list.Add(new EventOption(this, Resist, "THE_RAIL.pages.INITIAL.options.RESIST"));

        // The zeke troll warning... don't matter how much he donates... The trains are out for blood.
        if (Owner.Character is Ironclad && Owner.RunState.Players.Count() > 1)
        {
            List<CardModel> zekeTrollCards = Owner.Deck.Cards
                .Where(card => card is FightMe)
                .ToList();
            if (zekeTrollCards.Count() > 0)
            {
                SetEventState(L10NLookup("THE_RAIL.pages.DONATE_ZEKE_TROLL_WARN.description"), list);
            }
        }
        else
        {
            if (current_success_chance < 60)
            {
                SetEventState(L10NLookup("THE_RAIL.pages.DONATE_FAIR.description"), list);
            }
            else if (current_success_chance <= 80)
            {
                SetEventState(L10NLookup("THE_RAIL.pages.DONATE_GOOD.description"), list);
            }
            else
            {
                SetEventState(L10NLookup("THE_RAIL.pages.DONATE_PERFECT.description"), list);
            }
            MainFile.Logger.Info("TheRail: Gold donated, Current success chance: " + current_success_chance.ToString());
        }
    }

    private async Task TryCross()
    {
        // 0.8.5 Meme: Zeke Punishment :)
        if (Owner.Character is Ironclad && Owner.RunState.Players.Count() > 1)
        {
            List<CardModel> zekeTrollCards = Owner.Deck.Cards
                .Where(card => card is FightMe)
                .ToList();
            if (zekeTrollCards.Count() > 0)
            {
                foreach (CardModel card in zekeTrollCards)
                {
                    CardModel ShameCard = base.Owner.RunState.CreateCard<Shame>(base.Owner);
                    ShameCard.AddKeyword(MegaCrit.Sts2.Core.Entities.Cards.CardKeyword.Eternal);
                    ShameCard.AddKeyword(MegaCrit.Sts2.Core.Entities.Cards.CardKeyword.Innate);
                    ShameCard.AddKeyword(MegaCrit.Sts2.Core.Entities.Cards.CardKeyword.Retain);
                    await CardCmd.Transform(card, ShameCard, MegaCrit.Sts2.Core.Nodes.CommonUi.CardPreviewStyle.MessyLayout);
                    //CardPileCmd.RemoveFromDeck(card);
                }
                int currentHP = Owner.Creature.CurrentHp;
                await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), base.Owner.Creature, currentHP - 1, ValueProp.Unblockable | ValueProp.Unpowered, null, null);
                SetEventFinished(L10NLookup("THE_RAIL.pages.ZEKE_PUNISHMENT.description"));
                return;
            }

        }
        //
        int roll = base.Rng.NextInt(0, 100);
        MainFile.Logger.Info("TheRail: Rolled " + roll.ToString() + ", Current success chance: " + current_success_chance.ToString());
        if (roll > current_success_chance)
        {
            // Train strike :)
            await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), base.Owner.Creature, CurrentHpLoss, ValueProp.Unblockable | ValueProp.Unpowered, null, null);
            numberOfAttempts++;
            base.DynamicVars["HpLoss"].BaseValue = CurrentHpLoss;
            string currentStrikeCountSuffix = GetTrainStrikeSuffix(numberOfAttempts - 1);
            string nextStrikeCountSuffix = GetTrainStrikeSuffix(numberOfAttempts);
            string textKey = "THE_RAIL.pages.CROSS_" + currentStrikeCountSuffix + ".options.CROSS_" + nextStrikeCountSuffix;

            List<EventOption> list = new List<EventOption>();
            list.Add(new EventOption(this, TryCross, textKey).ThatDoesDamage(CurrentHpLoss));
            if (base.Owner.Gold >= _donationCost)
            {
                list.Add(new EventOption(this, Donate, "THE_RAIL.pages.DONATE.options"));
            }
            else
            {
                list.Add(new EventOption(this, Donate, "THE_RAIL.pages.DONATE_FAIL.options"));
            }
            list.Add(new EventOption(this, Resist, "THE_RAIL.pages.INITIAL.options.RESIST"));

            SetEventState(L10NLookup("THE_RAIL.pages.CROSS_" + currentStrikeCountSuffix + ".description"), list);
        }
        else
        {
            // Glittering Prize :P
            /**
            await RelicCmd.Obtain(reward.ToMutable(), base.Owner);
            if(numberOfAttempts > 0)
            {
                SetEventFinished(L10NLookup("THE_RAIL.pages.LOOTDAMAGED.description"));
            }
            else
            {
                SetEventFinished(L10NLookup("THE_RAIL.pages.LOOTSUCCESS.description"));
            }
            **/

            LocString locString;
            if (numberOfAttempts > 0)
            {
                locString = L10NLookup("THE_RAIL.pages.SUCCESS_WITH_DAMAGE.description");
            }
            else
            {
                locString = L10NLookup("THE_RAIL.pages.SUCCESS_NO_DAMAGE.description");
            }
            List<EventOption> eventOptions = new List<EventOption>();
            eventOptions.Add(new EventOption(this, Top, "THE_RAIL.pages.LOOT.options.TOP", GetRelicHoverTips(0)));
            eventOptions.Add(new EventOption(this, Middle, "THE_RAIL.pages.LOOT.options.MIDDLE", GetRelicHoverTips(1)));
            eventOptions.Add(new EventOption(this, Bottom, "THE_RAIL.pages.LOOT.options.BOTTOM", GetRelicHoverTips(2)));

            SetEventState(locString, eventOptions);
        }
    }

    private async Task Top()
    {
        await ObtainRelic(0);
    }

    private async Task Middle()
    {
        await ObtainRelic(1);
    }

    private async Task Bottom()
    {
        await ObtainRelic(2);
    }

    private async Task ObtainRelic(int index)
    {
        await RelicCmd.Obtain(selectableRelics[index].ToMutable(), base.Owner);
        await Done();
    }

    private Task Done()
    {
        if (numberOfAttempts > 0)
        {
            SetEventFinished(L10NLookup("THE_RAIL.pages.LOOTDAMAGED.description"));
        }
        else
        {
            SetEventFinished(L10NLookup("THE_RAIL.pages.LOOTSUCCESS.description"));
        }
        return Task.CompletedTask;
    }

    private Task Resist()
    {
        SetEventFinished(L10NLookup("THE_RAIL.pages.RESIST.description"));
        return Task.CompletedTask;
    }

    private IEnumerable<IHoverTip> GetRelicHoverTips(int index)
    {
        if (selectableRelics.Count <= index)
        {
            return Array.Empty<IHoverTip>();
        }

        List<IHoverTip> list = new List<IHoverTip>();
        list.AddRange(selectableRelics.ElementAt(index).HoverTips);

        return list;
    }

}