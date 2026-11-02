using Godot;
using HarmonyLib;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.Screens.Timeline;
using MegaCrit.Sts2.Core.Timeline;
using System.Reflection;

namespace SpireOfFactorio.Libraries.TimelineLib.Timeline;

public abstract class CustomEpochEra
{
    // Points to a new CustomEnum EpochEra
    public abstract EpochEra CustomEra { get; }

    // The reference era obtained from Location and Direction
    public abstract EpochEra ReferenceEra { get; }

    // Which direction to insert the era relative to the Location parameter
    public abstract RelativeEraDirection Direction { get; }

    // The relative position of the era on the timeline.
    // The library considers each mod as a unique entity and then sets up ordering based on the parameter here
    // Other mods with the same value are ignored, this is all relative to the start of the timeline for each individual mod
    public virtual int Location => 0;

    // The icon displayed on the timeline
    public abstract string EraIconPath { get; }
    public Texture2D EraIconTexture => PreloadManager.Cache.GetTexture2D(EraIconPath);

    // Force the game to use the original texture size instead of resizing the image.
    public virtual bool UseOriginalImageSize => false;

    // Stop the game from applying tint to the era icon.
    public virtual bool DisableTinting => false;

    [HarmonyPatch]
    private static class Patches
    {
        private static readonly FieldInfo? NEraCustomIconField = AccessTools.Field(typeof(NEraColumn), "_icon");
        private static readonly FieldInfo? NEraCustomNameField = AccessTools.Field(typeof(NEraColumn), "_name");
        private static readonly FieldInfo? NEraCustomYearField = AccessTools.Field(typeof(NEraColumn), "_year");

        [HarmonyPostfix]
        [HarmonyPatch(typeof(NEraColumn), nameof(NEraColumn.Init))]
        private static void NEraColumnInitPostfix(NEraColumn __instance, EpochSlotData epochSlot)
        {
            // Check to ensure all the parameters needed are set
            if (epochSlot.Model is not CustomEpochModel customEpochModel)
            {
                return;
            }
            if (customEpochModel.CustomEra is null)
            {
                return;
            }
            if (NEraCustomIconField?.GetValue(__instance) is not TextureRect textureRect)
            {
                return;
            }
            if (NEraCustomNameField?.GetValue(__instance) is not MegaLabel name)
            {
                return;
            }
            if (NEraCustomYearField?.GetValue(__instance) is not MegaLabel year)
            {
                return;
            }
            if (textureRect.Texture is null)
            {
                // Start by enabling the texture, if it fails after the load method we can then disable it.
                textureRect.Visible = true;
            }
            textureRect.Texture = customEpochModel.CustomEra.EraIconTexture;
            if (customEpochModel.CustomEra.UseOriginalImageSize)
            {
                var oSize = customEpochModel.CustomEra.EraIconTexture.GetSize();
                textureRect.Size = oSize;
                textureRect.Position -= (oSize / 2) - new Vector2(24, 24);
                // Large textures overlap the bottom epochs
                textureRect.MouseFilter = Control.MouseFilterEnum.Ignore;
            }
            if (customEpochModel.CustomEra.DisableTinting)
            {
                textureRect.Modulate = new Color("ffffff");
            }
            var slugifiedName = StringHelper.Slugify(customEpochModel.CustomEra.GetType().Name);
            name.SetTextAutoSize(new LocString("eras", slugifiedName + ".name").GetFormattedText());
            year.SetTextAutoSize(new LocString("eras", slugifiedName + ".year").GetFormattedText());
        }
    }
}


public enum RelativeEraDirection
{
    // Should not be used, primarily here to catch errors
    None,
    // Insert to the left
    Before,
    // Insert to the right
    After,
}