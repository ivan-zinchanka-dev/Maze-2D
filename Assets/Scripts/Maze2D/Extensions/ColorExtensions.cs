using UnityEngine;

namespace Maze2D.Extensions
{
    public static class ColorExtensions
    {
        public static bool EqualsApproximately(this Color color, Color other)
        {
            return color.r.EqualsApproximately(other.r) &&
                   color.g.EqualsApproximately(other.g) &&
                   color.b.EqualsApproximately(other.b) &&
                   color.a.EqualsApproximately(other.a);
        }
    }
}