using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Maze2D.Audio
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioSource _mainAudioSource;

        private AudioMixer _mainAudioMixer;
        
        private void Awake()
        {
            _mainAudioMixer = _mainAudioSource.outputAudioMixerGroup.audioMixer;
        }

        public void PlayOneShot(AudioClip audioClip)
        {
            _mainAudioSource.PlayOneShot(audioClip);
        }
    }
}