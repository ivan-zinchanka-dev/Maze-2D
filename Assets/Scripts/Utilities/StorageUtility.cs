using Game;
using UnityEngine;

namespace Utilities
{
    static class StorageUtility
    {
        private const string DifficultyKey = "difficulty";
        private const string PlayerColorKey = "color";
        
        private const Difficulty DefaultDifficulty = Difficulty.Normal;
        
        public static void SetDifficulty(Difficulty difficultyLevel) {

            PlayerPrefs.SetInt(DifficultyKey, (int)difficultyLevel);
            PlayerPrefs.Save();
        }

        public static Difficulty GetDifficulty() {
            
            return (Difficulty)PlayerPrefs.GetInt(DifficultyKey, (int)DefaultDifficulty);
        }
        
        public static void SetPlayerColor(Color playerColor)
        {
            PlayerPrefs.SetString(PlayerColorKey, "#" + ColorUtility.ToHtmlStringRGBA(playerColor));
            PlayerPrefs.Save();
        }
        
        public static Color GetPlayerColor() {

            string htmlString = PlayerPrefs.GetString(PlayerColorKey, string.Empty);
            
            if (ColorUtility.TryParseHtmlString(htmlString, out Color result))
            {
                return result;
            }
            else
            {
                return default;
            }
        }
    }
    
}