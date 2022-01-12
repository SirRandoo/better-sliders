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

namespace SirRandoo.BetterSliders
{
    public class SliderSettings : ModSettings
    {
        public enum Style { Hover, AlwaysOn }

        public static string DisplayStyleRaw = Style.Hover.ToString();

        public static List<FloatMenuOption> StyleOptions;
        public static float HeuristicsBeginDistance = 35f;
        public static float HeuristicsEndDistance = 110f;

        private static bool _isDrawingBeginHeuristics;
        private static bool _isDrawingEndHeuristics;
        private static bool _isViewingHeuristics;

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

            (Rect heuristicsViewLabel, Rect heuristicsViewField) = listing.Split();
            UiHelper.Label(heuristicsViewLabel, "BetterSliders.HeuristicsView.Label".TranslateSimple());

            if (_isViewingHeuristics)
            {
                PreviewHeuristicsFade(heuristicsViewField);
            }

            if (Widgets.ButtonText(
                heuristicsViewField,
                _isViewingHeuristics ? "BetterSliders.Heuristics.Hide".TranslateSimple() : "BetterSliders.Heuristics.View".TranslateSimple()
            ))
            {
                _isViewingHeuristics = !_isViewingHeuristics;
            }

            GUI.color = Color.white;

            (Rect heuristicsBeginLabel, Rect heuristicsBeginField) = listing.Split();
            UiHelper.Label(heuristicsBeginLabel, "BetterSliders.HeuristicsBegin.Label".TranslateSimple());

            if (Widgets.ButtonText(heuristicsBeginField, "BetterSliders.Heuristics.Calculate".TranslateSimple()))
            {
                _isDrawingBeginHeuristics = true;
            }

            (Rect heuristicsEndLabel, Rect heuristicsEndField) = listing.Split();
            UiHelper.Label(heuristicsEndLabel, "BetterSliders.HeuristicsEnd.Label".TranslateSimple());

            if (Widgets.ButtonText(heuristicsEndField, "BetterSliders.Heuristics.Calculate".TranslateSimple()))
            {
                _isDrawingEndHeuristics = true;
            }

            if (_isViewingHeuristics)
            {
                DrawHeuristicsInterface(heuristicsViewField.center);
            }

            if (_isDrawingBeginHeuristics)
            {
                DrawHeuristicsInterface(heuristicsBeginField.center);
            }

            if (_isDrawingEndHeuristics)
            {
                DrawHeuristicsInterface(heuristicsEndField.center);
            }

            if (_isDrawingBeginHeuristics || _isDrawingEndHeuristics || _isViewingHeuristics)
            {
                listing.Gap(6f);
                Text.Font = GameFont.Tiny;

                (Rect _, Rect heuristicsLegendField) = listing.Split();
                UiHelper.Label(heuristicsLegendField, "BetterSliders.HeuristicsLegend.Label".TranslateSimple(), TextAnchor.MiddleCenter);

                (Rect _, Rect heuristicsLegendBeginField) = listing.Split();
                UiHelper.Label(heuristicsLegendBeginField, "BetterSliders.HeuristicsLegend.Begin".TranslateSimple(), Color.yellow);

                (Rect _, Rect heuristicsLegendEndField) = listing.Split();
                UiHelper.Label(heuristicsLegendEndField, "BetterSliders.HeuristicsLegend.End".TranslateSimple(), Color.red);

                Text.Font = GameFont.Small;
            }

            listing.Label(HeuristicsBeginDistance.ToString("N0"));
            listing.Label(HeuristicsEndDistance.ToString("N0"));

            listing.End();
            GUI.EndGroup();
        }
        private static void PreviewHeuristicsFade(Rect region)
        {
            float distance = GenUI.DistFromRect(region, Event.current.mousePosition);

            if (distance >= HeuristicsEndDistance)
            {
                GUI.color = new Color(1f, 1f, 1f, 0f);

                return;
            }

            if (distance >= HeuristicsBeginDistance)
            {
                GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, Mathf.Clamp(1f - 1f * (distance / HeuristicsEndDistance), 0f, 1f));

                return;
            }

            if (distance <= HeuristicsBeginDistance)
            {
                GUI.color = Color.white;
            }
        }
        private static void DrawHeuristicsInterface(Vector2 position)
        {
            var beginningRect = new Rect(
                position.x - HeuristicsBeginDistance,
                position.y - HeuristicsBeginDistance,
                HeuristicsBeginDistance * 2f,
                HeuristicsBeginDistance * 2f
            );

            var endingRect = new Rect(position.x - HeuristicsEndDistance, position.y - HeuristicsEndDistance, HeuristicsEndDistance * 2f, HeuristicsEndDistance * 2f);

            GUI.color = Color.blue;
            Widgets.DrawBox(beginningRect);

            GUI.color = Color.yellow;
            GUI.DrawTexture(beginningRect, Textures.HeuristicsCircle);

            GUI.color = Color.red;
            GUI.DrawTexture(endingRect, Textures.HeuristicsCircle);

            GUI.color = Color.white;

            if (_isViewingHeuristics)
            {
                return;
            }

            if (_isDrawingBeginHeuristics)
            {
                HeuristicsBeginDistance = GenUI.DistFromRect(beginningRect, Event.current.mousePosition);

                if (InputHelper.AnyMouseButtonDown(MouseButtonCode.Left))
                {
                    _isDrawingBeginHeuristics = false;
                }
            }

            if (_isDrawingEndHeuristics)
            {
                HeuristicsEndDistance = GenUI.DistFromRect(endingRect, Event.current.mousePosition);

                if (InputHelper.AnyMouseButtonDown(MouseButtonCode.Left))
                {
                    _isDrawingEndHeuristics = false;
                }
            }
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref DisplayStyleRaw, "displayStyle", Style.Hover.ToString());
            Scribe_Values.Look(ref HeuristicsBeginDistance, "heuristicsBeginDistance", 35f);
            Scribe_Values.Look(ref HeuristicsEndDistance, "heuristicsEndDistance", 110f);
        }
        public static void PrepareState()
        {
            StyleOptions ??= Enum.GetNames(typeof(Style))
               .Select(s => new FloatMenuOption($"BetterSliders.DisplayStyles.{s}".TranslateSimple(), () => DisplayStyleRaw = s))
               .ToList();
        }
    }
}
