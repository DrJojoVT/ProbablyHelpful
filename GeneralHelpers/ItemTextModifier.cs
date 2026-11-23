using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public struct ItemTextModifier
{
    public readonly float fontSize;
    public readonly Color color;
    public readonly TextAlignmentOptions alignment;
    public readonly FontLoader.FontRole fontRole;

    public ItemTextModifier(float fontSize = 36f, Color color = default(Color), 
        TextAlignmentOptions alignment = TextAlignmentOptions.TopLeft, 
        FontLoader.FontRole fontRole = FontLoader.FontRole.Noto_Sans)
    {
        this.fontSize = fontSize;
        this.color = ((color == default(Color)) ? Color.white : color);
        this.alignment = alignment;
        this.fontRole = fontRole;
    }
}