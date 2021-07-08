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

using UnityEngine;
using Verse;

namespace SirRandoo.BetterSliders.Helpers
{
    public static class UIHelper
    {
        public static bool WasClicked(this Rect region)
        {
            if (!Mouse.IsOver(region))
            {
                return false;
            }

            Event current = Event.current;
            bool was = current.button == 0;

            switch (current.type)
            {
                case EventType.Used when was:
                case EventType.MouseDown when was:
                    current.Use();
                    return true;
                default:
                    return false;
            }
        }

        public static void Spinbox(
            Rect region,
            ref int value,
            ref string buffer,
            float min = 0f,
            float max = 1E+09f,
            bool percent = false
        )
        {
            buffer ??= value.ToString();

            if (percent && !buffer.EndsWith("%"))
            {
                buffer += "%";
            }

            var controlName = $"TextField{region.y:F0}{region.x:F0}";
            GUI.SetNextControlName(controlName);

            string result = GUI.TextField(region, buffer, Text.CurTextFieldStyle);

            if (GUI.GetNameOfFocusedControl() != controlName)
            {
                NumberHelper.ParseInteger(buffer, ref value, ref buffer, min, max, true);
            }
            else
            {
                if (percent && result.EndsWith("%"))
                {
                    result = result.Substring(0, result.Length - 1);
                }

                if (result == buffer || !NumberHelper.IsPartiallyOrFullyTypedNumber<int>(result, min))
                {
                    return;
                }

                buffer = result;

                if (!result.IsFullyTypedNumber<int>())
                {
                    return;
                }

                NumberHelper.ParseInteger(result, ref value, ref buffer, min, max, false);
            }
        }

        public static void DoubleSpinbox(
            Rect region,
            ref float value,
            ref string buffer,
            float min = 0f,
            float max = 1E+09f,
            bool percent = false
        )
        {
            buffer ??= value.ToString("0.0#########");

            if (percent && !buffer.EndsWith("%"))
            {
                buffer += "%";
            }

            var controlName = $"TextField{region.y:F0}{region.x:F0}";
            GUI.SetNextControlName(controlName);

            string result = GUI.TextField(region, buffer, Text.CurTextFieldStyle);

            if (percent && result.EndsWith("%"))
            {
                result = result.Substring(0, result.Length - 1);
            }

            if (GUI.GetNameOfFocusedControl() != controlName)
            {
                NumberHelper.ParseFloat(buffer, ref value, ref buffer, min, max, true);
            }
            else
            {
                if (result == buffer || !NumberHelper.IsPartiallyOrFullyTypedNumber<float>(result, min))
                {
                    return;
                }

                buffer = result;

                if (!result.IsFullyTypedNumber<float>())
                {
                    return;
                }

                NumberHelper.ParseFloat(result, ref value, ref buffer, min, max, false);
            }
        }

        public static bool IsRenderDisabled()
        {
            return Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
        }
    }
}
