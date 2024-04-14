// MIT License
//
// Copyright (c) 2022 SirRandoo
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using UnityEngine;
using Verse;

namespace SirRandoo.BetterSliders.Helpers;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public static class UiHelper
{
    private const float EntryButtonWidth = 50f;
    private static readonly Color ExperimentalNoticeColor = new(1f, 0.53f, 0.76f);
    private static readonly Color DescriptionTextColor = new(0.72f, 0.72f, 0.72f);

    /// <summary>
    ///     Draws a stateful number field.
    /// </summary>
    /// <param name="region">The region to draw the number field in</param>
    /// <param name="buffer">
    ///     A <see cref="string" /> that houses the user's raw input
    /// </param>
    /// <param name="value">
    ///     The numerical value of the user's input if it's valid
    /// </param>
    /// <param name="bufferValid">
    ///     Whether or not the <see cref="buffer" /> is a valid number
    /// </param>
    /// <param name="minimum">The minimum number <see cref="value" /> can be</param>
    /// <param name="maximum">The maximum value <see cref="value" /> can be</param>
    public static void NumberField(Rect region, ref string buffer, ref int value, ref bool bufferValid, int minimum = 0, int maximum = int.MaxValue)
    {
        GUI.backgroundColor = bufferValid ? Color.white : Color.red;

        if (TextField(region, buffer, out string newBuffer))
        {
            buffer = newBuffer;

            if (int.TryParse(buffer, out int result))
            {
                value = Mathf.Clamp(result, minimum, maximum);
                bufferValid = true;
            }
            else
            {
                bufferValid = false;
            }
        }

        GUI.backgroundColor = Color.white;
    }

    /// <summary>
    ///     Draws a stateful number field.
    /// </summary>
    /// <param name="region">The region to draw the number field in</param>
    /// <param name="buffer">
    ///     A <see cref="string" /> that houses the user's raw input
    /// </param>
    /// <param name="value">
    ///     The numerical value of the user's input if it's valid
    /// </param>
    /// <param name="bufferValid">
    ///     Whether or not the <see cref="buffer" /> is a valid number
    /// </param>
    /// <param name="minimum">The minimum number <see cref="value" /> can be</param>
    /// <param name="maximum">The maximum value <see cref="value" /> can be</param>
    public static bool NumberField(Rect region, ref string buffer, ref float value, ref bool bufferValid, float minimum = 0f, float maximum = float.MaxValue)
    {
        var wasChanged = false;
        GUI.backgroundColor = bufferValid ? Color.white : Color.red;

        if (TextField(region, buffer, out string newBuffer))
        {
            buffer = newBuffer;
            wasChanged = true;

            if (float.TryParse(buffer, out float result))
            {
                value = Mathf.Clamp(result, minimum, maximum);
                bufferValid = true;
            }
            else
            {
                bufferValid = false;
            }
        }

        GUI.backgroundColor = Color.white;

        return wasChanged;
    }

    /// <summary>
    ///     Draws a label at the given region.
    /// </summary>
    /// <param name="region">The region to draw the label in</param>
    /// <param name="text">The text of the label</param>
    /// <param name="anchor">The anchor of the label</param>
    /// <param name="fontScale">The font scale of the label</param>
    /// <param name="vertical">Whether or not to draw the label vertically</param>
    public static void Label(Rect region, string text, TextAnchor anchor = TextAnchor.MiddleLeft, GameFont fontScale = GameFont.Small, bool vertical = false)
    {
        Text.Anchor = anchor;
        Text.Font = fontScale;

        if (vertical)
        {
            region.y += region.width;
            GUIUtility.RotateAroundPivot(-90f, region.position);
        }

        Widgets.Label(region, text);

        if (vertical)
        {
            GUI.matrix = Matrix4x4.identity;
        }

        Text.Anchor = TextAnchor.UpperLeft;
        Text.Font = GameFont.Small;
    }

    /// <summary>
    ///     Draws a label at the given region.
    /// </summary>
    /// <param name="region">The region to draw the label in</param>
    /// <param name="text">The text of the label</param>
    /// <param name="color">The color of the label's text</param>
    /// <param name="anchor">The anchor of the label</param>
    /// <param name="fontScale">The font scale of the label</param>
    /// <param name="vertical">Whether or not to draw the label vertically</param>
    public static void Label(Rect region, string text, Color color, TextAnchor anchor = TextAnchor.MiddleLeft, GameFont fontScale = GameFont.Small, bool vertical = false)
    {
        GUI.color = color;
        Label(region, text, anchor, fontScale, vertical);
        GUI.color = Color.white;
    }

    /// <summary>
    ///     Draws a text field that notifies when its contents were changed.
    /// </summary>
    /// <param name="region">The region to draw the text field in</param>
    /// <param name="content">The text within the field</param>
    /// <param name="newContent">The changed text</param>
    /// <returns>Whether or not the text field was changed</returns>
    [ContractAnnotation("=> true,newContent:notnull; => false,newContent:null")]
    public static bool TextField(Rect region, string content, out string newContent)
    {
        string text = Widgets.TextField(region, content);
        newContent = string.Equals(text, content) ? null : text;

        return newContent != null;
    }

    /// <summary>
    ///     A convenience method for tipping a region and drawing
    ///     a mouse over highlight in said region.
    /// </summary>
    /// <param name="region">The region to tip</param>
    /// <param name="tooltip">The tooltip of the region</param>
    public static void TipRegion(this Rect region, string tooltip)
    {
        Widgets.DrawHighlightIfMouseover(region);
        TooltipHandler.TipRegion(region, tooltip);
    }

    /// <summary>
    ///     Draws an experimental notice at the region specified.
    /// </summary>
    /// <param name="listing">The <see cref="Listing" /> object use for layout</param>
    public static void DrawExperimentalNotice([NotNull] this Listing listing)
    {
        listing.DrawDescription("ExperimentalContent".TranslateSimple(), ExperimentalNoticeColor);
    }

    /// <summary>
    ///     Draws text suitable for content descriptions.
    /// </summary>
    /// <param name="listing">The <see cref="Listing" /> object use for layout</param>
    /// <param name="text">The description of the content</param>
    /// <param name="color">The color of the description text</param>
    /// <param name="anchor">The text anchor of the description text</param>
    public static void DrawDescription([NotNull] this Listing listing, string text, Color color, TextAnchor anchor = TextAnchor.UpperLeft)
    {
        GameFont fontCache = Text.Font;
        GUI.color = color;
        Text.Font = GameFont.Tiny;
        float width = listing.ColumnWidth * 0.7f;
        float height = Text.CalcHeight(text, width);
        Rect lineRect = listing.GetRect(height);
        var labelRect = new Rect(lineRect.x + 10f, lineRect.y, lineRect.width - 10 - width, lineRect.height);

        Label(labelRect, text, anchor, GameFont.Tiny);

        GUI.color = Color.white;
        Text.Font = fontCache;

        listing.Gap(6f);
    }

    /// <summary>
    ///     Draws text suitable for content descriptions.
    /// </summary>
    /// <param name="listing">The <see cref="Listing" /> object use for layout</param>
    /// <param name="text">The description of the content</param>
    /// <param name="anchor">The text anchor of the description text</param>
    public static void DrawDescription([NotNull] this Listing listing, string text, TextAnchor anchor)
    {
        DrawDescription(listing, text, DescriptionTextColor, anchor);
    }

    /// <summary>
    ///     Draws text suitable for content descriptions.
    /// </summary>
    /// <param name="listing">The <see cref="Listing" /> object use for layout</param>
    /// <param name="text">The description of the content</param>
    public static void DrawDescription([NotNull] this Listing listing, string text)
    {
        DrawDescription(listing, text, DescriptionTextColor);
    }

    /// <summary>
    ///     Draws a grouping header for the given content.
    /// </summary>
    /// <param name="listing">The <see cref="Listing" /> object use for layout</param>
    /// <param name="name">The name of the group header</param>
    /// <param name="gapPrefix">
    ///     Whether or not to prepend a gap before the group header
    /// </param>
    public static void GroupHeader([NotNull] this Listing listing, string name, bool gapPrefix = true)
    {
        if (gapPrefix)
        {
            listing.Gap(Mathf.CeilToInt(Text.LineHeight * 1.25f));
        }

        Label(listing.GetRect(Text.LineHeight), name, TextAnchor.LowerLeft, GameFont.Tiny);
        listing.GapLine(6f);
    }
}
