using System;
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
    public static class ExpandedSliderFloat
    {
        [UsedImplicitly]
        private static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(Widgets), nameof(Widgets.FloatRange));
        }

        [UsedImplicitly]
        [SuppressMessage("ReSharper", "RedundantAssignment")]
        private static void Prefix(Rect rect, ref ExpandedState __state)
        {
            __state = new ExpandedState();

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
        }

        [UsedImplicitly]
        private static void Postfix(
            Rect rect,
            ref FloatRange range,
            float min,
            float max,
            ToStringStyle valueStyle,
            ref ExpandedState __state
        )
        {
            if (!Mouse.IsOver(rect))
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
            UIHelper.MinMaxSpinboxes(
                __state.MinimumDrawRect,
                __state.MaximumDrawRect,
                ref range.min,
                ref range.max,
                min,
                max,
                valueStyle == ToStringStyle.PercentZero
                || valueStyle == ToStringStyle.PercentOne
                || valueStyle == ToStringStyle.PercentTwo
            );
            Text.Font = cache;
        }
    }
}
