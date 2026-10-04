//using BaseLib.Abstracts;
//using BaseLib.Utils;
//using Godot;
//using MegaCrit.Sts2.addons.mega_text;
//using MegaCrit.Sts2.Core.Entities.Cards;
//using MegaCrit.Sts2.Core.Entities.UI;
//using MegaCrit.Sts2.Core.Helpers;
//using MegaCrit.Sts2.Core.Nodes.Cards;
//using SpireOfFactorio.SpireOfFactorioCode.Cards;
//using SpireOfFactorio.SpireOfFactorioCode.Util;

//namespace SpireOfFactorio.SpireOfFactorioCode.UI;

/**
public partial class CardScrapCostDisplay : Control
{
    private NCard _card = null!;
    private MegaLabel _label = null!;
    private TextureRect _textRect = null!;

    public static AddedNode<NCard, CardScrapCostDisplay> Node = new((card) =>
    {
        var cardContainer = card.GetChild(0)!;

        var control = new CardScrapCostDisplay
        {
            Name = "CardScrapCostDisplay"
        };

        control._card = card;

        // Texture
        var tex = ResourceLoader.Load<Texture2D>(
            "res://SpireOfFactorio/images/charui/text_scrap.png");

        control._textRect = new TextureRect
        {
            Name = tex.ResourcePath,
            Size = new Vector2(50, 50),
            Texture = tex,
            PivotOffset = tex.GetSize() / 2f,
            ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
            StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
            MouseFilter = MouseFilterEnum.Ignore
        };

        control.Size = new Vector2(50, 50);
        control.Position = new Vector2(-126, -231);
        control.AddChild(control._textRect);

        // Fetch the star cost label
        var _starLabel = cardContainer.GetNode<MegaLabel>("%StarLabel");
        // Label
        control._label = (MegaLabel)_starLabel.Duplicate();
        control._label.SetAnchorsAndOffsetsPreset(LayoutPreset.Center);
        control.AddChild(control._label);

        // Add to CardContainer
        cardContainer.AddChild(control);

        cardContainer.MoveChild(
            control,
            cardContainer.GetNode("%StarIcon").GetIndex());

        //control.UpdateScrapCostVisuals(null);

        return control;
    });

    public ModelVisibility Visibility
    {
        get
        {
            return _visibility;
        }
        set
        {
            if (_visibility != value)
            {
                _visibility = value;
                Reload();
            }
        }
    }
    public ModelVisibility _visibility = ModelVisibility.Visible;


    public void UpdateScrapCost(PileType ?pileType, CardPreviewMode ?previewMode)
    {
        if (_card.Model is not EngineerCard engineerCard)
        {
            Visible = false;
            return;
        }

        var scrap = CustomResources<Scrap>.Cost(engineerCard);

        if (scrap == null)
        {
            Visible = false;
            return;
        }

        Visible = true;
        _label.Text = scrap.CostsX
            ? "X"
            : scrap.Canonical.ToString();
    }

    public void UpdateScrapCostVisuals(PileType pileType)
    {
        if (_card.Model is not EngineerCard engineerCard)
        {
            _textRect.Visible = false;
            Visible = false;
            return;
        }
        var scrap = CustomResources<Scrap>.Cost(engineerCard);
        if (scrap == null)
        {
            Visible = false;
            return;
        }

        if (Visibility != ModelVisibility.Visible)
        {
            _label.SetTextAutoSize(string.Empty);
            _textRect.Visible = false;
            Visible = false;
            _label.AddThemeColorOverride(ThemeConstants.Label.FontColor, StsColors.cream);
            _label.AddThemeColorOverride(ThemeConstants.Label.FontOutlineColor, _card.Model.Pool.EnergyOutlineColor);
            return;
        }
        if (scrap.CostsX)
        {
            _label.SetTextAutoSize("X");
            _textRect.Visible = true;
            Visible = true;
        }
        else
        {
            _label.SetTextAutoSize(scrap.GetWithModifiers(CostModifiers.All).ToString());
            _textRect.Visible = scrap.GetWithModifiers(CostModifiers.All) >= 0;
            Visible = scrap.GetWithModifiers(CostModifiers.All) >= 0;
        }

        // Remove anything that is "-1"
        if (scrap.Canonical == -1)
        {
            _textRect.Visible = false;
            Visible = false;
            return;
        }

        //Debug
        //MainFile.Logger.Info($"[CardScrapCostDisplay:UpdateScrapCostVisuals] {_card.Model.Id} - Cost Internal: {scrap.Canonical.ToString()}, is-X? {scrap.CostsX}");

        UpdateScrapCostColor(pileType);
        //if (pileType == PileType.Hand && !_card.Model.CanPlay(out UnplayableReason reason, out AbstractModel _))
        //{
        //   _unplayableStarIcon.Visible = !reason.HasResourceCostReason();
        //}
        //else
        //{
        //    _unplayableStarIcon.Visible = false;
        //}
    }

    public void UpdateScrapCostText(int cost)
    {
        if (_card.Model is not EngineerCard engineerCard)
        {
            Visible = false;
            return;
        }
        var scrap = CustomResources<Scrap>.Cost(engineerCard);
        if (scrap == null)
        {
            Visible = false;
            return;
        }
        if (scrap.CostsX)
        {
            _label.SetTextAutoSize("X");
            _textRect.Visible = true;
            Visible = true;
        }
        else if (cost >= 0)
        {
            _label.SetTextAutoSize(cost.ToString());
            _textRect.Visible = true;
            Visible = true;
        }
        else
        {
            _textRect.Visible = false;
            Visible = false;
        }
    }

    public void UpdateScrapCostColor(PileType pileType)
    {
        if (_card.Model is not EngineerCard engineerCard)
        {
            return;
        }
        var scrap = CustomResources<Scrap>.Cost(engineerCard);
        if (scrap == null)
        {
            return;
        }
        Color color = StsColors.cream;
        Color color2 = StsColors.defaultStarCostOutline;
        if (!scrap.CostsX && scrap.WasJustUpgraded)
        {
            color = StsColors.green;
            color2 = StsColors.energyGreenOutline;
        }
        else if (pileType == PileType.Hand)
        {
            CardCostColor scrapCostColor = Scrap.GetScrapCostColor(_card.Model, _card.Model.CombatState);
            color = NCard.GetCostTextColorInHand(scrapCostColor, _card._pretendCardCanBePlayed, color);
            color2 = NCard.GetCostOutlineColorInHand(scrapCostColor, _card._pretendCardCanBePlayed, color2);
        }

        _label.AddThemeColorOverride(ThemeConstants.Label.FontColor, color);
        _label.AddThemeColorOverride(ThemeConstants.Label.FontOutlineColor, color2);
    }

    public void Reload()
    {

    }

}
**/