using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Models.Orbs;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using SpireOfFactorio.SpireOfFactorioCode.Cards.Enchantments;
using SpireOfFactorio.SpireOfFactorioCode.Cards.Event;
using SpireOfFactorio.SpireOfFactorioCode.Powers;

namespace SpireOfFactorio.SpireOfFactorioCode.Events;

public class TheAssemblingMachine : SpireOfFactorioEvent
{
    public enum RiderEffect
    {
        None,
        Strength,
        Heal,
        Poison,
        Shivs,
        MinionStrike,
        Forge,
        Summon,
        Doom,
        Lightning,
        Plasma,
        Shock,
        Scorch
    }

    public enum CardColor
    {
        Red,
        Green,
        Yellow,
        Purple,
        Blue,
        Orange,
    }

    private const int _SharpGold = 50;
    private const int _EfficientGold = 100;
    private const int _ReplicatingGold = 150;
    private const string _SharpCost = "SharpCost";
    private const string _EfficientCost = "EfficientCost";
    private const string _ReplicatingCost = "ReplicatingCost";

    public CardColor _chosenCardColor;

    public CardColor ChosenCardColor
    {
        get
        {
            return _chosenCardColor;
        }
        set
        {
            AssertMutable();
            _chosenCardColor = value;
        }
    }

    public int _rngRed;
    public int RngRed
    {
        get
        {
            return _rngRed;
        }
        set
        {
            AssertMutable();
            _rngRed = value;
        }
    }

    public int _rngGreen;
    public int RngGreen
    {
        get
        {
            return _rngGreen;
        }
        set
        {
            AssertMutable();
            _rngGreen = value;
        }
    }

    public int _rngYellow;
    public int RngYellow
    {
        get
        {
            return _rngYellow;
        }
        set
        {
            AssertMutable();
            _rngYellow = value;
        }
    }

    public int _rngPurple;
    public int RngPurple
    {
        get
        {
            return _rngPurple;
        }
        set
        {
            AssertMutable();
            _rngPurple = value;
        }
    }

    public int _rngBlue;
    public int RngBlue
    {
        get
        {
            return _rngBlue;
        }
        set
        {
            AssertMutable();
            _rngBlue = value;
        }
    }

    public int _rngOrange;
    public int RngOrange
    {
        get
        {
            return _rngOrange;
        }
        set
        {
            AssertMutable();
            _rngOrange = value;
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(6m, ValueProp.Move),
        new DamageVar("BonusRiderDamage", 3m, ValueProp.Move),
        new EnergyVar("BonusRiderEnergy", 1),
        new DynamicVar("BonusRiderReplay", 1m),
        new DynamicVar("RiderStrength", 2m),
        new HealVar("RiderHeal", 5m),
        new PowerVar<PoisonPower>("RiderPoison", 5m),
        new DynamicVar("RiderShivs", 2m),
        new DynamicVar("RiderMinion", 1m),
        new DynamicVar("RiderForge", 7m),
        new SummonVar("RiderSummon", 5m),
        new PowerVar<DoomPower>("RiderDoom", 5m),
        new DynamicVar("RiderLightning", 2m),
        new DynamicVar("RiderPlasma", 1m),
        new PowerVar<ShockPower>("RiderShock", 10m),
        new PowerVar<ScorchPower>("RiderScorch", 7m),
        new GoldVar(_SharpCost, _SharpGold),
        new GoldVar(_EfficientCost, _EfficientGold),
        new GoldVar(_ReplicatingCost, _ReplicatingGold)
    ];

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        RngRed = Rng.NextInt(0, 100);
        RngGreen = Rng.NextInt(0, 100);
        RngYellow = Rng.NextInt(0, 100);
        RngPurple = Rng.NextInt(0, 100);
        RngBlue = Rng.NextInt(0, 100);
        RngOrange = Rng.NextInt(0, 100);
        return [new EventOption(this, ChooseGemstone, "THE_ASSEMBLING_MACHINE.INITIAL.options.CHOOSE_GEMSTONE")];
    }

    public Task ChooseGemstone()
    {
        IEnumerable<EventOption> collection = new EventOption[6]
        {
            new EventOption(this, GemstoneRed, "THE_ASSEMBLING_MACHINE.pages.CHOOSE_GEMSTONE.options.RED", GetCardTypeHoverTip(CardColor.Red)),
            new EventOption(this, GemstoneGreen, "THE_ASSEMBLING_MACHINE.pages.CHOOSE_GEMSTONE.options.GREEN", GetCardTypeHoverTip(CardColor.Green)),
            new EventOption(this, GemstoneYellow, "THE_ASSEMBLING_MACHINE.pages.CHOOSE_GEMSTONE.options.YELLOW", GetCardTypeHoverTip(CardColor.Yellow)),
            new EventOption(this, GemstonePurple, "THE_ASSEMBLING_MACHINE.pages.CHOOSE_GEMSTONE.options.PURPLE", GetCardTypeHoverTip(CardColor.Purple)),
            new EventOption(this, GemstoneBlue, "THE_ASSEMBLING_MACHINE.pages.CHOOSE_GEMSTONE.options.BLUE", GetCardTypeHoverTip(CardColor.Blue)),
            new EventOption(this, GemstoneOrange, "THE_ASSEMBLING_MACHINE.pages.CHOOSE_GEMSTONE.options.ORANGE", GetCardTypeHoverTip(CardColor.Orange))
        };
        SetEventState(L10NLookup("THE_ASSEMBLING_MACHINE.pages.CHOOSE_GEMSTONE.description"), collection.TakeRandom(3, base.Rng));
        return Task.CompletedTask;
    }

    public RiderEffect GetCardRider(CardColor cardColor)
    {
        switch (cardColor)
        {
            case CardColor.Red:
                return RngRed < 50 ? RiderEffect.Strength : RiderEffect.Heal;
            case CardColor.Green:
                return RngGreen < 50 ? RiderEffect.Poison : RiderEffect.Shivs;
            case CardColor.Yellow:
                return RngYellow < 50 ? RiderEffect.MinionStrike : RiderEffect.Forge;
            case CardColor.Purple:
                return RngPurple < 50 ? RiderEffect.Summon : RiderEffect.Doom;
            case CardColor.Blue:
                return RngBlue < 50 ? RiderEffect.Lightning : RiderEffect.Plasma;
            case CardColor.Orange:
                return RngOrange < 50 ? RiderEffect.Shock : RiderEffect.Scorch;
        }
        return RiderEffect.None;
    }

    public CardHoverTip GetCardTypeHoverTip(CardColor cardColor)
    {
        AssembledStrike assembledStrike = base.Owner.RunState.CreateCard<AssembledStrike>(base.Owner);
        assembledStrike.AssemblingMachineColor = cardColor;
        assembledStrike.AssemblingMachineRider = GetCardRider(cardColor);
        return new CardHoverTip(assembledStrike);
    }

    public Task GemstoneRed()
    {
        ChosenCardColor = CardColor.Red;
        return ChooseBonusEffect();
    }

    public Task GemstoneGreen()
    {
        ChosenCardColor = CardColor.Green;
        return ChooseBonusEffect();
    }

    public Task GemstoneYellow()
    {
        ChosenCardColor = CardColor.Yellow;
        return ChooseBonusEffect();
    }

    public Task GemstonePurple()
    {
        ChosenCardColor = CardColor.Purple;
        return ChooseBonusEffect();
    }

    public Task GemstoneBlue()
    {
        ChosenCardColor = CardColor.Blue;
        return ChooseBonusEffect();
    }

    public Task GemstoneOrange()
    {
        ChosenCardColor = CardColor.Orange;
        return ChooseBonusEffect();
    }

    public Task ChooseBonusEffect()
    {
        List<EventOption> list = new List<EventOption>();
        if (base.Owner.Gold >= _SharpGold)
        {
            list.Add(new EventOption(this, SharpEnhancement, "THE_ASSEMBLING_MACHINE.pages.CHOOSE_ENHANCEMENT.options.SHARP", GetEnhancementHoverTip(ModelDb.Enchantment<Sharp>())));
        }
        else
        {
            list.Add(new EventOption(this, null, "THE_ASSEMBLING_MACHINE.pages.CHOOSE_ENHANCEMENT.options.LOCKED"));
        }

        if (base.Owner.Gold >= _EfficientGold)
        {
            list.Add(new EventOption(this, EfficientEnhancement, "THE_ASSEMBLING_MACHINE.pages.CHOOSE_ENHANCEMENT.options.EFFICIENT", GetEnhancementHoverTip(ModelDb.Enchantment<Efficient>())));
        }
        else
        {
            list.Add(new EventOption(this, null, "THE_ASSEMBLING_MACHINE.pages.CHOOSE_ENHANCEMENT.options.LOCKED"));
        }

        if (base.Owner.Gold >= _ReplicatingGold)
        {
            list.Add(new EventOption(this, ReplicatingEnhancement, "THE_ASSEMBLING_MACHINE.pages.CHOOSE_ENHANCEMENT.options.REPLICATING", GetEnhancementHoverTip(ModelDb.Enchantment<Replicating>())));
        }
        else
        {
            list.Add(new EventOption(this, null, "THE_ASSEMBLING_MACHINE.pages.CHOOSE_ENHANCEMENT.options.LOCKED"));
        }
        list.Add(new EventOption(this, SkipEnhancement, "THE_ASSEMBLING_MACHINE.pages.CHOOSE_ENHANCEMENT.options.NONE"));
        SetEventState(L10NLookup("THE_ASSEMBLING_MACHINE.pages.CHOOSE_ENHANCEMENT.description"), list);

        return Task.CompletedTask;
    }

    public async Task SharpEnhancement()
    {
        await PlayerCmd.LoseGold(_SharpGold, base.Owner);
        AssembledStrike assembledStrike = base.Owner.RunState.CreateCard<AssembledStrike>(base.Owner);
        //assembledStrike.AssemblingMachineBonus = RiderEnhancement.BonusDamage;
        assembledStrike.AssemblingMachineColor = ChosenCardColor;
        assembledStrike.AssemblingMachineRider = GetCardRider(ChosenCardColor);
        CardCmd.Enchant<Sharp>(assembledStrike, 3m);
        CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(assembledStrike, PileType.Deck), 3f);
        SetEventFinished(L10NLookup("THE_ASSEMBLING_MACHINE.pages.DONE"));
    }

    public async Task EfficientEnhancement()
    {
        await PlayerCmd.LoseGold(_EfficientGold, base.Owner);
        AssembledStrike assembledStrike = base.Owner.RunState.CreateCard<AssembledStrike>(base.Owner);
        //assembledStrike.AssemblingMachineBonus = RiderEnhancement.ReducedCost;
        assembledStrike.AssemblingMachineColor = ChosenCardColor;
        assembledStrike.AssemblingMachineRider = GetCardRider(ChosenCardColor);
        CardCmd.Enchant<Efficient>(assembledStrike, 1m);
        CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(assembledStrike, PileType.Deck), 3f);
        SetEventFinished(L10NLookup("THE_ASSEMBLING_MACHINE.pages.DONE"));
    }

    public async Task ReplicatingEnhancement()
    {
        await PlayerCmd.LoseGold(_ReplicatingGold, base.Owner);
        AssembledStrike assembledStrike = base.Owner.RunState.CreateCard<AssembledStrike>(base.Owner);
        //assembledStrike.AssemblingMachineBonus = RiderEnhancement.Replay;
        assembledStrike.AssemblingMachineColor = ChosenCardColor;
        assembledStrike.AssemblingMachineRider = GetCardRider(ChosenCardColor);
        CardCmd.Enchant<Replicating>(assembledStrike, 1m);
        CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(assembledStrike, PileType.Deck), 3f);
        SetEventFinished(L10NLookup("THE_ASSEMBLING_MACHINE.pages.DONE"));
    }

    public async Task SkipEnhancement()
    {
        AssembledStrike assembledStrike = base.Owner.RunState.CreateCard<AssembledStrike>(base.Owner);
        //assembledStrike.AssemblingMachineBonus = RiderEnhancement.None;
        assembledStrike.AssemblingMachineColor = ChosenCardColor;
        assembledStrike.AssemblingMachineRider = GetCardRider(ChosenCardColor);
        CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(assembledStrike, PileType.Deck), 3f);
        SetEventFinished(L10NLookup("THE_ASSEMBLING_MACHINE.pages.DONE"));
    }

    public CardHoverTip GetEnhancementHoverTip(EnchantmentModel rider)
    {
        AssembledStrike assembledStrike = base.Owner.RunState.CreateCard<AssembledStrike>(base.Owner);
        assembledStrike.AssemblingMachineColor = ChosenCardColor;
        assembledStrike.AssemblingMachineRider = GetCardRider(ChosenCardColor);
        assembledStrike.Enchantment = rider;
        return new CardHoverTip(assembledStrike);
    }

    public static IHoverTip[] GetRiderHoverTips(RiderEffect rider)
    {
        return rider switch
        {
            RiderEffect.None => Array.Empty<IHoverTip>(),
            RiderEffect.Strength => new IHoverTip[1]
            {
                HoverTipFactory.FromPower<StrengthPower>(),
            },
            RiderEffect.Heal => Array.Empty<IHoverTip>(),
            RiderEffect.Poison => new IHoverTip[1] { HoverTipFactory.FromPower<PoisonPower>() },
            RiderEffect.Shivs => new IHoverTip[1] { HoverTipFactory.FromCard<Shiv>() },
            RiderEffect.MinionStrike => new IHoverTip[1] { HoverTipFactory.FromCard<MinionStrike>() },
            RiderEffect.Forge => new IHoverTip[1] { HoverTipFactory.FromCard<SovereignBlade>() },
            RiderEffect.Doom => new IHoverTip[1] { HoverTipFactory.FromPower<DoomPower>() },
            RiderEffect.Summon => Array.Empty<IHoverTip>(),
            RiderEffect.Lightning => new IHoverTip[1] { HoverTipFactory.FromOrb<LightningOrb>() },
            RiderEffect.Plasma => new IHoverTip[1] { HoverTipFactory.FromOrb<PlasmaOrb>() },
            RiderEffect.Scorch => new IHoverTip[1] { HoverTipFactory.FromPower<ScorchPower>() },
            RiderEffect.Shock => new IHoverTip[1] { HoverTipFactory.FromPower<ShockPower>() },
            _ => throw new ArgumentOutOfRangeException("rider", rider, null),
        };
    }

    public override bool IsAllowed(IRunState runState)
    {
        return true;
    }

}
