using System.Collections.Generic;
using JanZinch.Services.Audio.Contracts;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VContainer;

namespace JanZinch.Services.Audio.Events
{
    [RequireComponent(typeof(Selectable))]
    public class OnSubmitAudioEvent : MonoBehaviour, ISubmitHandler, IPointerClickHandler
    {
        [SerializeField] 
        private List<AudioClip> _audioClips;
        [Inject] 
        private IAudioManger _audioManager;

        public void OnSubmit(BaseEventData eventData) => PlayRandomClip();
        public void OnPointerClick(PointerEventData eventData) => PlayRandomClip();
        
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