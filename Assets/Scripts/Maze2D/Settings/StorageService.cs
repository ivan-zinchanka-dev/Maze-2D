﻿using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Maze2D.Settings
{
    internal class StorageService : IDisposable
    {
        private const string DifficultyKey = "difficulty";
        private const string PlayerColorKey = "color";
        private const string MusicVolumeKey = "music_volume";
        private const string SoundsVolumeKey = "sounds_volume";
        
        private readonly Maze2D.Settings.Settings _defaults;
        public Lazy<Maze2D.Settings.Settings> Settings { get; private set; }
        
        private readonly ICollection<IDisposable> _disposables = new CompositeDisposable();
        
        public StorageService(Maze2D.Settings.Settings defaults)
        {
            _defaults = defaults;
            Settings = new Lazy<Maze2D.Settings.Settings>(CreateSettingsModel);
        }
        
        private Maze2D.Settings.Settings CreateSettingsModel()
        {
            var settings = new Maze2D.Settings.Settings(
                ReadGameDifficulty(),
                ReadPlayerColor(), 
                ReadMusicVolume(), 
                ReadSoundsVolume());

            settings.GameDifficulty.Subscribe(WriteGameDifficulty).AddTo(_disposables);
            settings.PlayerColor.Subscribe(WritePlayerColor).AddTo(_disposables);
            settings.MusicVolume.Subscribe(WriteMusicVolume).AddTo(_disposables);
            settings.SoundsVolume.Subscribe(WriteSoundsVolume).AddTo(_disposables);
            return settings;
        }
        
        private Difficulty ReadGameDifficulty() {
            
            return (Difficulty)PlayerPrefs.GetInt(DifficultyKey, (int)_defaults.GameDifficulty.Value);
        }
        
        private Color32 ReadPlayerColor() {

            string htmlString = PlayerPrefs.GetString(PlayerColorKey, string.Empty);
            
            if (ColorUtility.TryParseHtmlString(htmlString, out Color result))
            {
                return result;
            }

            return _defaults.PlayerColor.Value;
        }

        private float ReadMusicVolume()
        {
            return PlayerPrefs.GetFloat(MusicVolumeKey, _defaults.MusicVolume.Value);
        }
        
        private float ReadSoundsVolume()
        {
            return PlayerPrefs.GetFloat(SoundsVolumeKey, _defaults.SoundsVolume.Value);
        }

        private void WriteGameDifficulty(Difficulty difficultyLevel) {

            PlayerPrefs.SetInt(DifficultyKey, (int)difficultyLevel);
            PlayerPrefs.Save();
        }
        
        private void WritePlayerColor(Color32 playerColor)
        {
            PlayerPrefs.SetString(PlayerColorKey, "#" + ColorUtility.ToHtmlStringRGBA(playerColor));
            PlayerPrefs.Save();
        }
        
        private void WriteMusicVolume(float volume)
        {
            PlayerPrefs.SetFloat(MusicVolumeKey, volume);
            PlayerPrefs.Save();
        }
        
        private void WriteSoundsVolume(float volume)
        {
            PlayerPrefs.SetFloat(SoundsVolumeKey, volume);
            PlayerPrefs.Save();
        }
        
        public void Dispose()
        {
            _disposables.Clear();
        }
    }
}