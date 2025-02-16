using System;
using UnityEngine;

namespace Maze2D.Domain
{
    public class StorageService
    {
        private const string DifficultyKey = "difficulty";
        private const string PlayerColorKey = "color";
        private const string MusicVolumeKey = "music_volume";
        private const string SoundsVolumeKey = "sounds_volume";
        
        private static readonly Settings Defaults = new Settings(
            Difficulty.Easy,
            new Color(1.0f, 0.1680528f, 0.0f, 1.0f),
            0.8f, 1.0f);

        public Lazy<Settings> Settings { get; private set; }
        
        public StorageService()
        {
            Settings = new Lazy<Settings>(CreateSettingsModel);
        }
        
        private Settings CreateSettingsModel()
        {
            return new Settings(
                ReadGameDifficulty(),
                ReadPlayerColor(), 
                Defaults.MusicVolume, 
                Defaults.SoundsVolume,
                SaveSettingsModel);
        }
        
        private void SaveSettingsModel()
        {
            WriteDifficulty(Settings.Value.GameDifficulty);
            WritePlayerColor(Settings.Value.PlayerColor);
            SaveWritings();
        }

        private Difficulty ReadGameDifficulty() {
            
            return (Difficulty)PlayerPrefs.GetInt(DifficultyKey, (int)Defaults.GameDifficulty);
        }
        
        private Color32 ReadPlayerColor() {

            string htmlString = PlayerPrefs.GetString(PlayerColorKey, string.Empty);
            
            if (ColorUtility.TryParseHtmlString(htmlString, out Color result))
            {
                return result;
            }

            return Defaults.PlayerColor;
        }
        
        private void WriteDifficulty(Difficulty difficultyLevel) {

            PlayerPrefs.SetInt(DifficultyKey, (int)difficultyLevel);
        }
        
        private void WritePlayerColor(Color32 playerColor)
        {
            PlayerPrefs.SetString(PlayerColorKey, "#" + ColorUtility.ToHtmlStringRGBA(playerColor));
        }

        private void SaveWritings()
        {
            PlayerPrefs.Save();
        }
    }
}