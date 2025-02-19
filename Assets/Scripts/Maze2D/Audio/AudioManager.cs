using System;
using System.Collections.Generic;
using Maze2D.Domain;
using UniRx;
using UnityEngine;
using UnityEngine.Audio;
using VContainer;

namespace Maze2D.Audio
{
    public class AudioManager : MonoBehaviour
    {
        private const string MusicVolumeKey = "music_volume";
        private const string SoundsVolumeKey = "sounds_volume";
        
        [SerializeField] 
        private AudioSource _mainAudioSource;
        private AudioMixer _mainAudioMixer;

        [Inject] 
        private Lazy<Settings> _settings;

        private readonly ICollection<IDisposable> _disposables = new CompositeDisposable();
        
        private void Awake()
        {
            _mainAudioMixer = _mainAudioSource.outputAudioMixerGroup.audioMixer;
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

        public void PlayOneShot(AudioClip audioClip)
        {
            _mainAudioSource.PlayOneShot(audioClip);
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