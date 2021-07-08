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

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
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
        private static void ResetValue<T>([CanBeNull] ref T val, [NotNull] ref string buffer, float min, float max)
        {
            val = default;
            if (min > 0.0)
            {
                val = (T) (object) Mathf.RoundToInt(min);
            }

            if (max < 0.0)
            {
                val = (T) (object) Mathf.RoundToInt(max);
            }

            buffer = ToStringTypedIn(val);
        }

        [NotNull]
        private static string ToStringTypedIn<T>(T val)
        {
            return typeof(T) == typeof(float) ? ((float) (object) val).ToString("N1") : val.ToString();
        }

        public static bool IsPartiallyOrFullyTypedNumber<T>([NotNull] string s, float min)
        {
            return s == ""
                   || (s[0] != '-' || min < 0.0)
                   && (s.Length <= 1 || s[s.Length - 1] != '-')
                   && s != "00"
                   && s.Length <= 12
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

            if (strArray.Length is > 2 or < 1
                || !strArray[0].ContainsOnlyCharacters("-0123456789")
                || strArray.Length == 2
                && (strArray[1].Length == 0 || !strArray[1].ContainsOnlyCharacters("0123456789")))
            {
                return false;
            }

            return !(typeof(T) == typeof(int)) || s.ContainsOnlyCharacters("-0123456789");
        }

        public static int CharacterCount([NotNull] this string s, char c)
        {
            return s.Count(t => t == c);
        }

        private static bool ContainsOnlyCharacters([NotNull] this string s, string allowedChars)
        {
            return s.All(allowedChars.Contains);
        }
    }
}
