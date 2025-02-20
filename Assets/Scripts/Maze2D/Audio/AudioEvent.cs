using System.Collections.Generic;
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
        
        private float _lastPlayTime;

        public void PlayOneShot()
        {
            if (_audioClips.Count > 0 && Time.unscaledTime - _lastPlayTime > _cooldownSeconds)
            {
                int randomIndex = Random.Range(0, _audioClips.Count);
                _audioManager.PlayOneShot(_audioClips[randomIndex]);
                
                _lastPlayTime = Time.unscaledTime;
            }
        }
    }
}