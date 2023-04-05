using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sataura
{
    public class SoundManager : Singleton<SoundManager>
    {
        private static Dictionary<SoundType, float> soundTimerDictionary;
        public SoundAudioClip[] soundAudioClips;

        [SerializeField] private GameObject oneShotGameObject;
        private AudioSource oneShotAudioSource;

        private void Awake()
        {
            Initialized();
        }

        public void Initialized()
        {
            soundTimerDictionary = new Dictionary<SoundType, float>();
            soundTimerDictionary[SoundType.EnemyHit] = 0.0f;
            soundTimerDictionary[SoundType.EnemyDie] = 0.0f;
            soundTimerDictionary[SoundType.MainMenuBtnHover] = 0.0f;
        }

        public void PlaySound(SoundType soundType)
        {
            if (CanPlaySound(soundType) == false) return;
            if(oneShotGameObject == null)
            {
                oneShotGameObject = new GameObject("Sound");
                oneShotAudioSource = oneShotGameObject.AddComponent<AudioSource>();
            }
            else
            {
                oneShotAudioSource = oneShotGameObject.GetComponent<AudioSource>();
            }

            oneShotAudioSource.PlayOneShot(GetAudioClip(soundType));
        }

        public void PlaySound(SoundType soundType, Vector2 position)
        {
            if (CanPlaySound(soundType) == false) return;
            if (oneShotGameObject == null)
            {
                oneShotGameObject = new GameObject("Sound");
                oneShotAudioSource = oneShotGameObject.AddComponent<AudioSource>();
            }
            oneShotAudioSource.clip = GetAudioClip(soundType);
            oneShotAudioSource.Play(); 
        }

        public void PlaySound(SoundType soundType, AudioClip audioClip)
        {
            if (CanPlaySound(soundType) == false) return;
            if (oneShotGameObject == null)
            {
                oneShotGameObject = new GameObject("Sound");
                oneShotAudioSource = oneShotGameObject.AddComponent<AudioSource>();
            }
            oneShotAudioSource.PlayOneShot(audioClip);
        }


        private AudioClip GetAudioClip(SoundType soundType)
        {
            foreach (var soundAudioClip in soundAudioClips)
            {
                if (soundAudioClip.soundType.Equals(soundType))
                {
                    return soundAudioClip.audioClip;
                }
            }

            Debug.LogError($"Sound {soundType} not found!");
            return null;
        }


        private bool CanPlaySound(SoundType soundType)
        {
            switch (soundType)
            {
                case SoundType.EnemyHit:
                    if (soundTimerDictionary.ContainsKey(soundType))
                    {
                        float lastTimePlayed = soundTimerDictionary[soundType];
                        float maxTimePlay = .05f;
                        if (lastTimePlayed + maxTimePlay < Time.time)
                        {
                            soundTimerDictionary[soundType] = Time.time;
                            return true;
                        }
                        return false;
                    }
                    else
                        return false;
                case SoundType.EnemyDie:
                    if (soundTimerDictionary.ContainsKey(soundType))
                    {
                        float lastTimePlayed = soundTimerDictionary[soundType];
                        float maxTimePlay = .05f;
                        if (lastTimePlayed + maxTimePlay < Time.time)
                        {
                            soundTimerDictionary[soundType] = Time.time;
                            return true;
                        }
                        return false;
                    }
                    else
                        return false;
                case SoundType.MainMenuBtnHover:
                    if (soundTimerDictionary.ContainsKey(soundType))
                    {
                        float lastTimePlayed = soundTimerDictionary[soundType];
                        float maxTimePlay = .1f;
                        if (lastTimePlayed + maxTimePlay < Time.time)
                        {
                            soundTimerDictionary[soundType] = Time.time;
                            return true;
                        }
                        return false;
                    }
                    else
                        return false;
                default:
                    return true;
            }
        }

        /*private void SetSoundTypePlayFrequency(SoundType soundType, float frequency)
        {
            if (soundTimerDictionary.ContainsKey(soundType))
            {
                float lastTimePlayed = soundTimerDictionary[soundType];
                float maxTimePlay = .5f;
                if (lastTimePlayed + maxTimePlay < Time.time)
                {
                    soundTimerDictionary[soundType] = Time.time;
                }
                else
                {
                }
            }
        }*/
    }



    [System.Serializable]
    public class SoundAudioClip
    {
        public SoundType soundType;
        public AudioClip audioClip;
    }

}
