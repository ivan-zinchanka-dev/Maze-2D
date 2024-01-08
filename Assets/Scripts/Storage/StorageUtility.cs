using System;
using System.Linq;
using Game;
using UnityEngine;

namespace Storage
{
    static class StorageUtility
    {
        private const string DifficultyKey = "difficulty";
        private const string PlayerColorKey = "color";
        
        private const Difficulty DefaultDifficulty = Difficulty.Normal;
        private const int DefaultIndex = -1;

        private static Color[] _playerColors = Array.Empty<Color>();
        
        public static void Initialize(Color[] playerColors)
        {
            _playerColors = playerColors;
        }
        
        public static void SetDifficulty(Difficulty difficultyLevel) {

            PlayerPrefs.SetInt(DifficultyKey, (int)difficultyLevel);
            PlayerPrefs.Save();
        }

        public static Difficulty GetDifficulty() {
            
            return (Difficulty)PlayerPrefs.GetInt(DifficultyKey, (int)DefaultDifficulty);
        }
        
        public static void SetPlayerColor(Color playerColor)
        {
            Debug.Log("Saved: " + ColorUtility.ToHtmlStringRGBA(playerColor));
            
            PlayerPrefs.SetString(PlayerColorKey, "#" + ColorUtility.ToHtmlStringRGBA(playerColor));
            PlayerPrefs.Save();
        }
        
        public static Color GetPlayerColor() {

            string htmlString = PlayerPrefs.GetString(PlayerColorKey, string.Empty);

            Debug.Log("Read: " + htmlString);
            
            if (ColorUtility.TryParseHtmlString(htmlString, out Color result))
            {
                Debug.Log("Success");
                return result;
            }
            else
            {
                Debug.Log("Fail");
                return default;
            }
            
        }
        
        /*public static Color GetPlayerColor()
        {
            int index = GetPlayerColorIndex();

            if (index > 0 && index < _playerColors.Length)
            {
                return _playerColors[index];
            }
            else
            {
                return Color.white;
            }
        }*/
    }
    
}