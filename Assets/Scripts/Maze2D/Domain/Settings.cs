using System;
using UnityEngine;

namespace Maze2D.Domain
{
    public class Settings
    {
        public Difficulty GameDifficulty { get; set; }
        public Color32 PlayerColor { get; set; }
        
        public float MusicVolume { get; set; }
        public float SoundsVolume { get; set; }

        private event Action OnSave;
        
        public Settings(
            Difficulty gameDifficulty, 
            Color32 playerColor, 
            float musicVolume, 
            float soundsVolume,
            Action onSave = null)
        {
            GameDifficulty = gameDifficulty;
            PlayerColor = playerColor;
            MusicVolume = musicVolume;
            SoundsVolume = soundsVolume;
            OnSave = onSave;
        }
        
        public void Save() => OnSave?.Invoke();
    }
}