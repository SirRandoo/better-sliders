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
using SirRandoo.BetterSliders.Entities;
using UnityEngine;
using Verse;

namespace SirRandoo.BetterSliders.HarmonyPatches;

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
    private static void Prefix(ref Rect rect, IntRange range, [NotNull] ref NumberEntryController __state, out bool __continue)
    {
        __continue = true;
        var tempState = SliderController.ControllerForPosition(rect);

        if (tempState == null)
        {
            __continue = false;
            return;
        }

        __state = tempState;
        __state.SetStateIfNull(range.min, range.max);

        GameFont cache = Text.Font;
        Text.Font = GameFont.Tiny;

        var gapRect = new Rect(rect.center.x - 5f, Text.LineHeight / 2f - 1f, 10f, 5f);

        float usedWidth = rect.width - gapRect.width - 10f;
        float distributedWidth = usedWidth / 4f;
        __state.MinimumEntryRect = new Rect(rect.x, rect.y, distributedWidth, Text.LineHeight);
        __state.MaximumEntryRect = new Rect(gapRect.x + gapRect.width + __state.MinimumEntryRect.Value.width + 5f, rect.y, distributedWidth, Text.LineHeight);

        Text.Font = cache;

        if (SliderSettings.IsAlwaysOn)
        {
            rect = new Rect(__state.MinimumEntryRect.Value.x + __state.MinimumEntryRect.Value.width + 5f, rect.y, usedWidth - 10f, rect.height);
        }
    }

    private static void Postfix(Rect rect, ref IntRange range, int min, int max, [NotNull] ref NumberEntryController __state, bool __continue)
    {
        if (!__continue)
        {
            return;
        }

        GameFont cache = Text.Font;
        Text.Font = GameFont.Tiny;

        __state.BeginHysteresis(rect);

        bool active = __state.IsCurrentlyActive();

        if (active)
        {
            __state.BeginLogging();
            __state.Draw(ref range.min, ref range.max);
        }

        __state.EndHysteresis();

        if (!active)
        {
            __state.EndLogging();
        }

        range.min = Mathf.Clamp(range.min, min, range.max);
        range.max = Mathf.Clamp(range.max, range.min, max);

        Text.Font = cache;
    }
}
