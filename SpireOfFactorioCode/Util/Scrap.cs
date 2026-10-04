using BaseLib.Abstracts;
using BaseLib.BaseLibScenes;
using BaseLib.Patches.UI;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace SpireOfFactorio.SpireOfFactorioCode.Util;

public sealed class Scrap : BasicCustomResource
{
    public Scrap() : base("SCRAP")
    {

    }

    public override string TexturePath => "res://SpireOfFactorio/images/charui/text_scrap.png";

    public override void RegisterResourceVisuals<T>()
    {
        ValidateType<T>();
        /**
        ExtraCombatUi.RegisterCombatUiElement(static (ui, player, combatState) =>
        {
            var playerCombatState = player.PlayerCombatState;
            if (playerCombatState == null) return null;

            if (CustomResources<Scrap>.Get(playerCombatState) is not BasicCustomResource resource) return null;

            var tex = PreloadManager.Cache.GetTexture2D(resource.TexturePath);
            var display = NAdditionalResourceDisplay.Create<T>(player, resource, tex);

            ui.AddChild(display);

            return display;
        }, ExtraCombatUi.CombatUiPositioning.AroundEnergy);
        **/

        var getDisplay = ExtraCardUi.RegisterCreateCardUiElement(ExtraCardUi.CardUiPositioning.AroundCost,
            _ => new NAdditionalCostDisplay(Id, TexturePath, MainColor),
            (card, model, display) =>
            {
                if (model == null) return false;
                var cost = CustomResources<Scrap>.Cost(model);
                if (cost == null) return false;

                display.UpdateCostVisual(card, cost, PileType.None);
                return true;
            });

        CustomResources<Scrap>.UpdateCostVisuals += (card, cost, pileType) =>
        {
            getDisplay(card).UpdateCostVisual(card, cost, pileType);
        };
    }

    /**
    public static CardCostColor GetScrapCostColor(CardModel card, ICombatState? state)
    {
        if (state == null)
        {
            return CardCostColor.Unmodified;
        }

        if (card is not EngineerCard engineerCard)
        {
            return CardCostColor.Unmodified;
        }
        var scrap = CustomResources<Scrap>.Cost(engineerCard);
        if (scrap == null)
        {
            return CardCostColor.Unmodified;
        }

        if (!card.CanPlay(out UnplayableReason reason, out AbstractModel _) && reason.HasFlag(UnplayableReason.StarCostTooHigh))
        {
            return CardCostColor.InsufficientResources;
        }

        if (scrap.CostsX)
        {
            return CardCostColor.Unmodified;
        }

        // Phantom139: Implement later or hopefully BaseLib will do so.
        //if (TryModifyScrapCostWithHooks(card, state, out var hookModifiedCost))
        //{
        //    return CardCostHelper.GetColorForHookModifiedCost(hookModifiedCost, card.BaseStarCost);
        //}
        //if (card.TemporaryStarCost != null)
        //{
        //    return CardCostHelper.GetColorForLocalCost(card.TemporaryStarCost.Cost, card.BaseStarCost);
        //}

        return CardCostColor.Unmodified;
    }
    **/

}

public class ScrapVar : DynamicVar
{
    public const string defaultName = "Scrap";

    public ScrapVar(int scrap)
        : this("Scrap", scrap)
    {
    }

    public ScrapVar(string name, int scrap)
        : base(name, scrap)
    {
    }
}