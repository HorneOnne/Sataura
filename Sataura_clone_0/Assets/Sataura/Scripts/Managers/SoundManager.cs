using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sataura
{
    public class SoundManager : Singleton<SoundManager>
    {
        public static void PlaySound(AudioClip audioClip)
        {
            GameObject soundGameObject = new GameObject("Sound");
            AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
            audioSource.PlayOneShot(audioClip);
        }
    }

}
