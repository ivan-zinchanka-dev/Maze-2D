using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace Maze2D.Audio
{
    public class AudioEvent : MonoBehaviour
    {
        [SerializeField] 
        private List<AudioClip> _audioClips;
        [Inject] 
        private AudioManager _audioManager;
        
        public void PlayOneShot()
        {
            if (_audioClips.Count > 0)
            {
                int randomIndex = Random.Range(0, _audioClips.Count);
                _audioManager.PlayOneShot(_audioClips[randomIndex]);
            }
        }
    }
}