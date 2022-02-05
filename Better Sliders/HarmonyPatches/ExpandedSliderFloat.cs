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
using System.Text;
using HarmonyLib;
using JetBrains.Annotations;
using SirRandoo.BetterSliders.Entities;
using Unity.Collections;
using UnityEngine;
using Verse;

namespace SirRandoo.BetterSliders.HarmonyPatches
{
    [HarmonyPatch]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class ExpandedSliderFloat
    {
        private static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(Widgets), nameof(Widgets.FloatRange));
        }

        [SuppressMessage("ReSharper", "RedundantAssignment")]
        private static void Prefix(ref Rect rect, FloatRange range)
        {
            NumberEntryController controller = SliderController.ControllerForPosition(rect);
            controller = SliderController.ControllerForPosition(rect);
            controller.SetStateIfNull(range.min, range.max);

            GameFont cache = Text.Font;
            Text.Font = GameFont.Tiny;

            var gapRect = new Rect(rect.center.x - 5f, Text.LineHeight / 2f - 1f, 10f, 5f);

            float usedWidth = rect.width - gapRect.width - 10f;
            float distributedWidth = usedWidth / 4f;
            controller.MinimumEntryRect = new Rect(rect.x, rect.y, distributedWidth, Text.LineHeight);
            controller.MaximumEntryRect = new Rect(gapRect.x + gapRect.width + controller.MinimumEntryRect.Value.width + 5f, rect.y, distributedWidth, Text.LineHeight);

            Text.Font = cache;

            if (SliderSettings.IsAlwaysOn)
            {
                rect = new Rect(
                    controller.MinimumEntryRect.Value.x + controller.MinimumEntryRect.Value.width + 5f,
                    rect.y,
                    rect.width - controller.MaximumEntryRect.Value.width - controller.MaximumEntryRect.Value.width - 10f,
                    rect.height
                );
            }
        }

        private static void Postfix(Rect rect, ref FloatRange range, float min, float max, ToStringStyle valueStyle)
        {
            NumberEntryController controller = SliderController.ControllerForPosition(rect);
            GameFont cache = Text.Font;
            Text.Font = GameFont.Tiny;

            controller.BeginHeuristics(rect);

            bool active = controller.IsCurrentlyActive();

            if (active)
            {
                controller.BeginLogging();
                controller.Draw(ref range.min, ref range.max);
            }

            controller.EndHeuristics();

            if (!active)
            {
                controller.EndLogging();
            }

            range.min = Mathf.Clamp(range.min, min, range.max);
            range.max = Mathf.Clamp(range.max, range.min, max);

            Text.Font = cache;
        }
    }
}
