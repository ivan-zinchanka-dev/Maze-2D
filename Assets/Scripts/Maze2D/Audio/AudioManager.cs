using UnityEngine;

namespace Maze2D.Audio
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioSource _mainAudioSource;
        
        public void PlayOneShot(AudioClip audioClip)
        {
            _mainAudioSource.PlayOneShot(audioClip);
        }
    }
}