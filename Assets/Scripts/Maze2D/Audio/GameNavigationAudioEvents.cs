using System;
using System.Collections.Generic;
using JanZinch.Services.Audio.Contracts;
using Maze2D.Management;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using VContainer;

namespace Maze2D.Audio
{
    public class GameNavigationAudioEvents : MonoBehaviour
    {
        [SerializeField] 
        private GameNavigationManager _gameNavigationManager;
        [Space]
        [SerializeField] 
        private AudioClip _onPauseAudioClip;
        [SerializeField]
        private AudioClip _levelRegenerationAudioClip;
        [SerializeField]
        private AudioClip _levelFinishedAudioClip;
        [Inject] 
        private IAudioManger _audioManager;

        private readonly ICollection<IDisposable> _disposables = new CompositeDisposable();
        
        private void OnEnable()
        {
            BindSoundToEvent(_gameNavigationManager.OnPause, _onPauseAudioClip);
            BindSoundToEvent(_gameNavigationManager.OnLevelRegeneration, _levelRegenerationAudioClip);
            BindSoundToEvent(_gameNavigationManager.OnLevelFinished, _levelFinishedAudioClip);
        }

        private void BindSoundToEvent(UnityEvent sourceEvent, AudioClip sound)
        {
            sourceEvent.AsObservable()
                .Subscribe(u => _audioManager.PlayOneShot(sound))
                .AddTo(_disposables);
        }

        private void OnDisable()
        {
            _disposables.Clear();
        }
    }
}