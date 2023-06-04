using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sataura
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; }

        private static Dictionary<SoundType, float> soundTimerDictionary;
        public SoundAudioClip[] soundAudioClips;

        [SerializeField] private GameObject oneShotGameObject;
        private AudioSource oneShotAudioSource;

        private void Awake()
        {
            Instance = this;
            Initialized();
        }

        public void Initialized()
        {
            soundTimerDictionary = new Dictionary<SoundType, float>();
            soundTimerDictionary[SoundType.EnemyHit] = 0.0f;
            soundTimerDictionary[SoundType.EnemyDie] = 0.0f;
            soundTimerDictionary[SoundType.MainMenuBtnHover] = 0.0f;
            soundTimerDictionary[SoundType.ArrowProjectileHitGround] = 0.0f;
        }

        public void PlaySound(SoundType soundType, bool playRandom, float volume = 1.0f,float pitch = 1.0f)
        {
            if (CanPlaySound(soundType) == false) return;
            if(oneShotGameObject == null)
            {
                oneShotGameObject = new GameObject("Sound");
                oneShotAudioSource = oneShotGameObject.AddComponent<AudioSource>();
                oneShotAudioSource.volume = volume;
                oneShotAudioSource.pitch = pitch;
            }
            else
            {
                oneShotAudioSource = oneShotGameObject.GetComponent<AudioSource>();
                oneShotAudioSource.volume = volume;
                oneShotAudioSource.pitch = pitch;
            }

            if(playRandom)
            {
                oneShotAudioSource.PlayOneShot(GetRandomAudioClip(soundType));
            }
            else
            {
                oneShotAudioSource.PlayOneShot(GetFirstAudioClip(soundType));
            }
            
        }

        public void PlaySound(SoundType soundType, bool playRandom, Vector2 position)
        {
            if (CanPlaySound(soundType) == false) return;
            if (oneShotGameObject == null)
            {
                oneShotGameObject = new GameObject("Sound");
                oneShotAudioSource = oneShotGameObject.AddComponent<AudioSource>();
            }

            if (playRandom)
            {
                oneShotAudioSource.clip = GetRandomAudioClip(soundType);
            }
            else
            {
                oneShotAudioSource.clip = GetFirstAudioClip(soundType);
            }
            
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


        private AudioClip GetFirstAudioClip(SoundType soundType)
        {
            foreach (var soundAudioClip in soundAudioClips)
            {
                if (soundAudioClip.soundType.Equals(soundType))
                {
                    return soundAudioClip.audioClips[0];
                }
            }

            Debug.LogError($"Sound {soundType} not found!");
            return null;
        }

        private AudioClip GetRandomAudioClip(SoundType soundType)
        {
            foreach (var soundAudioClip in soundAudioClips)
            {
                if (soundAudioClip.soundType.Equals(soundType))
                {
                    return soundAudioClip.audioClips[Random.Range(0, soundAudioClip.audioClips.Count)];
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
                    return CanSoundTypePlay(soundType, 0.05f);
                case SoundType.EnemyDie:
                    return CanSoundTypePlay(soundType, 0.05f);
                case SoundType.MainMenuBtnHover:
                    return CanSoundTypePlay(soundType, 0.1f);
                case SoundType.ArrowProjectileHitGround:
                    return CanSoundTypePlay(soundType, 0.05f);
                default:
                    return true;
            }
        }

        private bool CanSoundTypePlay(SoundType soundType, float maxTimePlay)
        {
            if (soundTimerDictionary.ContainsKey(soundType))
            {
                float lastTimePlayed = soundTimerDictionary[soundType];
                if (lastTimePlayed + maxTimePlay < Time.time)
                {
                    soundTimerDictionary[soundType] = Time.time;
                    return true;
                }
                return false;
            }
            else
                return false;
        }
    }



    [System.Serializable]
    public class SoundAudioClip
    {
        public SoundType soundType;
        public List<AudioClip> audioClips;
    }

}
