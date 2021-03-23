using UnityEngine;

namespace SirRandoo.BetterSliders
{
    public class ExpandedState
    {
        public bool ShouldRender { get; set; }
        public bool ShouldFocusField { get; set; }
        public Rect HorizontalDrawRect { get; set; }
        public Rect MinimumDrawRect { get; set; }
        public Rect MaximumDrawRect { get; set; }
    }
}
