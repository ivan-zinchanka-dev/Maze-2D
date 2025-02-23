using System;
using Maze2D.Settings;
using UnityEngine;

namespace Maze2D.Configs
{
    [CreateAssetMenu(fileName = "default_settings", menuName = "Configs/DefaultSettings", order = 0)]
    internal class DefaultSettingsConfig : ScriptableObject
    {
        [SerializeField] 
        private DefaultSettings _settings;

        public Settings.Settings Settings => Map(_settings);
        
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

        private Settings.Settings Map(DefaultSettings settings)
        {
            return new Settings.Settings(
                settings.GameDifficulty,
                settings.PlayerColor,
                settings.MusicVolume,
                settings.SoundsVolume);
        }
    }
}