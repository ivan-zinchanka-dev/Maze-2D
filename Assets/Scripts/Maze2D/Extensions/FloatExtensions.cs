using UnityEngine;

namespace Maze2D.Extensions
{
    public static class FloatExtensions
    {
        private const float Epsilon = 0.000001f;
        
        public static bool EqualsApproximately(this float value, float other)
        {
            return Mathf.Abs(value - other) < Epsilon;
        }
    }
}