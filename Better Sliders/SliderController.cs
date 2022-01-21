// MIT License
//
// Copyright (c) 2022 SirRandoo
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
using JetBrains.Annotations;
using SirRandoo.BetterSliders.Entities;
using UnityEngine;
using Verse;

namespace SirRandoo.BetterSliders
{
    internal static class SliderController
    {
        private static readonly Dictionary<int, List<NumberEntryController>> WindowNumberControllers = new();
        private static readonly int BeginGroupHashCode = "BeginGroup".GetHashCode();

        [NotNull]
        internal static NumberEntryController[] ControllersForWindow([NotNull] Window window) =>
            !WindowNumberControllers.TryGetValue(window.ID, out List<NumberEntryController> controllers) ? Array.Empty<NumberEntryController>() : controllers.ToArray();

        [NotNull] internal static NumberEntryController[] ControllersForCurrentWindow() => ControllersForWindow(Find.WindowStack.currentlyDrawnWindow);

        [NotNull]
        internal static NumberEntryController ControllerForPosition(Rect region)
        {
            int groupId = GUIUtility.GetControlID(BeginGroupHashCode, FocusType.Passive);

            NumberEntryController controller;

            if (!WindowNumberControllers.TryGetValue(Find.WindowStack.currentlyDrawnWindow.ID, out List<NumberEntryController> controllers))
            {
                controller = new NumberEntryController { Parent = new System.WeakReference<Window>(Find.WindowStack.currentlyDrawnWindow), GroupId = groupId };

                controllers = new List<NumberEntryController> { controller };
                WindowNumberControllers[Find.WindowStack.currentlyDrawnWindow.ID] = controllers;

                return controller;
            }

            foreach (NumberEntryController c in controllers)
            {
                if (groupId != c.GroupId)
                {
                    continue;
                }
                
                if (c.MinimumEntryRect.HasValue && c.MinimumEntryRect.Value.Overlaps(region))
                {
                    return c;
                }

                if (c.MaximumEntryRect.HasValue && c.MaximumEntryRect.Value.Overlaps(region))
                {
                    return c;
                }
            }

            controller = new NumberEntryController { Parent = new System.WeakReference<Window>(Find.WindowStack.currentlyDrawnWindow), GroupId = groupId };
            controllers.Add(controller);

            return controller;
        }

        internal static void RemoveControllersForPosition(Rect region)
        {
            if (!WindowNumberControllers.TryGetValue(Find.WindowStack.currentlyDrawnWindow.ID, out List<NumberEntryController> controllers))
            {
                return;
            }

            controllers.RemoveAll(
                c => (c.MinimumEntryRect.HasValue && c.MinimumEntryRect.Value.Overlaps(region))
                    || (c.MaximumEntryRect.HasValue && c.MaximumEntryRect.Value.Overlaps(region))
            );
        }

        internal static void RemoveControllerForWindow([NotNull] Window window)
        {
            if (WindowNumberControllers.TryGetValue(window.ID, out List<NumberEntryController> controllers))
            {
                for (var i = 0; i < controllers.Count; i++)
                {
                    controllers[i].EndLogging();
                }
            }

            WindowNumberControllers.Remove(window.ID);
        }

        [NotNull]
        internal static NumberEntryController[] GetActiveControllers()
        {
            return ControllersForCurrentWindow().Where(c => c.IsCurrentlyActive()).ToArray();
        }

        internal static bool IsControllerClosest([NotNull] this NumberEntryController controller)
        {
            Vector2 mousePosition = Event.current.mousePosition;
            float controllerDistance = controller.EffectiveDistanceFromPoint(mousePosition);

            foreach (NumberEntryController c in GetActiveControllers())
            {
                if (c.EffectiveDistanceFromPoint(mousePosition) < controllerDistance)
                {
                    return false;
                }
            }

            return true;
        }

        private static float EffectiveDistanceFromPoint([NotNull] this NumberEntryController controller, Vector2 position)
        {
            var controllerDistance = 10000f;

            if (controller.MinimumEntryRect.HasValue)
            {
                controllerDistance = Mathf.Min(controllerDistance, GenUI.DistFromRect(controller.MinimumEntryRect.Value, position));
            }

            if (controller.MaximumEntryRect.HasValue)
            {
                controllerDistance = Mathf.Min(controllerDistance, GenUI.DistFromRect(controller.MaximumEntryRect.Value, position));
            }

            return controllerDistance;
        }
    }
}
