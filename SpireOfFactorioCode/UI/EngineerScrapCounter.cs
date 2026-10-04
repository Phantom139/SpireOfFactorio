using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using SpireOfFactorio.SpireOfFactorioCode.Characters;
using SpireOfFactorio.SpireOfFactorioCode.Util;

namespace SpireOfFactorio.SpireOfFactorioCode.UI
{
    public partial class EngineerScrapCounter : Control
    {
        private Control? _control;
        private Label? _scrapCount;
        private NEnergyCounter? _energyCounter;
        private Player? _player;
        private int _lastScrapCount = -1;
        private bool _wasMouseOverCounter;
        private bool _tooltipShown;

        public override void _Ready()
        {
            SetMouseFilterRecursive(this, MouseFilterEnum.Ignore);

            _control = FindControl("TextureRect");
            _scrapCount = FindCounterLabel("ScrapCount");
            if (GetParent() is NEnergyCounter energyCounter)
            {
                _energyCounter = energyCounter;
                _player = energyCounter._player;
            }
            if (!ShouldShowForPlayer(_player))
            {
                HideScrapTooptip();
                Visible = false;
                SetProcess(false);
                return;
            }
            Visible = true;
            SetProcess(true);
            Refresh(force: true);
        }

        public override void _ExitTree()
        {
            HideScrapTooptip();
        }

        public override void _Process(double delta)
        {
            if (!ShouldShowForPlayer(_player))
            {
                HideScrapTooptip();
                Visible = false;
                SetProcess(false);
                return;
            }

            Refresh(force: false);
            UpdateScrapHover();
        }

        private static bool ShouldShowForPlayer(Player? player)
        {
            if (player == null)
            {
                return false;
            }

            return player.Character is Engineer;
        }

        private static void SetMouseFilterRecursive(Node node, MouseFilterEnum mouseFilter)
        {
            if (node is Control control)
            {
                control.MouseFilter = mouseFilter;
            }

            foreach (Node child in node.GetChildren())
            {
                SetMouseFilterRecursive(child, mouseFilter);
            }
        }

        private Control? FindControl(string name)
        {
            return GetNodeOrNull<Control>(name)
                   ?? GetNodeOrNull<Control>($"Visual/{name}");
        }

        private Label? FindCounterLabel(string path)
        {
            return GetNodeOrNull<Label>(path)
                   ?? GetNodeOrNull<Label>($"Visual/{path}");
        }

        private void Refresh(bool force)
        {
            if (_player == null)
                return;

            var scrap = CustomResources<Scrap>.Get(_player.PlayerCombatState);
            int currentScrap = scrap.Amount;
            UpdateLabel(_scrapCount, currentScrap, ref _lastScrapCount, force);
        }

        private static void UpdateLabel(Label? label, int amount, ref int lastAmount, bool force)
        {
            if (label == null)
            {
                return;
            }

            if (!force && amount == lastAmount)
            {
                return;
            }

            lastAmount = amount;
            label.Text = amount.ToString();
        }

        private void UpdateScrapHover()
        {
            bool isMouseOverMaterial = IsMouseOverMaterialCounter();

            if (isMouseOverMaterial == _wasMouseOverCounter)
            {
                return;
            }

            _wasMouseOverCounter = isMouseOverMaterial;

            if (isMouseOverMaterial)
            {
                ShowScrapTooltip();
            }
            else
            {
                HideScrapTooptip();
            }
        }

        public bool IsMouseOverMaterialCounter()
        {
            Vector2 mouse = GetGlobalMousePosition();

            return IsMouseOverControl(_control, mouse);
        }

        private static bool IsMouseOverControl(Control? control, Vector2 mouse)
        {
            if (control == null || !GodotObject.IsInstanceValid(control))
            {
                return false;
            }

            return control.GetGlobalRect().Grow(4f).HasPoint(mouse);
        }

        private void ShowScrapTooltip()
        {
            if (_player == null)
            {
                return;
            }

            if (!ShouldShowForPlayer(_player))
            {
                return;
            }

            if (_tooltipShown)
            {
                return;
            }

            _tooltipShown = true;

            // Important:
            // Clean stale entry for this owner before showing.
            NHoverTipSet.Remove(this);

            // Also remove the energy tooltip, because the material UI overlaps it.
            if (_energyCounter != null && GodotObject.IsInstanceValid(_energyCounter))
            {
                NHoverTipSet.Remove(_energyCounter);
            }

            HoverTip hoverTip = BuildHoverTip();

            NHoverTipSet.CreateAndShow(this, hoverTip)
                ?.SetGlobalPosition(GlobalPosition + new Vector2(-70f, -200f));
        }

        private void HideScrapTooptip()
        {
            _tooltipShown = false;

            NHoverTipSet.Remove(this);

            if (_energyCounter == null || !GodotObject.IsInstanceValid(_energyCounter))
            {
                return;
            }

            if (!IsMouseOverEnergyCounter())
            {
                return;
            }

            // Let Material fully unregister first, then let Energy try to show again.
            _energyCounter.CallDeferred(nameof(NEnergyCounter.OnHovered));
        }

        private bool IsMouseOverEnergyCounter()
        {
            if (_energyCounter == null || !GodotObject.IsInstanceValid(_energyCounter))
                return false;

            return _energyCounter.GetGlobalRect().Grow(4f).HasPoint(GetGlobalMousePosition());
        }

        private HoverTip BuildHoverTip()
        {
            LocString title = new(
                "static_hover_tips",
                "SPIREOFFACTORIO-SCRAP_COUNT.title");

            LocString description = new(
                "static_hover_tips",
                "SPIREOFFACTORIO-SCRAP_COUNT.description");

            var scrap = CustomResources<Scrap>.Get(_player.PlayerCombatState);
            int currentScrap = scrap.Amount;

            description.Add("Scrap", currentScrap.ToString());

            return new HoverTip(title, description);
        }

    }
}
