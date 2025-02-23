using UnityEngine;

namespace JanZinch.Services.Audio.Contracts
{
    public interface IAudioManger
    {
        public void PlayOneShot(AudioClip audioClip);
    }
}