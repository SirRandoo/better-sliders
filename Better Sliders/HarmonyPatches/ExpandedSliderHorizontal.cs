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
    public static class ExpandedSliderHorizontal
    {
        private static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(Widgets), nameof(Widgets.HorizontalSlider));
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

            float fieldWidth = rect.width / 5f;
            __state.HorizontalDrawRect = new Rect(
                rect.x + rect.width - fieldWidth,
                rect.y,
                fieldWidth,
                Text.LineHeight
            );
            __state.ShouldFocusField = __state.HorizontalDrawRect.WasClicked();
            Text.Font = cache;

            if (SliderSettings.DisplayStyleRaw.Equals(nameof(SliderSettings.Style.AlwaysOn)))
            {
                rect = new Rect(rect.x, rect.y, rect.width - __state.HorizontalDrawRect.width - 5f, rect.height);
            }
        }

        private static void Postfix(
            Rect rect,
            ref float __result,
            float leftValue,
            float rightValue,
            float roundTo,
            [NotNull] ref ExpandedState __state
        )
        {
            if (!ExpandedState.AlwaysOn && (!__state.ShouldRender || !Mouse.IsOver(rect)))
            {
                return;
            }

            if (__state.ShouldFocusField)
            {
                GUI.FocusControl($"TextField{__state.HorizontalDrawRect.y:F0}{__state.HorizontalDrawRect.x:F0}");
            }

            GameFont cache = Text.Font;
            Text.Font = GameFont.Tiny;

            var fieldBuffer = __result.ToString("0.0#########");
            UIHelper.DoubleSpinbox(
                __state.HorizontalDrawRect,
                ref __result,
                ref fieldBuffer,
                leftValue <= rightValue ? leftValue : rightValue,
                rightValue >= leftValue ? rightValue : leftValue
            );
            Text.Font = cache;

            if (roundTo > 0.0)
            {
                __result = Mathf.RoundToInt(__result / roundTo) * roundTo;
            }
        }
    }
}
