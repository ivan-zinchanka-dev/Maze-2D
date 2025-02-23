using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JanZinch.Services.Audio.Contracts;
using UniRx;
using UnityEngine;
using UnityEngine.Audio;
using VContainer;

namespace Maze2D.Audio
{
    internal class AudioManager : MonoBehaviour, IAudioManger
    {
        private const string MusicVolumeKey = "music_volume";
        private const string SoundsVolumeKey = "sounds_volume";
        
        [SerializeField] 
        private AudioSource _mainAudioSource;
        [SerializeField] 
        private int _maxConcurrentSounds = 250;
        [Inject] 
        private Lazy<Settings.Settings> _settings;

        private AudioMixer _mainAudioMixer;
        private readonly LinkedList<Task> _audioTasks = new LinkedList<Task>();
        private readonly ICollection<IDisposable> _disposables = new CompositeDisposable();
        
        public void PlayOneShot(AudioClip audioClip)
        {
            if (_audioTasks.Count > _maxConcurrentSounds)
            {
                return;
            }

            AddAudioTask(audioClip.length);
            
            _mainAudioSource.PlayOneShot(audioClip);
        }
        
        private void Awake()
        {
            _mainAudioMixer = _mainAudioSource.outputAudioMixerGroup.audioMixer;
            _mainAudioMixer.updateMode = AudioMixerUpdateMode.UnscaledTime;
        }

        private void OnEnable()
        {
            _settings.Value.MusicVolume.Subscribe(SetMusicVolumeToMixer).AddTo(_disposables);
            _settings.Value.SoundsVolume.Subscribe(SetSoundsVolumeToMixer).AddTo(_disposables);
        }

        private void Start()
        {
            SetAudioSettingsToMixer();
        }

        private void AddAudioTask(float clipDurationSeconds)
        {
            Task audioTask = Task.Delay(TimeSpan.FromSeconds(clipDurationSeconds));
            audioTask.ContinueWith(task =>
            {
                _audioTasks.Remove(audioTask);
            }, TaskScheduler.FromCurrentSynchronizationContext());
            
            _audioTasks.AddLast(audioTask);
        }

        private void SetMusicVolumeToMixer(float volume)
        {
            _mainAudioMixer.SetFloat(MusicVolumeKey, LinearToDecibel(volume));
        }
        
        private void SetSoundsVolumeToMixer(float volume)
        {
            _mainAudioMixer.SetFloat(SoundsVolumeKey, LinearToDecibel(volume));
        }

        private void SetAudioSettingsToMixer()
        {
            SetMusicVolumeToMixer(_settings.Value.MusicVolume.Value);
            SetSoundsVolumeToMixer(_settings.Value.SoundsVolume.Value);
        }

        private static float LinearToDecibel(float linear)
        {
            if (linear != 0)
            {
                return 20.0f * Mathf.Log10(linear);
            }
            else
            {
                return -144.0f;
            }
        }

        private void OnDisable()
        {
            _disposables.Clear();
        }
    }
}