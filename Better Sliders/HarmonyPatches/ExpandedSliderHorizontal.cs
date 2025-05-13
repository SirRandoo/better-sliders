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
using System.Net.NetworkInformation;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using SirRandoo.BetterSliders.Entities;
using UnityEngine;
using Verse;

namespace SirRandoo.BetterSliders.HarmonyPatches;

[HarmonyPatch]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public static class ExpandedSliderHorizontal
{
    private static IEnumerable<MethodBase> TargetMethods()
    {
        yield return AccessTools.Method(
            typeof(Widgets),
            nameof(Widgets.HorizontalSlider),
            [typeof(Rect), typeof(float), typeof(float), typeof(float), typeof(bool), typeof(string), typeof(string), typeof(string), typeof(float)]
        );
    }

    [SuppressMessage("ReSharper", "RedundantAssignment")]
    private static bool Prefix(ref Rect rect, float value, [NotNull] ref NumberEntryController __state)
    {
        __state = SliderController.ControllerForPosition(rect);
        __state.SetStateIfNull(value);

        GameFont cache = Text.Font;
        Text.Font = GameFont.Tiny;

        float fieldWidth = rect.width / 5f;
        __state.MinimumEntryRect = new Rect(rect.x + rect.width - fieldWidth, rect.y, fieldWidth, Text.LineHeight);
        Text.Font = cache;

        if (SliderSettings.IsAlwaysOn)
        {
            rect = new Rect(rect.x, rect.y, rect.width - __state.MinimumEntryRect.Value.width - 5f, rect.height);
        }

        return true;
    }

    private static void Postfix(Rect rect, ref float __result, float roundTo, [NotNull] ref NumberEntryController __state)
    {
        GameFont cache = Text.Font;
        Text.Font = GameFont.Tiny;

        __state.BeginHysteresis(rect);

        bool active = __state.IsCurrentlyActive();

        if (active)
        {
            __state.BeginLogging();
            __state.Draw(ref __result);
        }

        __state.EndHysteresis();

        if (!active)
        {
            __state.EndLogging();
        }

        Text.Font = cache;

        if (roundTo > 0.0)
        {
            __result = Mathf.RoundToInt(__result / roundTo) * roundTo;
        }
    }
}
