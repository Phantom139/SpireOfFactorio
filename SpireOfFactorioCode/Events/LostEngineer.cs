using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Unlocks;
using SpireOfFactorio.SpireOfFactorioCode.Cards.Quest;
using SpireOfFactorio.SpireOfFactorioCode.Relics;

namespace SpireOfFactorio.SpireOfFactorioCode.Events;

public class LostEngineer : SpireOfFactorioEvent
{
    private const string _TopFactorioRelic = "TopFactorioRelic";
    private const string _MiddleFactorioRelic = "MiddleFactorioRelic";
    private const string _BottomFactorioRelic = "BottomFactorioRelic";
    private const string _TopRelicCost = "TopRelicCost";
    private const string _MiddleRelicCost = "MiddleRelicCost";
    private const string _BottomRelicCost = "BottomRelicCost";

    private const int _TopRelicPrice = 50;
    private const int _MiddleRelicPrice = 100;
    private const int _BottomRelicPrice = 150;

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

    private bool canTakeEngineerQuest = false;

    public override bool IsAllowed(IRunState runState)
    {
        return runState.Players.All((Player player) => player.Gold >= 50);
    }

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        List<EventOption> list = new List<EventOption>();
        if (base.Owner.Gold >= _TopRelicPrice)
        {
            list.Add(new EventOption(this, Top, "LOST_ENGINEER.pages.INITIAL.options.TOP", GetRelicHoverTips(0)));
        }
        else
        {
            list.Add(new EventOption(this, null, "LOST_ENGINEER.pages.INITIAL.options.LOCKED"));
        }

        if (base.Owner.Gold >= _MiddleRelicPrice)
        {
            list.Add(new EventOption(this, Middle, "LOST_ENGINEER.pages.INITIAL.options.MIDDLE", GetRelicHoverTips(1)));
        }
        else
        {
            list.Add(new EventOption(this, null, "LOST_ENGINEER.pages.INITIAL.options.LOCKED"));
        }

        if (base.Owner.Gold >= _BottomRelicPrice)
        {
            list.Add(new EventOption(this, Bottom, "LOST_ENGINEER.pages.INITIAL.options.BOTTOM", GetRelicHoverTips(2)));
        }
        else
        {
            list.Add(new EventOption(this, null, "LOST_ENGINEER.pages.INITIAL.options.LOCKED"));
        }
        //if (canTakeEngineerQuest)
        //{
        //    list.Add(new EventOption(this, Quest, "LOST_ENGINEER.pages.INITIAL.options.PROPOSAL", HoverTipFactory.FromCardWithCardHoverTips<Radar>()));
        //}
        //else
        //{
        list.Add(new EventOption(this, Refuse, "LOST_ENGINEER.pages.INITIAL.options.REFUSE"));
        //}
        return list;
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new StringVar(_TopFactorioRelic),
        new StringVar(_MiddleFactorioRelic),
        new StringVar(_BottomFactorioRelic),
        new GoldVar(_TopRelicCost, _TopRelicPrice),
        new GoldVar(_MiddleRelicCost, _MiddleRelicPrice),
        new GoldVar(_BottomRelicCost, _BottomRelicPrice)];

    private async Task Top()
    {
        await Trade(0);
    }

    private async Task Middle()
    {
        await Trade(1);
    }

    private async Task Bottom()
    {
        await Trade(2);
    }

    private async Task Refuse()
    {
        await DoneRefuse();
    }

    private async Task Trade(int index)
    {
        if (index == 0)
            await PlayerCmd.LoseGold(_TopRelicPrice, base.Owner);
        else if (index == 1)
            await PlayerCmd.LoseGold(_MiddleRelicPrice, base.Owner);
        else
            await PlayerCmd.LoseGold(_BottomRelicPrice, base.Owner);

        await RelicCmd.Obtain(selectableRelics[index].ToMutable(), base.Owner);
        await Done();
    }

    private Task Done()
    {
        SetEventFinished(L10NLookup("LOST_ENGINEER.pages.TRADEDONE.description"));
        return Task.CompletedTask;
    }

    private Task DoneRefuse()
    {
        SetEventFinished(L10NLookup("LOST_ENGINEER.pages.TRADEREFUSE.description"));
        return Task.CompletedTask;
    }

    private async Task Quest()
    {
        CardModel card = base.Owner.RunState.CreateCard<Radar>(base.Owner);
        CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(card, PileType.Deck), 2f);
        SetEventFinished(L10NLookup("LOST_ENGINEER.pages.RADARPROPOSALDONE.description"));
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

    public override void CalculateVars()
    {
        ((StringVar)base.DynamicVars["TopFactorioRelic"]).StringValue = selectableRelics[0].Title.GetFormattedText();
        ((StringVar)base.DynamicVars["MiddleFactorioRelic"]).StringValue = selectableRelics[1].Title.GetFormattedText();
        ((StringVar)base.DynamicVars["BottomFactorioRelic"]).StringValue = selectableRelics[2].Title.GetFormattedText();

        bool haveAll = true;
        MainFile.Logger.Info("Begin Relic Test...");
        UnlockState unlockState = SaveManager.Instance.GenerateUnlockStateFromProgress();
        foreach (RelicModel relic in factorioRelics)
        {
            if (!unlockState.Relics.ToList().Contains(relic))
            {
                MainFile.Logger.Info("Missing Relic: " + relic.Id + " (" + relic.Title + ")");
                int i = 0;
                foreach (RelicModel m in unlockState.Relics.ToList())
                {
                    MainFile.Logger.Info(i.ToString() + ". " + m.ToString());
                    i++;
                }
                haveAll = false;
                break;
            }
        }
        canTakeEngineerQuest = haveAll;
    }

}
