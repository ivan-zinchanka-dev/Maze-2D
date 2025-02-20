using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using Random = UnityEngine.Random;

namespace Maze2D.Audio
{
    public class AudioEvent : MonoBehaviour
    {
        [SerializeField] 
        private List<AudioClip> _audioClips;
        [Space]
        [SerializeField] [Range(0.15f, 1f)]
        private float _cooldownSeconds = 0.15f;
        
        [Inject] 
        private AudioManager _audioManager;
        
        private TimeSpan _cooldown;
        private UniTask _cooldownTask;
        
        private void Awake()
        {
            _cooldown = TimeSpan.FromSeconds(_cooldownSeconds);
        }

        public void PlayOneShot()
        {
            if (_audioClips.Count > 0 && _cooldownTask.Status != UniTaskStatus.Pending)
            {
                int randomIndex = Random.Range(0, _audioClips.Count);
                _audioManager.PlayOneShot(_audioClips[randomIndex]);
                
                _cooldownTask = UniTask.Delay(_cooldown, DelayType.UnscaledDeltaTime);
            }
        }
    }
}