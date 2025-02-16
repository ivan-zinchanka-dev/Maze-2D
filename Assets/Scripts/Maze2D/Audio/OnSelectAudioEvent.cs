using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VContainer;

namespace Maze2D.Audio
{
    [RequireComponent(typeof(Selectable))]
    public class OnSelectAudioEvent : MonoBehaviour, ISelectHandler, IPointerEnterHandler
    {
        [SerializeField] 
        private List<AudioClip> _audioClips;
        [Inject] 
        private AudioManager _audioManager;
        
        public void OnSelect(BaseEventData eventData) => PlayRandomClip();
        public void OnPointerEnter(PointerEventData eventData) => PlayRandomClip();
        
        private void PlayRandomClip()
        {
            if (_audioClips.Count > 0)
            {
                int randomIndex = Random.Range(0, _audioClips.Count);
                _audioManager.PlayOneShot(_audioClips[randomIndex]);
            }
        }
    }
}