using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Timeline;

namespace SpireOfFactorio.Libraries.TimelineLib.Timeline;

public abstract class CustomEpochModel : EpochModel, ICustomModel
{
    public override string Id => $"{GetType().GetPrefix()}{StringHelper.Slugify(GetType().Name).ToUpperInvariant()}";

    public abstract string CustomRealPortraitPath { get; }

    public abstract string CustomPackedPortaitPath { get; }

    public virtual CustomEpochEra? CustomEra => null;

    protected virtual bool UnlocksContent => false;

}
