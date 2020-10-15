using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;
using Verse;

namespace SirRandoo.BetterSliders.Helpers
{
    public static class NumberHelper
    {
        public static void ParseInteger(
            string serialized,
            ref int value,
            ref string buffer,
            float min,
            float max,
            bool force
        )
        {
            if (serialized.NullOrEmpty())
            {
                ResetValue(ref value, ref buffer, min, max);
            }
            else
            {
                if (int.TryParse(serialized, out int result))
                {
                    value = Mathf.RoundToInt(Mathf.Clamp(result, min, max));
                    buffer = value.ToString();
                }
                else
                {
                    if (!force)
                    {
                        return;
                    }
                    
                    ResetValue(ref value, ref buffer, min, max);
                }
            }
        }

        public static void ParseFloat(
            string serialized,
            ref float value,
            ref string buffer,
            float min,
            float max,
            bool force
        )
        {
            if (float.TryParse(serialized, out float result))
            {
                value = Mathf.Clamp(result, min, max);
                buffer = value.ToString("0.0#########");
            }
            else
            {
                if (!force)
                {
                    return;
                }
                
                ResetValue(ref value, ref buffer, min, max);
            }
        }

        [SuppressMessage("ReSharper", "RedundantAssignment")]
        private static void ResetValue<T>(
            ref T val,
            ref string buffer,
            float min,
            float max)
        {
            val = default;
            if (min > 0.0)
                val = (T) (object) Mathf.RoundToInt(min);
            if (max < 0.0)
                val = (T) (object) Mathf.RoundToInt(max);
            buffer = ToStringTypedIn(val);
        }
        
        private static string ToStringTypedIn<T>(T val)
        {
            return typeof(T) == typeof(float) 
                ? ((float) (object) val).ToString("N1") 
                : val.ToString();
        }

        public static bool IsPartiallyOrFullyTypedNumber<T>(
            string s,
            float min)
        {
            return s == ""
                   || (s[0] != '-' || min < 0.0)
                   && (s.Length <= 1 || s[s.Length - 1] != '-')
                   && (s != "00" && s.Length <= 12)
                   && (typeof(T) == typeof(float)
                       && s.CharacterCount('.') <= 1
                       && s.ContainsOnlyCharacters("-.0123456789")
                       || s.IsFullyTypedNumber<T>());
        }

        internal static bool IsFullyTypedNumber<T>(this string s)
        {
            if (s == "")
            {
                return false;
            }

            if (typeof(T) != typeof(float))
            {
                return !(typeof(T) == typeof(int)) || s.ContainsOnlyCharacters("-0123456789");
            }

            string[] strArray = s.Split('.');
            if (strArray.Length > 2
                || strArray.Length < 1
                || !strArray[0].ContainsOnlyCharacters("-0123456789")
                || strArray.Length == 2
                && (strArray[1].Length == 0 || !strArray[1].ContainsOnlyCharacters("0123456789")))
                return false;
            return !(typeof(T) == typeof(int)) || s.ContainsOnlyCharacters("-0123456789");
        }

        public static int CharacterCount(this string s, char c)
        {
            return s.Count(t => (int) t == (int) c);
        }
        
        private static bool ContainsOnlyCharacters(this string s, string allowedChars)
        {
            return s.All(allowedChars.Contains);
        }
    }
}
