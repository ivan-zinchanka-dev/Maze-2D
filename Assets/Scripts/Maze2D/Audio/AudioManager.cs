using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
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
        private StorageService _storageService;
        private Settings _settings;

        private readonly ICollection<IDisposable> _disposables = new CompositeDisposable();
        
        private void Awake()
        {
            _mainAudioMixer = _mainAudioSource.outputAudioMixerGroup.audioMixer;
            _settings = _storageService.Settings.Value;
        }

        private void OnEnable()
        {
            _settings.MusicVolume.Subscribe(SetMusicVolumeToMixer).AddTo(_disposables);
            _settings.SoundsVolume.Subscribe(SetSoundsVolumeToMixer).AddTo(_disposables);
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
            SetMusicVolumeToMixer(_settings.MusicVolume.Value);
            SetSoundsVolumeToMixer(_settings.SoundsVolume.Value);
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