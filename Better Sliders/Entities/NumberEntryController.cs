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

using System.Diagnostics.CodeAnalysis;
using System.Text;
using JetBrains.Annotations;
using SirRandoo.BetterSliders.Helpers;
using UnityEngine;
using Verse;

namespace SirRandoo.BetterSliders.Entities
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    internal sealed class NumberEntryController
    {
        private bool _isEffectivelyDisabled;
        private float _maximum;
        private string _maximumBuffer;
        private bool _maximumBufferValid;
        private string _maximumEntryName;
        private Rect? _maximumEntryRect;
        private float _maximumInteractionTick;
        private float _minimum;
        private string _minimumBuffer;
        private bool _minimumBufferValid;
        private string _minimumEntryName;
        private Rect? _minimumEntryRect;
        private float _minimumInteractionTick;
        private Color? _previousColor;
        private StringBuilder _logBuilder;
        private bool MinimumInteractedRecently => Mathf.Abs(Time.unscaledTime - _minimumInteractionTick) <= 1f;
        private bool MaximumInteractedRecently => Mathf.Abs(Time.unscaledTime - _maximumInteractionTick) <= 1f;

        public Rect? MaximumEntryRect
        {
            get => _maximumEntryRect;
            set
            {
                _maximumEntryRect = value;
                _maximumEntryName = $"TextField{MaximumEntryRect!.Value.y:F0}{MaximumEntryRect.Value.x:F0}";
            }
        }
        public Rect? MinimumEntryRect
        {
            get => _minimumEntryRect;
            set
            {
                _minimumEntryRect = value;
                _minimumEntryName = $"TextField{MinimumEntryRect!.Value.y:F0}{MinimumEntryRect.Value.x:F0}";
            }
        }

        public int GroupId { get; set; }

        public System.WeakReference<Window> Parent { get; set; }

        public bool IsCurrentlyActive()
        {
            if (!Parent.TryGetTarget(out Window parent) || !Find.WindowStack.GetsInput(parent))
            {
                return false;
            }

            return SliderSettings.IsAlwaysOn || (!InputHelper.AnyKeyDown(KeyCode.LeftControl, KeyCode.RightControl) && !_isEffectivelyDisabled);
        }

        public void SetMaximumValue(float value)
        {
            _maximumBufferValid = false;
            _maximumBuffer = value.ToString("N");
            _maximum = value;
            _maximumBufferValid = true;
        }

        public void SetMinimumValue(float value)
        {
            _minimumBufferValid = false;
            _minimumBuffer = value.ToString("N");
            _minimum = value;
            _minimumBufferValid = true;
        }

        public void SetStateIfNull(float minimum)
        {
            if (_minimumBuffer == null)
            {
                SetMinimumValue(minimum);
            }
        }

        public void SetStateIfNull(float minimum, float maximum)
        {
            if (_minimumBuffer == null)
            {
                SetMinimumValue(minimum);
            }

            if (_maximumBuffer == null)
            {
                SetMaximumValue(maximum);
            }
        }

        public void Draw(ref float value)
        {
            _logBuilder?.Append("  - Attempting to draw single number field\n");

            DrawMinimumEntry(ref value);
        }
        private void DrawMinimumEntry(ref float value)
        {
            if (MinimumEntryRect.HasValue)
            {
                _logBuilder?.Append($"    - Drawing minimum entry field (id: {_minimumEntryName}) @");
                _logBuilder?.Append($" [x: {MinimumEntryRect.Value.x:N}, y: {MinimumEntryRect.Value.y:N}, group: {GroupId:N}]");
                _logBuilder?.Append($" with a size of [width: {MinimumEntryRect.Value.width:N}, height: {MinimumEntryRect.Value.height:N}]\n");

                GUI.SetNextControlName(_minimumEntryName);

                if (UiHelper.NumberField(MinimumEntryRect.Value, ref _minimumBuffer, ref _minimum, ref _minimumBufferValid))
                {
                    _minimumInteractionTick = Time.unscaledTime;
                }

                TryFocusControl(_minimumEntryName);
                TryProcessInteraction();
            }

            if (!MinimumInteractedRecently)
            {
                _logBuilder?.Append("    - Minimum entry field hasn't been interacted with recently; updating field with slider's value\n");
                SetMinimumValue(value);
            }
            else
            {
                _logBuilder?.Append("    - Minimum entry field has been interacted with recently; updating slider's value\n");
                value = _minimum;
            }
        }

        public void Draw(ref float minimum, ref float maximum)
        {
            _logBuilder?.Append("  - Attempting to draw min/max number fields\n");
            DrawMinimumEntry(ref minimum);
            DrawMaximumEntry(ref maximum);
        }

        private void DrawMaximumEntry(ref float maximum)
        {
            if (MaximumEntryRect.HasValue)
            {
                _logBuilder?.Append($"    - Drawing maximum entry field (id: {_maximumEntryName}) @");
                _logBuilder?.Append($" [x: {MaximumEntryRect.Value.x:N}, y: {MaximumEntryRect.Value.y:N}, group: {GroupId:N}]");
                _logBuilder?.Append($" with a size of [width: {MaximumEntryRect.Value.width:N}, height: {MaximumEntryRect.Value.height:N}]\n");
                GUI.SetNextControlName(_maximumEntryName);

                if (UiHelper.NumberField(MaximumEntryRect.Value, ref _maximumBuffer, ref _maximum, ref _maximumBufferValid))
                {
                    _maximumInteractionTick = Time.unscaledTime;
                }

                TryFocusControl(_maximumEntryName);
                TryProcessInteraction();
            }

            if (!MaximumInteractedRecently)
            {
                _logBuilder?.Append("    - Maximum entry field hasn't been interacted with recently; updating field with slider's value\n");
                SetMaximumValue(maximum);
            }
            else
            {
                _logBuilder?.Append("    - Maximum entry field has been interacted with recently; updating slider's value\n");
                maximum = _maximum;
            }
        }

        public void Draw(ref int minimum, ref int maximum)
        {
            _logBuilder?.Append("  - Attempting to draw min/max integer fields\n");
            float minProxy = minimum;
            float maxProxy = maximum;

            DrawMinimumEntry(ref minProxy);
            minimum = (int)minProxy;

            DrawMaximumEntry(ref maxProxy);
            maximum = (int)maxProxy;
        }

        public void BeginHeuristics(Rect region)
        {
            _logBuilder?.Append($"  - Beginning heuristics at [x: {region.x:N}, y: {region.y:N}, group: {GroupId:N}]");
            _logBuilder?.Append($" with a size of [width: {region.width:N}, height: {region.height:N}]\n");

            if (region.Contains(Event.current.mousePosition))
            {
                _logBuilder?.Append("    - Mouse isn't over slider position\n");
                return;
            }

            float distance = GenUI.DistFromRect(region, Event.current.mousePosition);
            _logBuilder?.Append($"    - Mouse distance from slider: {distance:N}");

            if (distance >= SliderSettings.HeuristicsEndDistance)
            {
                _logBuilder?.Append("      - Mouse is too far from slider; hiding number field\n");
                Color color = _previousColor ?? Color.white;
                GUI.color = new Color(color.r, color.g, color.b, 0f);
                _isEffectivelyDisabled = true;
            }
            else if (distance >= SliderSettings.HeuristicsBeginDistance)
            {
                _logBuilder?.Append("      - Mouse is within the fade out distance; adjusting number field's visibility to match\n");
                _previousColor ??= GUI.color;

                GUI.color = new Color(
                    _previousColor.Value.r,
                    _previousColor.Value.g,
                    _previousColor.Value.b,
                    Mathf.Clamp(1f - _previousColor.Value.a * (distance / SliderSettings.HeuristicsEndDistance), 0f, 1f)
                );

                _isEffectivelyDisabled = false;
            }
            else if (distance <= SliderSettings.HeuristicsBeginDistance)
            {
                _logBuilder?.Append("      - Mouse isn't within fade out distance; ensuring number field isn't transparent\n");
                Color color = _previousColor ?? Color.white;
                GUI.color = new Color(color.r, color.g, color.b, 1f);
                _isEffectivelyDisabled = false;
            }

            if (SliderSettings.IsAlwaysOn)
            {
                _logBuilder?.Append("    - Mode is 'always on'; ensuring number fields aren't transparent\n");
                GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, 1f);
            }
        }

        public void EndHeuristics()
        {
            _logBuilder?.Append("  - Ending mouse heuristics\n");
            GUI.color = _previousColor ?? Color.white;
            _previousColor = null;

            if (_isEffectivelyDisabled)
            {
                _logBuilder?.Append("    - Number field was marked as irrelevant; attempting to clear focus\n");
                TryClearFocusIfHas();
            }
        }

        private void TryClearFocusIfHas()
        {
            if (MinimumEntryRect.HasValue && GUI.GetNameOfFocusedControl() == _minimumEntryName)
            {
                GUIUtility.keyboardControl = 0;
            }

            if (MaximumEntryRect.HasValue && GUI.GetNameOfFocusedControl() == _maximumEntryName)
            {
                GUIUtility.keyboardControl = 0;
            }
        }

        private void TryFocusControl(string name)
        {
            if (GUI.GetNameOfFocusedControl() == name || !this.IsControllerClosest())
            {
                return;
            }

            GUI.FocusControl(name);
        }

        private void TryProcessInteraction()
        {
            _logBuilder?.Append("  - Attempting to process interaction\n");

            if (SliderSettings.DisplayStyleRaw == nameof(SliderSettings.Style.AlwaysOn))
            {
                _logBuilder?.Append("    - Display mode is 'always on'; ignoring interaction\n");
                return;
            }

            if (MinimumEntryRect.HasValue && GUI.GetNameOfFocusedControl() == _minimumEntryName && IsCurrentEventInteraction(MinimumEntryRect.Value))
            {
                _logBuilder?.Append("    - User interacted with minimum number field; updating relevant interaction tick\n");
                _minimumInteractionTick = Time.unscaledTime;
            }

            if (MaximumEntryRect.HasValue && GUI.GetNameOfFocusedControl() == _maximumEntryName && IsCurrentEventInteraction(MaximumEntryRect.Value))
            {
                _logBuilder?.Append("    - User interacted with maximum number field; updating relevant interaction tick\n");
                _maximumInteractionTick = Time.unscaledTime;
            }
        }

        private static bool IsCurrentEventInteraction(Rect region)
        {
            switch (Event.current.type)
            {
                case EventType.Used:
                case EventType.MouseDown:
                case EventType.KeyDown:
                    return region.Contains(Event.current.mousePosition);;
                default:
                    return false;
            }
        }

        internal void BeginLogging()
        {
            _logBuilder = new StringBuilder();
        }

        internal void EndLogging()
        {
            if (_logBuilder == null)
            {
                return;
            }

            Log.Message(_logBuilder.ToString());
            _logBuilder = null;
        }
    }
}
