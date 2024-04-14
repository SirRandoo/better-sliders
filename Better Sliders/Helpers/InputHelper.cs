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

using JetBrains.Annotations;
using UnityEngine;
using Verse;

namespace SirRandoo.BetterSliders.Helpers;

public static class InputHelper
{
    /// <summary>
    ///     Checks if any passed keys have been pressed, or currently being
    ///     held down.
    /// </summary>
    /// <param name="codes">The <see cref="KeyCode" />s to check</param>
    /// <returns>
    ///     Whether or not any of the provided <see cref="KeyCode" />s
    ///     were pressed or held down
    /// </returns>
    public static bool AnyKeyDown([NotNull] params KeyCode[] codes)
    {
        for (var i = 0; i < codes.Length; i++)
        {
            if (Input.GetKey(codes[i]))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    ///     Checks if any passed mouse buttons has been pressed, or currently
    ///     being held down.
    /// </summary>
    /// <param name="codes">The <see cref="MouseButtonCode" />s to check</param>
    /// <returns>
    ///     Whether or not any of the provided <see cref="MouseButtonCode" />s
    ///     were pressed, or are currently being held down
    /// </returns>
    public static bool AnyMouseButtonDown([NotNull] params MouseButtonCode[] codes)
    {
        for (var i = 0; i < codes.Length; i++)
        {
            if (Input.GetMouseButton((int)codes[i]))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    ///     Checks whether or not the user left clicked in the region
    ///     specified.
    /// </summary>
    /// <param name="region">The region to check</param>
    /// <returns>
    ///     Whether or not the user left clicked in the region specified.
    /// </returns>
    public static bool LeftClicked(this Rect region) => MouseButtonClicked(region, MouseButtonCode.Left);

    /// <summary>
    ///     Checks whether or not the user right clicked in the region
    ///     specified.
    /// </summary>
    /// <param name="region">The region to check</param>
    /// <returns>
    ///     Whether or not the user right clicked in the region specified.
    /// </returns>
    public static bool RightClicked(this Rect region) => MouseButtonClicked(region, MouseButtonCode.Right);

    /// <summary>
    ///     Checks whether or not the user clicked in the region specified.
    /// </summary>
    /// <param name="region">The region to check</param>
    /// <param name="codes">The <see cref="MouseButtonCode" />s to check</param>
    /// <returns>Whether or not the user clicked in the region specified.</returns>
    public static bool MouseButtonClicked(this Rect region, params MouseButtonCode[] codes)
    {
        if (!Mouse.IsOver(region))
        {
            return false;
        }

        Event current = Event.current;

        if (current.type != EventType.Used || current.type != EventType.MouseDown)
        {
            return false;
        }

        for (var i = 0; i < codes.Length; i++)
        {
            if (current.button == (int)codes[i])
            {
                current.Use();

                return true;
            }
        }

        return false;
    }
}
