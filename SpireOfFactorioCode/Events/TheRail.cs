using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using SpireOfFactorio.SpireOfFactorioCode.Relics;

namespace SpireOfFactorio.SpireOfFactorioCode.Events;

public class TheRail : SpireOfFactorioEvent
{
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

    private int CurrentHpLoss => _initialHPLoss; // + (5 * numberOfAttempts);
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[4]
    {
        new DynamicVar("HpLoss", CurrentHpLoss),
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
        List<EventOption> list = new List<EventOption>();
        list.Add(new EventOption(this, TryCross, "THE_RAIL.pages.INITIAL.options.CROSS_0").ThatDoesDamage(CurrentHpLoss));
        list.Add(new EventOption(this, Resist, "THE_RAIL.pages.INITIAL.options.RESIST"));
        return list;
    }

    private async Task TryCross()
    {
        int roll = base.Rng.NextInt(0, 100);
        MainFile.Logger.Info("TheRail: Rolled " + roll.ToString());
        if (roll > 39)
        {
            // Train strike :)
            await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), base.Owner.Creature, CurrentHpLoss, ValueProp.Unblockable | ValueProp.Unpowered, null, null);
            numberOfAttempts++;
            base.DynamicVars["HpLoss"].BaseValue = CurrentHpLoss;
            string currentStrikeCountSuffix = GetTrainStrikeSuffix(numberOfAttempts - 1);
            string nextStrikeCountSuffix = GetTrainStrikeSuffix(numberOfAttempts);
            string textKey = "THE_RAIL.pages.CROSS_" + currentStrikeCountSuffix + ".options.CROSS_" + nextStrikeCountSuffix;
            SetEventState(L10NLookup("THE_RAIL.pages.CROSS_" + currentStrikeCountSuffix + ".description"), new EventOption[2]
            {
                new EventOption(this, TryCross, textKey).ThatDoesDamage(CurrentHpLoss),
                new EventOption(this, Resist, "THE_RAIL.pages.INITIAL.options.RESIST")
            });
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