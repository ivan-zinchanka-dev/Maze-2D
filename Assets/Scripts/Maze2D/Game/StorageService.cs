﻿using UnityEngine;

namespace Maze2D.Game
{
    public class StorageService
    {
        private const string DifficultyKey = "difficulty";
        private const string PlayerColorKey = "color";
        
        private readonly Difficulty _defaultDifficulty;
        
        private static readonly Color32 DefaultColor = new Color(1.0f, 0.1680528f, 0.0f, 1.0f);

        public StorageService(Difficulty defaultDifficulty)
        {
            _defaultDifficulty = defaultDifficulty;
        }
        
        public void SetDifficulty(Difficulty difficultyLevel) {

            PlayerPrefs.SetInt(DifficultyKey, (int)difficultyLevel);
            PlayerPrefs.Save();
        }

        public Difficulty GetDifficulty() {
            
            return (Difficulty)PlayerPrefs.GetInt(DifficultyKey, (int)_defaultDifficulty);
        }
        
        public void SetPlayerColor(Color32 playerColor)
        {
            PlayerPrefs.SetString(PlayerColorKey, "#" + ColorUtility.ToHtmlStringRGBA(playerColor));
            PlayerPrefs.Save();
        }
        
        public Color32 GetPlayerColor() {

            string htmlString = PlayerPrefs.GetString(PlayerColorKey, string.Empty);
            
            if (ColorUtility.TryParseHtmlString(htmlString, out Color result))
            {
                return result;
            }
            else
            {
                return DefaultColor;
            }
        }
    }
}