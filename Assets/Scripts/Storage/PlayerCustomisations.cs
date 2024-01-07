using UnityEngine;

namespace Storage
{
    public struct PlayerCustomisations
    {
        public static PlayerCustomisations Default { get; } = new PlayerCustomisations { ColorIndex = 0, RGBAColor = Color.white };

        public Color RGBAColor { get; set; }
        public int ColorIndex { get; set; }

    }
}