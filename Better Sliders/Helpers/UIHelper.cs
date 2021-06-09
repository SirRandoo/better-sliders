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
