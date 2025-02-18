using System;
using UniRx;
using UnityEngine;

namespace Maze2D.Domain
{
    public class Settings
    {
        private readonly ReactiveProperty<Difficulty> _gameDifficulty;
        private readonly ReactiveProperty<Color32> _playerColor;
        private readonly ReactiveProperty<float> _musicVolume;
        private readonly ReactiveProperty<float> _soundsVolume;
        
        public IReactiveProperty<Difficulty> GameDifficulty => _gameDifficulty;
        public IReactiveProperty<Color32> PlayerColor => _playerColor;
        public IReactiveProperty<float> MusicVolume => _musicVolume;
        public IReactiveProperty<float> SoundsVolume =>_soundsVolume;
        
        private event Action OnSave;
        
        public Settings(
            Difficulty gameDifficulty, 
            Color32 playerColor, 
            float musicVolume, 
            float soundsVolume,
            Action onSave = null)
        {
            _gameDifficulty = new ReactiveProperty<Difficulty>(gameDifficulty);
            _playerColor = new ReactiveProperty<Color32>(playerColor);
            _musicVolume = new ReactiveProperty<float>(musicVolume);
            _soundsVolume = new ReactiveProperty<float>(soundsVolume);
            OnSave = onSave;
        }
        
        public void Save() => OnSave?.Invoke();
    }
}