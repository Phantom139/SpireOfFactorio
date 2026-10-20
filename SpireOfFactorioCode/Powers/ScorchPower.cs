using BaseLib.Hooks;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using SpireOfFactorio.Libraries.ConstructLib.Construct;
using SpireOfFactorio.SpireOfFactorioCode.Relics;

namespace SpireOfFactorio.SpireOfFactorioCode.Powers;

public sealed class ScorchPower : SpireOfFactorioPower
{

    public override PowerType Type
    {
        get
        {
            return PowerType.Debuff;
        }
    }

    private int _plasmaFuelDebuffsAdded = 0;

    [SavedProperty]
    public int PlasmaFuelDebuffsAdded
    {
        get
        {
            return _plasmaFuelDebuffsAdded;
        }
        set
        {
            AssertMutable();
            _plasmaFuelDebuffsAdded = value;
            InvokeDisplayAmountChanged();
        }
    }

    public override PowerStackType StackType => PowerStackType.Counter;

    public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (!participants.Contains(base.Owner))
        {
            return;
        }
        if (side == CombatSide.Player && base.Owner.IsPlayer)
        {
            // Player based scorch triggers at the end of the turn
            return;
        }

        NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NGroundFireVfx.Create(base.Owner));
        SfxCmd.Play("event:/sfx/characters/attack_fire");
        await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), base.Owner, base.Amount, ValueProp.Unpowered, null, null);
        if (base.Owner.IsAlive)
        {
            if (base.Amount < 5)
            {
                await PowerCmd.Remove<ScorchPower>(base.Owner);
            }

            double percent = 50;
            int halvings = 0;
            foreach (Player p in base.Owner.CombatState.Players)
            {
                halvings += p.Relics.Contains(ModelDb.Relic<RefinedFlammables>()) ? 1 : 0;
            }

            if (halvings > 0)
            {
                percent /= Math.Pow(2, halvings);
            }

            Decimal reduction = decimal.Ceiling((decimal)(base.Amount) * (decimal)(percent / 100));
            await PowerCmd.ModifyAmount(new ThrowingPlayerChoiceContext(), this, -reduction, null, null);

        }
        else
        {
            await Cmd.CustomScaledWait(0.1f, 0.25f);
        }
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        // This block of code is specifically for scorch applied to players, it works a little differently.
        if (!participants.Contains(base.Owner))
        {
            return;
        }
        if (side == CombatSide.Enemy && !base.Owner.IsPlayer)
        {
            // Enemy-based scorch triggers at the start of the turn
            return;
        }

        NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NGroundFireVfx.Create(base.Owner));
        SfxCmd.Play("event:/sfx/characters/attack_fire");
        await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), base.Owner, base.Amount, ValueProp.Unpowered, null, null);
        if (base.Owner.IsAlive)
        {
            if (base.Amount < 5)
            {
                await PowerCmd.Remove<ScorchPower>(base.Owner);
            }

            double percent = 50;
            Decimal reduction = decimal.Ceiling((decimal)(base.Amount) * (decimal)(percent / 100));
            await PowerCmd.ModifyAmount(new ThrowingPlayerChoiceContext(), this, -reduction, null, null);

        }
        else
        {
            await Cmd.CustomScaledWait(0.1f, 0.25f);
        }
    }

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        if (applier.IsPlayer && base.Owner.IsEnemy)
        {
            if (applier.Player.GetRelic<PlasmaFuel>() != null)
            {
                await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), base.Owner, -PlasmaFuel.debuff_count, applier, cardSource);
                await PowerCmd.Apply<DexterityPower>(new ThrowingPlayerChoiceContext(), base.Owner, -PlasmaFuel.debuff_count, applier, cardSource);
                PlasmaFuelDebuffsAdded += 1;
            }
        }
        else
        {
            // Could have been a construct
            if (applier.Monster is ConstructModel)
            {
                if (applier.PetOwner.GetRelic<PlasmaFuel>() != null)
                {
                    await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), base.Owner, -PlasmaFuel.debuff_count, applier, cardSource);
                    await PowerCmd.Apply<DexterityPower>(new ThrowingPlayerChoiceContext(), base.Owner, -PlasmaFuel.debuff_count, applier, cardSource);
                    PlasmaFuelDebuffsAdded += 1;
                }
            }

        }
    }

    public override async Task AfterRemoved(Creature oldOwner)
    {
        if (PlasmaFuelDebuffsAdded > 0)
        {
            decimal StackAdjust = -(PlasmaFuelDebuffsAdded * -PlasmaFuel.debuff_count);
            if (base.Owner.GetPower<StrengthPower>() != null)
            {
                await PowerCmd.ModifyAmount(new ThrowingPlayerChoiceContext(), base.Owner.GetPower<StrengthPower>(), StackAdjust, null, null);
            }
            else
            {
                await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), base.Owner, StackAdjust, null, null);
            }

            if (base.Owner.GetPower<DexterityPower>() != null)
            {
                await PowerCmd.ModifyAmount(new ThrowingPlayerChoiceContext(), base.Owner.GetPower<DexterityPower>(), StackAdjust, null, null);
            }
            else
            {
                await PowerCmd.Apply<DexterityPower>(new ThrowingPlayerChoiceContext(), base.Owner, StackAdjust, null, null);
            }
            PlasmaFuelDebuffsAdded = 0;
        }
    }

    public override IEnumerable<HealthBarForecastSegment> GetHealthBarForecastSegments(
        HealthBarForecastContext context)
    {
        if (base.Owner.IsDead || base.Amount <= 0)
        {
            yield break;
        }

        int blockAmount = base.Owner.Block;
        int totalDamage = base.Amount - blockAmount;

        if (totalDamage > 0)
        {
            yield return new HealthBarForecastSegment(
                totalDamage,
                new Color(0.3f, 0.035f, 0.015f),
                HealthBarForecastDirection.FromRight,
                HealthBarForecastOrder.ForSideTurnStart(
                    context.Creature,
                    CombatSide.Enemy));
        }
        yield break;

    }

}
