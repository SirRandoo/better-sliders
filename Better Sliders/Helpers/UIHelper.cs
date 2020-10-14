using System.Globalization;
using UnityEngine;
using Verse;

namespace SirRandoo.BetterSliders.Helpers
{
    public static class UIHelper
    {
        private static string _singleSliderBuffer = "";
        private static Vector2 _singleCurrentlyFocused = new Vector2(-1f, -1f);

        private static string _doubleMinSliderBuffer = "";
        private static string _doubleMaxSliderBuffer = "";
        private static Vector2 _doubleCurrentlyFocusedMin = new Vector2(-1f, -1f);
        private static Vector2 _doubleCurrentlyFocusedMax = new Vector2(-1f, -1f);

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

        public static void Spinbox(Rect region, ref int value, float min = 0f, float max = 1E+09f, bool percent = false)
        {
            if (_singleCurrentlyFocused != region.position)
            {
                _singleCurrentlyFocused = region.position;
                _singleSliderBuffer = percent ? $"{value}%" : value.ToString();
            }

            if (percent && !_singleSliderBuffer.EndsWith("%"))
            {
                _singleSliderBuffer += "%";
            }

            GUI.SetNextControlName($"TextField{region.y:F0}{region.x:F0}");
            _singleSliderBuffer = GUI.TextField(region, _singleSliderBuffer, Text.CurTextFieldStyle);

            if (int.TryParse(_singleSliderBuffer.TrimEnd('%'), out int maybeValue))
            {
                value = (int) Mathf.Clamp(maybeValue, min, max);
            }
        }

        public static void DoubleSpinbox(
            Rect region,
            ref float value,
            float min = 0f,
            float max = 1E+09f,
            bool percent = false
        )
        {
            if (_singleCurrentlyFocused != region.position)
            {
                _singleCurrentlyFocused = region.position;
                _singleSliderBuffer = percent ? $"{value}%" : value.ToString(CultureInfo.InvariantCulture);
            }

            if (percent && !_singleSliderBuffer.EndsWith("%"))
            {
                _singleSliderBuffer += "%";
            }

            GUI.SetNextControlName($"TextField{region.y:F0}{region.x:F0}");
            _singleSliderBuffer = GUI.TextField(region, _singleSliderBuffer, Text.CurTextFieldStyle);

            if (float.TryParse(_singleSliderBuffer.TrimEnd('%'), out float maybeValue))
            {
                value = Mathf.Clamp(maybeValue, min, max);
            }
        }

        public static void MinMaxSpinboxes(
            Rect minRegion,
            Rect maxRegion,
            ref float minValue,
            ref float maxValue,
            float min = 0f,
            float max = 1E+09f,
            bool percentages = false
        )
        {
            ValidateFocusChange(
                ref _doubleCurrentlyFocusedMin,
                ref _doubleMinSliderBuffer,
                minRegion,
                minValue,
                percentages
            );

            ValidateFocusChange(
                ref _doubleCurrentlyFocusedMax,
                ref _doubleMaxSliderBuffer,
                maxRegion,
                maxValue,
                percentages
            );

            if (percentages && !_doubleMinSliderBuffer.EndsWith("%"))
            {
                _doubleMinSliderBuffer += "%";
            }

            if (percentages && !_doubleMaxSliderBuffer.EndsWith("%"))
            {
                _doubleMaxSliderBuffer += "%";
            }

            GUI.SetNextControlName($"TextField{minRegion.y:F0}{minRegion.x:F0}");
            _doubleMinSliderBuffer = GUI.TextField(minRegion, _doubleMinSliderBuffer, Text.CurTextFieldStyle);
            GUI.SetNextControlName($"TextField{maxRegion.y:F0}{maxRegion.x:F0}");
            _doubleMaxSliderBuffer = GUI.TextField(maxRegion, _doubleMaxSliderBuffer, Text.CurTextFieldStyle);

            if (TryParseFloat(_doubleMinSliderBuffer, min, maxValue, out float maybeMinValue, percentages))
            {
                minValue = maybeMinValue;
            }

            if (TryParseFloat(_doubleMaxSliderBuffer, minValue, max, out float maybeMaxValue, percentages))
            {
                maxValue = maybeMaxValue;
            }
        }
        
        public static void MinMaxSpinboxes(
            Rect minRegion,
            Rect maxRegion,
            ref int minValue,
            ref int maxValue,
            float min = 0f,
            float max = 1E+09f,
            bool percentages = false
        )
        {
            ValidateFocusChange(
                ref _doubleCurrentlyFocusedMin,
                ref _doubleMinSliderBuffer,
                minRegion,
                minValue,
                percentages
            );

            ValidateFocusChange(
                ref _doubleCurrentlyFocusedMax,
                ref _doubleMaxSliderBuffer,
                maxRegion,
                maxValue,
                percentages
            );

            if (percentages && !_doubleMinSliderBuffer.EndsWith("%"))
            {
                _doubleMinSliderBuffer += "%";
            }

            if (percentages && !_doubleMaxSliderBuffer.EndsWith("%"))
            {
                _doubleMaxSliderBuffer += "%";
            }

            GUI.SetNextControlName($"TextField{minRegion.y:F0}{minRegion.x:F0}");
            _doubleMinSliderBuffer = GUI.TextField(minRegion, _doubleMinSliderBuffer, Text.CurTextFieldStyle);
            GUI.SetNextControlName($"TextField{maxRegion.y:F0}{maxRegion.x:F0}");
            _doubleMaxSliderBuffer = GUI.TextField(maxRegion, _doubleMaxSliderBuffer, Text.CurTextFieldStyle);

            if (TryParseInt(_doubleMinSliderBuffer, min, maxValue, out int maybeMinValue, percentages))
            {
                minValue = maybeMinValue;
            }

            if (TryParseInt(_doubleMaxSliderBuffer, minValue, max, out int maybeMaxValue, percentages))
            {
                maxValue = maybeMaxValue;
            }
        }

        private static bool TryParseFloat(string input, float min, float max, out float value, bool percent = false)
        {
            if (!float.TryParse(input.TrimEnd('%'), out float maybeMinValue))
            {
                value = 0;
                return false;
            }

            if (percent)
            {
                maybeMinValue /= 100f;
            }

            value = Mathf.Clamp(maybeMinValue, min, max);
            return true;
        }

        private static bool TryParseInt(string input, float min, float max, out int value, bool percent = false)
        {
            if (!float.TryParse(input.TrimEnd('%'), out float maybeMinValue))
            {
                value = 0;
                return false;
            }

            if (percent)
            {
                maybeMinValue /= 100f;
            }

            value = (int) Mathf.Clamp(maybeMinValue, min, max);
            return true;
        }

        private static void ValidateFocusChange(
            ref Vector2 focusCache,
            ref string focusBufferCache,
            Rect region,
            float value,
            bool percents
        )
        {
            if (focusCache != region.position)
            {
                focusCache = region.position;
                focusBufferCache = percents ? $"{value * 100f}%" : value.ToString(CultureInfo.InvariantCulture);
            }
        }
    }
}
