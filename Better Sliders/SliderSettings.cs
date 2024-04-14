// MIT License
// 
// Copyright (c) 2021 SirRandoo
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

using System;
using System.Collections.Generic;
using System.Linq;
using SirRandoo.BetterSliders.Helpers;
using UnityEngine;
using Verse;

namespace SirRandoo.BetterSliders;

public class SliderSettings : ModSettings
{
    public enum Style { Hover, AlwaysOn }

    public static string DisplayStyleRaw = Style.Hover.ToString();

    public static List<FloatMenuOption> StyleOptions;
    public static float HysteresisBeginDistance = 35f;
    public static float HysteresisEndDistance = 110f;

    private static bool _isDrawingBeginHysteresis;
    private static bool _isDrawingEndHysteresis;
    private static bool _isViewingHysteresis;

    public static bool IsAlwaysOn => DisplayStyleRaw == nameof(Style.AlwaysOn);

    public static void Draw(Rect region)
    {
        GUI.BeginGroup(region);

        var listing = new Listing_Standard();
        listing.Begin(region.AtZero());

        (Rect styleLabel, Rect styleField) = listing.Split();
        UiHelper.Label(styleLabel, "BetterSliders.DisplayStyle.Label".TranslateSimple());

        if (Widgets.ButtonText(styleField, $"BetterSliders.DisplayStyles.{DisplayStyleRaw}".TranslateSimple()))
        {
            Find.WindowStack.Add(new FloatMenu(StyleOptions));
        }

        (Rect hysteresisViewLabel, Rect hysteresisViewField) = listing.Split();
        UiHelper.Label(hysteresisViewLabel, "BetterSliders.HysteresisView.Label".TranslateSimple());

        if (_isViewingHysteresis)
        {
            PreviewHysteresisFade(hysteresisViewField);
        }

        if (Widgets.ButtonText(
            hysteresisViewField,
            _isViewingHysteresis ? "BetterSliders.Hysteresis.Hide".TranslateSimple() : "BetterSliders.Hysteresis.View".TranslateSimple()
        ))
        {
            _isViewingHysteresis = !_isViewingHysteresis;
        }

        GUI.color = Color.white;

        (Rect hysteresisBeginLabel, Rect hysteresisBeginField) = listing.Split();
        UiHelper.Label(hysteresisBeginLabel, "BetterSliders.HysteresisBegin.Label".TranslateSimple());

        if (Widgets.ButtonText(hysteresisBeginField, "BetterSliders.Hysteresis.Calculate".TranslateSimple()))
        {
            _isDrawingBeginHysteresis = true;
        }

        (Rect hysteresisEndLabel, Rect hysteresisEndField) = listing.Split();
        UiHelper.Label(hysteresisEndLabel, "BetterSliders.HysteresisEnd.Label".TranslateSimple());

        if (Widgets.ButtonText(hysteresisEndField, "BetterSliders.Hysteresis.Calculate".TranslateSimple()))
        {
            _isDrawingEndHysteresis = true;
        }

        if (_isViewingHysteresis)
        {
            DrawHysteresisInterface(hysteresisViewField.center);
        }

        if (_isDrawingBeginHysteresis)
        {
            DrawHysteresisInterface(hysteresisBeginField.center);
        }

        if (_isDrawingEndHysteresis)
        {
            DrawHysteresisInterface(hysteresisEndField.center);
        }

        if (_isDrawingBeginHysteresis || _isDrawingEndHysteresis || _isViewingHysteresis)
        {
            listing.Gap(6f);
            Text.Font = GameFont.Tiny;

            (Rect _, Rect hysteresisLegendField) = listing.Split();
            UiHelper.Label(hysteresisLegendField, "BetterSliders.HysteresisLegend.Label".TranslateSimple(), TextAnchor.MiddleCenter);

            (Rect _, Rect hysteresisLegendBeginField) = listing.Split();
            UiHelper.Label(hysteresisLegendBeginField, "BetterSliders.HysteresisLegend.Begin".TranslateSimple(), Color.yellow);

            (Rect _, Rect hysteresisLegendEndField) = listing.Split();
            UiHelper.Label(hysteresisLegendEndField, "BetterSliders.HysteresisLegend.End".TranslateSimple(), Color.red);

            Text.Font = GameFont.Small;
        }

        listing.End();
        GUI.EndGroup();
    }

    private static void PreviewHysteresisFade(Rect region)
    {
        float distance = GenUI.DistFromRect(region, Event.current.mousePosition);

        if (distance >= HysteresisEndDistance)
        {
            GUI.color = new Color(1f, 1f, 1f, 0f);

            return;
        }

        if (distance >= HysteresisBeginDistance)
        {
            GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, Mathf.Clamp(1f - 1f * (distance / HysteresisEndDistance), 0f, 1f));

            return;
        }

        if (distance <= HysteresisBeginDistance)
        {
            GUI.color = Color.white;
        }
    }

    private static void DrawHysteresisInterface(Vector2 position)
    {
        var beginningRect = new Rect(
            position.x - HysteresisBeginDistance,
            position.y - HysteresisBeginDistance,
            HysteresisBeginDistance * 2f,
            HysteresisBeginDistance * 2f
        );

        var endingRect = new Rect(position.x - HysteresisEndDistance, position.y - HysteresisEndDistance, HysteresisEndDistance * 2f, HysteresisEndDistance * 2f);

        GUI.color = Color.blue;
        Widgets.DrawBox(beginningRect);

        GUI.color = Color.yellow;
        GUI.DrawTexture(beginningRect, Textures.HysteresisCircle);

        GUI.color = Color.red;
        GUI.DrawTexture(endingRect, Textures.HysteresisCircle);

        GUI.color = Color.white;

        if (_isViewingHysteresis)
        {
            return;
        }

        if (_isDrawingBeginHysteresis)
        {
            HysteresisBeginDistance = GenUI.DistFromRect(beginningRect, Event.current.mousePosition);

            if (InputHelper.AnyMouseButtonDown(MouseButtonCode.Left))
            {
                _isDrawingBeginHysteresis = false;
            }
        }

        if (_isDrawingEndHysteresis)
        {
            HysteresisEndDistance = GenUI.DistFromRect(endingRect, Event.current.mousePosition);

            if (InputHelper.AnyMouseButtonDown(MouseButtonCode.Left))
            {
                _isDrawingEndHysteresis = false;
            }
        }
    }

    public override void ExposeData()
    {
        Scribe_Values.Look(ref DisplayStyleRaw, "displayStyle", Style.Hover.ToString());
        Scribe_Values.Look(ref HysteresisBeginDistance, "hysteresisBeginDistance", 35f);
        Scribe_Values.Look(ref HysteresisEndDistance, "hysteresisEndDistance", 110f);
    }

    public static void PrepareState()
    {
        StyleOptions ??= Enum.GetNames(typeof(Style))
           .Select(s => new FloatMenuOption($"BetterSliders.DisplayStyles.{s}".TranslateSimple(), () => DisplayStyleRaw = s))
           .ToList();
    }
}
