using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Models.Orbs;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using SpireOfFactorio.SpireOfFactorioCode.Events;
using SpireOfFactorio.SpireOfFactorioCode.Extensions;
using SpireOfFactorio.SpireOfFactorioCode.Powers;

namespace SpireOfFactorio.SpireOfFactorioCode.Cards.Event;

[Pool(typeof(EventCardPool))]
public sealed class AssembledStrike : SpireOfFactorioCard
{
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
        new PowerVar<ScorchPower>("RiderScorch", 7m)
    ];

    public TheAssemblingMachine.CardColor _assemblingMachineColor;
    public TheAssemblingMachine.RiderEffect _assemblingMachineRider;
    public TheAssemblingMachine.RiderEnhancement _assemblingMachineBonus;

    public AssembledStrike() : base(2, CardType.Attack, CardRarity.Event, TargetType.AnyEnemy, false)
    {

    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            if (AssemblingMachineRider != 0)
            {
                return TheAssemblingMachine.GetRiderHoverTips(AssemblingMachineRider);
            }

            return Array.Empty<IHoverTip>();
        }
    }

    public override string CustomPortraitPath
    {
        get
        {
            string color = "";
            switch (AssemblingMachineColor)
            {
                case TheAssemblingMachine.CardColor.Red:
                    color = "red";
                    break;
                case TheAssemblingMachine.CardColor.Green:
                    color = "green";
                    break;
                case TheAssemblingMachine.CardColor.Yellow:
                    color = "yellow";
                    break;
                case TheAssemblingMachine.CardColor.Purple:
                    color = "purple";
                    break;
                case TheAssemblingMachine.CardColor.Blue:
                    color = "blue";
                    break;
                case TheAssemblingMachine.CardColor.Orange:
                    color = "orange";
                    break;
            }
            return $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_{color}.png".PlaceholderProtected_BigCardImagePath();
        }
    }

    [SavedProperty(SerializationCondition.SaveIfNotTypeDefault)]
    public TheAssemblingMachine.CardColor AssemblingMachineColor
    {
        get
        {
            return _assemblingMachineColor;
        }
        set
        {
            AssertMutable();
            _assemblingMachineColor = value;
        }
    }

    [SavedProperty(SerializationCondition.SaveIfNotTypeDefault)]
    public TheAssemblingMachine.RiderEffect AssemblingMachineRider
    {
        get
        {
            return _assemblingMachineRider;
        }
        set
        {
            AssertMutable();
            _assemblingMachineRider = value;
        }
    }

    [SavedProperty(SerializationCondition.SaveIfNotTypeDefault)]
    public TheAssemblingMachine.RiderEnhancement AssemblingMachineBonus
    {
        get
        {
            return _assemblingMachineBonus;
        }
        set
        {
            AssertMutable();
            _assemblingMachineBonus = value;
            if (_assemblingMachineBonus == TheAssemblingMachine.RiderEnhancement.BonusDamage)
            {
                DynamicVars.Damage.UpgradeValueBy(DynamicVars["BonusRiderDamage"].BaseValue);
            }
            if (_assemblingMachineBonus == TheAssemblingMachine.RiderEnhancement.ReducedCost)
            {
                base.EnergyCost.UpgradeBy(-DynamicVars["BonusRiderEnergy"].IntValue);
            }
            if (_assemblingMachineBonus == TheAssemblingMachine.RiderEnhancement.Replay)
            {
                base.BaseReplayCount += DynamicVars["BonusRiderReplay"].IntValue;
            }
        }
    }

    protected override void AddExtraArgsToDescription(LocString description)
    {
        description.Add("HasRider", AssemblingMachineRider != TheAssemblingMachine.RiderEffect.None);
        TheAssemblingMachine.RiderEffect[] values = Enum.GetValues<TheAssemblingMachine.RiderEffect>();
        for (int i = 0; i < values.Length; i++)
        {
            TheAssemblingMachine.RiderEffect riderEffect = values[i];
            description.Add(riderEffect.ToString(), AssemblingMachineRider == riderEffect);
        }
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);
        switch (AssemblingMachineRider)
        {
            case TheAssemblingMachine.RiderEffect.Strength:
                await PowerCmd.Apply<StrengthPower>(choiceContext, base.Owner.Creature, DynamicVars["RiderStrength"].BaseValue, base.Owner.Creature, cardPlay.Card);
                break;
            case TheAssemblingMachine.RiderEffect.Heal:
                decimal healAmount = decimal.Ceiling(base.Owner.Creature.MaxHp * (DynamicVars["RiderHeal"].BaseValue / 100));
                await CreatureCmd.Heal(base.Owner.Creature, healAmount);
                break;
            case TheAssemblingMachine.RiderEffect.Poison:
                if (cardPlay.Target.IsAlive)
                {
                    await PowerCmd.Apply<PoisonPower>(choiceContext, cardPlay.Target, DynamicVars["RiderPoison"].BaseValue, base.Owner.Creature, this);
                }
                break;
            case TheAssemblingMachine.RiderEffect.Shivs:
                for (int i = 0; i < base.DynamicVars["RiderShivs"].IntValue; i++)
                {
                    await Shiv.CreateInHand(base.Owner, base.CombatState);
                    await Cmd.CustomScaledWait(0.1f, 0.2f);
                }
                break;
            case TheAssemblingMachine.RiderEffect.MinionStrike:
                CardModel cardModel = (await CardSelectCmd.FromHand(prefs: new CardSelectorPrefs(CardSelectorPrefs.TransformSelectionPrompt, DynamicVars["RiderMinion"].IntValue), context: choiceContext, player: base.Owner, filter: null, source: this)).FirstOrDefault();
                if (cardModel != null)
                {
                    CardModel cardModel2 = base.CombatState.CreateCard<MinionStrike>(base.Owner);
                    if (base.IsUpgraded)
                    {
                        CardCmd.Upgrade(cardModel2);
                    }

                    await CardCmd.Transform(cardModel, cardModel2);
                }
                break;
            case TheAssemblingMachine.RiderEffect.Forge:
                ForgeCmd.Forge(DynamicVars["RiderForge"].BaseValue, base.Owner, this);
                break;
            case TheAssemblingMachine.RiderEffect.Doom:
                if (cardPlay.Target.IsAlive)
                {
                    await PowerCmd.Apply<DoomPower>(choiceContext, cardPlay.Target, DynamicVars["RiderDoom"].BaseValue, base.Owner.Creature, this);
                }
                break;
            case TheAssemblingMachine.RiderEffect.Summon:
                await CreatureCmd.TriggerAnim(base.Owner.Creature, Necrobinder.GetSummonAnimIfApplicable(base.Owner.Character), Necrobinder.GetSummonDelayIfApplicable(base.Owner.Character));
                await OstyCmd.Summon(choiceContext, base.Owner, DynamicVars["RiderSummon"].BaseValue, this);
                break;
            case TheAssemblingMachine.RiderEffect.Lightning:
                for (int i = 0; i < base.DynamicVars["RiderLightning"].IntValue; i++)
                {
                    OrbCmd.Channel<LightningOrb>(choiceContext, base.Owner);
                }
                break;
            case TheAssemblingMachine.RiderEffect.Plasma:
                for (int i = 0; i < base.DynamicVars["RiderPlasma"].IntValue; i++)
                {
                    OrbCmd.Channel<PlasmaOrb>(choiceContext, base.Owner);
                }
                break;
            case TheAssemblingMachine.RiderEffect.Scorch:
                if (cardPlay.Target.IsAlive)
                {
                    await PowerCmd.Apply<ScorchPower>(choiceContext, cardPlay.Target, DynamicVars["RiderScorch"].BaseValue, base.Owner.Creature, this);
                }
                break;
            case TheAssemblingMachine.RiderEffect.Shock:
                if (cardPlay.Target.IsAlive)
                {
                    await PowerCmd.Apply<ShockPower>(choiceContext, cardPlay.Target, DynamicVars["RiderShock"].BaseValue, base.Owner.Creature, this);
                }
                break;
            default:
                break;
        }
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
        DynamicVars.Damage.UpgradeValueBy(3m);
    }

}
