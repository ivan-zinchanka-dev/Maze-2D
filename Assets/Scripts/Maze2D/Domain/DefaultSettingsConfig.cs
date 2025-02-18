using System;
using UnityEngine;

namespace Maze2D.Domain
{
    [CreateAssetMenu(fileName = "default_settings", menuName = "Configs/DefaultSettings", order = 0)]
    public class DefaultSettingsConfig : ScriptableObject
    {
        [SerializeField] 
        private DefaultSettings _settings;

        public Settings Settings => Map(_settings);
        
        [Serializable]
        private struct DefaultSettings
        {
            public Difficulty GameDifficulty;
            public Color32 PlayerColor;
            
            [Range(0f, 1f)]
            public float MusicVolume;
            [Range(0f, 1f)]
            public float SoundsVolume;
        }

        private Settings Map(DefaultSettings settings)
        {
            return new Settings(
                settings.GameDifficulty,
                settings.PlayerColor,
                settings.MusicVolume,
                settings.SoundsVolume);
        }
    }
}