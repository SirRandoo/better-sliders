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
using UnityEngine;
using Verse;

namespace SirRandoo.BetterSliders
{
    public class SliderSettings : ModSettings
    {
        public enum Style { Hover, AlwaysOn }

        public static string DisplayStyleRaw = Style.Hover.ToString();

        public static List<FloatMenuOption> StyleOptions;

        public static void Draw(Rect region)
        {
            StyleOptions ??= Enum.GetNames(typeof(Style))
               .Select(
                    s => new FloatMenuOption(
                        $"BetterSliders.DisplayStyles.{s}".TranslateSimple(),
                        () => DisplayStyleRaw = s
                    )
                )
               .ToList();

            var styleLabel = new Rect(0f, 0f, Mathf.CeilToInt(region.width * 0.7f), Text.LineHeight);
            var styleField = new Rect(styleLabel.width + 5f, 0f, region.width - styleLabel.width - 5f, Text.LineHeight);

            GUI.BeginGroup(region);

            Widgets.Label(styleLabel, "BetterSliders.DisplayStyle.Label".TranslateSimple());

            if (Widgets.ButtonText(styleField, $"BetterSliders.DisplayStyles.{DisplayStyleRaw}".TranslateSimple()))
            {
                Find.WindowStack.Add(new FloatMenu(StyleOptions));
            }

            GUI.EndGroup();
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref DisplayStyleRaw, "displayStyle", Style.Hover.ToString());
        }
    }
}
