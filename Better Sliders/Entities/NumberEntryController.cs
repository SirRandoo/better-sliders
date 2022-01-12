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
            DrawMinimumEntry(ref value);
        }
        private void DrawMinimumEntry(ref float value)
        {
            if (MinimumEntryRect.HasValue)
            {
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
                SetMinimumValue(value);
            }
            else
            {
                value = _minimum;
            }
        }

        public void Draw(ref float minimum, ref float maximum)
        {
            DrawMinimumEntry(ref minimum);
            DrawMaximumEntry(ref maximum);
        }
        private void DrawMaximumEntry(ref float maximum)
        {
            if (MaximumEntryRect.HasValue)
            {
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
                SetMaximumValue(maximum);
            }
            else
            {
                maximum = _maximum;
            }
        }

        public void Draw(ref int minimum, ref int maximum)
        {
            float minProxy = minimum;
            float maxProxy = maximum;

            DrawMinimumEntry(ref minProxy);
            minimum = (int)minProxy;

            DrawMaximumEntry(ref maxProxy);
            maximum = (int)maxProxy;
        }

        public void BeginHeuristics(Rect region)
        {
            if (region.Contains(Event.current.mousePosition))
            {
                return;
            }

            float distance = GenUI.DistFromRect(region, Event.current.mousePosition);

            if (distance >= SliderSettings.HeuristicsEndDistance)
            {
                Color color = _previousColor ?? Color.white;
                GUI.color = new Color(color.r, color.g, color.b, 0f);
                _isEffectivelyDisabled = true;
            }
            else if (distance >= SliderSettings.HeuristicsBeginDistance)
            {
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
                Color color = _previousColor ?? Color.white;
                GUI.color = new Color(color.r, color.g, color.b, 1f);
                _isEffectivelyDisabled = false;
            }

            if (SliderSettings.IsAlwaysOn)
            {
                GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, 1f);
            }
        }

        public void EndHeuristics()
        {
            GUI.color = _previousColor ?? Color.white;
            _previousColor = null;

            if (_isEffectivelyDisabled)
            {
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
            if (SliderSettings.DisplayStyleRaw == nameof(SliderSettings.Style.AlwaysOn))
            {
                return;
            }

            if (MinimumEntryRect.HasValue && GUI.GetNameOfFocusedControl() == _minimumEntryName && IsCurrentEventInteraction(MinimumEntryRect.Value))
            {
                _minimumInteractionTick = Time.unscaledTime;
            }

            if (MaximumEntryRect.HasValue && GUI.GetNameOfFocusedControl() == _maximumEntryName && IsCurrentEventInteraction(MaximumEntryRect.Value))
            {
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
    }
}
