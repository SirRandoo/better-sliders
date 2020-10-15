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
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class ExpandedSliderHorizontal
    {
        [UsedImplicitly]
        private static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(Widgets), nameof(Widgets.HorizontalSlider));
        }

        [UsedImplicitly]
        [SuppressMessage("ReSharper", "RedundantAssignment")]
        private static void Prefix(Rect rect, ref ExpandedState __state)
        {
            __state = new ExpandedState();
            
            GameFont cache = Text.Font;
            Text.Font = GameFont.Tiny;

            float fieldWidth = rect.width / 5f;
            __state.HorizontalDrawRect = new Rect(rect.x + rect.width - fieldWidth, rect.y, fieldWidth, Text.LineHeight);
            __state.ShouldFocusField = __state.HorizontalDrawRect.WasClicked();
            Text.Font = cache;
        }

        [UsedImplicitly]
        private static void Postfix(Rect rect, ref float __result, float leftValue, float rightValue, float roundTo, ref ExpandedState __state)
        {
            if (!Mouse.IsOver(rect))
            {
                return;
            }
            
            if (__state.ShouldFocusField)
            {
                GUI.FocusControl("TextField" + __state.HorizontalDrawRect.y.ToString("F0") + __state.HorizontalDrawRect.x.ToString("F0"));
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
