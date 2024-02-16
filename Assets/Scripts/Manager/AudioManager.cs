using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public enum SoundType
    {
        soundBG,
    }
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] AudioSource soundSource;
        public List<AudioClip> listSound;
        public void PlaySound(SoundType type)
        {
            soundSource.clip = listSound[(int)type];
            soundSource.Play();
        }
    }
}
