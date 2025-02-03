using UnityEditor.Graphs;
using UnityEngine;

namespace Maze2D.Game
{
    public class StorageService
    {
        private const string DifficultyKey = "difficulty";
        private const string PlayerColorKey = "color";
        
        private readonly Difficulty _defaultDifficulty;
        private static readonly Color DefaultColor = Color.red;

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
        
        public void SetPlayerColor(Color playerColor)
        {
            PlayerPrefs.SetString(PlayerColorKey, "#" + ColorUtility.ToHtmlStringRGBA(playerColor));
            PlayerPrefs.Save();
        }
        
        public Color GetPlayerColor() {

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