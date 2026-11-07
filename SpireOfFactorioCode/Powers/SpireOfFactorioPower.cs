using BaseLib.Abstracts;
using BaseLib.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SpireOfFactorio.Libraries.ConstructLib.Construct;
using SpireOfFactorio.SpireOfFactorioCode.Extensions;

namespace SpireOfFactorio.SpireOfFactorioCode.Powers;

public abstract class SpireOfFactorioPower : CustomPowerModel
{
    public override string CustomPackedIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
            return ResourceLoader.Exists(path) ? path : "power.png".PowerImagePath();
        }
    }

    public override string CustomBigIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
            return ResourceLoader.Exists(path) ? path : "power.png".BigPowerImagePath();
        }
    }

    public virtual Task OnConstructCreated(
        PlayerChoiceContext choiceContext,
        Creature construct,
        Player owner,
        ConstructCreationArgs args)
    {
        return Task.CompletedTask;
    }

    public virtual Task OnConstructPerformedTask(
        PlayerChoiceContext choiceContext,
        Creature construct,
        Player owner)
    {
        return Task.CompletedTask;
    }

    public virtual Task ModifyConstructTargetingList(
        Creature construct,
        ConstructTargetSelectionType targetingSelectionType,
        List<Creature> targetList)
    {
        return Task.CompletedTask;
    }

    public virtual Task ModifyConstructDamageAdditive(ConstructEffectContext context)
    {
        return Task.CompletedTask;
    }

    public virtual Task ModifyConstructDamageMultiplicative(ConstructEffectContext context)
    {
        return Task.CompletedTask;
    }

    public virtual Task ModifyConstructBlockAdditive(ConstructEffectContext context)
    {
        return Task.CompletedTask;
    }

    public virtual Task ModifyConstructBlockMultiplicative(ConstructEffectContext context)
    {
        return Task.CompletedTask;
    }

    public virtual Task ModifyConstructPowerList(ConstructEffectContext context)
    {
        return Task.CompletedTask;
    }

}