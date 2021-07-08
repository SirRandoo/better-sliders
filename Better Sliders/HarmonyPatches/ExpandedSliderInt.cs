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

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using SirRandoo.BetterSliders.Helpers;
using UnityEngine;
using Verse;

namespace SirRandoo.BetterSliders.HarmonyPatches
{
    [HarmonyPatch]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class ExpandedSliderInt
    {
        private static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(Widgets), nameof(Widgets.IntRange));
        }

        [SuppressMessage("ReSharper", "RedundantAssignment")]
        private static void Prefix(ref Rect rect, [NotNull] ref ExpandedState __state)
        {
            __state = new ExpandedState {ShouldRender = ExpandedState.AlwaysOn || !UIHelper.IsRenderDisabled()};

            if (!__state.ShouldRender)
            {
                return;
            }

            GameFont cache = Text.Font;
            Text.Font = GameFont.Tiny;

            var gapRect = new Rect(rect.center.x - 5f, Text.LineHeight / 2f - 1f, 10f, 5f);

            float usedWidth = rect.width - gapRect.width - 10f;
            float distributedWidth = usedWidth / 4f;
            __state.MinimumDrawRect = new Rect(rect.x, rect.y, distributedWidth, Text.LineHeight);
            __state.MaximumDrawRect = new Rect(
                gapRect.x + gapRect.width + __state.MinimumDrawRect.width + 5f,
                rect.y,
                distributedWidth,
                Text.LineHeight
            );

            __state.ShouldFocusField = __state.MinimumDrawRect.WasClicked() || __state.MaximumDrawRect.WasClicked();
            Text.Font = cache;

            if (SliderSettings.DisplayStyleRaw.Equals(nameof(SliderSettings.Style.AlwaysOn)))
            {
                rect = new Rect(
                    __state.MinimumDrawRect.x + __state.MinimumDrawRect.width + 5f,
                    rect.y,
                    usedWidth - 10f,
                    rect.height
                );
            }
        }

        private static void Postfix(
            Rect rect,
            ref IntRange range,
            int min,
            int max,
            [NotNull] ref ExpandedState __state
        )
        {
            if (!ExpandedState.AlwaysOn && (!__state.ShouldRender || !Mouse.IsOver(rect)))
            {
                return;
            }

            if (Mouse.IsOver(__state.MinimumDrawRect) && __state.ShouldFocusField)
            {
                GUI.FocusControl($"TextField{__state.MinimumDrawRect.y:F0}{__state.MinimumDrawRect.x:F0}");
            }

            if (Mouse.IsOver(__state.MaximumDrawRect) && __state.ShouldFocusField)
            {
                GUI.FocusControl($"TextField{__state.MaximumDrawRect.y:F0}{__state.MaximumDrawRect.x:F0}");
            }

            GameFont cache = Text.Font;
            Text.Font = GameFont.Tiny;


            var minBuffer = range.min.ToString();
            var maxBuffer = range.max.ToString();
            UIHelper.Spinbox(__state.MinimumDrawRect, ref range.min, ref minBuffer, min, range.max);
            UIHelper.Spinbox(__state.MaximumDrawRect, ref range.max, ref maxBuffer, range.min, max);
            Text.Font = cache;
        }
    }
}
