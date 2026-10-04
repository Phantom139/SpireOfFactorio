using BaseLib.Abstracts;
using Godot;
using SpireOfFactorio.SpireOfFactorioCode.Extensions;

namespace SpireOfFactorio.SpireOfFactorioCode.Characters;

public class EngineerPotionPool : CustomPotionPoolModel
{
    public override Color LabOutlineColor => Engineer.Color;

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}