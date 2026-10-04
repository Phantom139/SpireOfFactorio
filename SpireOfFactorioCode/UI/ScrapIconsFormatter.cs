using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SmartFormat.Core.Extensions;

namespace SpireOfFactorio.SpireOfFactorioCode.UI;

public class ScrapIconsFormatter : IAutoRegisterFormatSpecifier
{
    private const string _scrapIconPath = "res://SpireOfFactorio/images/charui/text_scrap.png";

    public const string scrapIconSprite = "[img]res://SpireOfFactorio/images/charui/text_scrap.png[/img]";

    public string Name
    {
        get
        {
            return "scrapIcons";
        }
        set
        {
            throw new NotImplementedException();
        }
    }

    public bool CanAutoDetect { get; set; }

    public bool TryEvaluateFormat(IFormattingInfo formattingInfo)
    {
        object currentValue = formattingInfo.CurrentValue;
        int num3;
        if (!(currentValue is DynamicVar dynamicVar))
        {
            if (!(currentValue is decimal num))
            {
                if (!(currentValue is int num2))
                {
                    throw new LocException($"Unknown value='{formattingInfo.CurrentValue}' type={formattingInfo.CurrentValue?.GetType()}");
                }

                num3 = num2;
            }
            else
            {
                num3 = (int)num;
            }
        }
        else
        {
            num3 = (int)dynamicVar.PreviewValue;
        }

        int count = num3;
        string text;
        if (count < 4)
        {
            text = string.Concat(Enumerable.Repeat("[img]res://SpireOfFactorio/images/charui/text_scrap.png[/img]", count));
        }
        else
        {
            text = string.Concat($"{count.ToString()} [img]res://SpireOfFactorio/images/charui/text_scrap.png[/img]");
        }
        formattingInfo.Write(text);
        return true;
    }
}

public class SingleScrapIconFormatter : IAutoRegisterFormatSpecifier
{
    private const string _scrapIconPath = "res://SpireOfFactorio/images/charui/text_scrap.png";

    public const string scrapIconSprite = "[img]res://SpireOfFactorio/images/charui/text_scrap.png[/img]";

    public string Name
    {
        get
        {
            return "singleScrapIcon";
        }
        set
        {
            throw new NotImplementedException();
        }
    }

    public bool CanAutoDetect { get; set; }

    public bool TryEvaluateFormat(IFormattingInfo formattingInfo)
    {
        string text;
        text = string.Concat($"[img]res://SpireOfFactorio/images/charui/text_scrap.png[/img]");
        formattingInfo.Write(text);
        return true;
    }
}