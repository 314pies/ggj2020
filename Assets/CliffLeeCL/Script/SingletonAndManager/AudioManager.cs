using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

namespace CliffLeeCL
{
    /// <summary>
    /// The class that manage all audio source in the scene.
    /// </summary>
    public class AudioManager : SingletonMono<AudioManager>
    {
        public enum AudioName
        {
            ButtonClicked,
            EnemyDead1,
            EnemyDead2,
            EnemyDead3,
            EnemySlash,
            Fail,
            Pass
        }

        public AudioMixerGroup audioGroup;
        public AudioClip[] audioClips;
        public List<AudioSource> pooledSources;
        public int pooledAmount = 10;
        public float lowPitchRange = 0.95f, highPitchRange = 1.05f;
        public bool canGrow = true;

        // Use this for initialization
        void Start()
        {
            pooledSources = new List<AudioSource>();

            for (int i = 0; i < pooledAmount; i++)
            {
                AudioSource source = gameObject.AddComponent<AudioSource>();

                source.outputAudioMixerGroup = audioGroup;
                source.playOnAwake = false;
                pooledSources.Add(source);
            }
        }

        public AudioSource GetSoucre()
        {
            for (int i = 0; i < pooledSources.Count; i++)
            {
                if (!pooledSources[i].isPlaying)
                {
                    return pooledSources[i];
                }
            }

            if (canGrow)
            {
                AudioSource source = gameObject.AddComponent<AudioSource>();

                source.outputAudioMixerGroup = audioGroup;
                source.playOnAwake = false;
                pooledSources.Add(source);
                return source;
            }

            return null;
        }

        public void PlaySound(params AudioName[] name)
        {
            foreach (AudioName clipName in name)
            {
                if (audioClips[(int)clipName])
                {
                    AudioSource source = GetSoucre();

                    source.pitch = 1.0f;
                    source.PlayOneShot(audioClips[(int)clipName]);
                }
                else
                {
                    print("AudioManager : AudioClip[" + name.ToString() + "] is not setted");
                }
            }
        }

        public void PlaySoundRandomPitch(params AudioName[] name)
        {
            float randomPitch = Random.Range(lowPitchRange, highPitchRange);

            foreach (AudioName clipName in name)
            {
                if (audioClips[(int)clipName])
                {
                    AudioSource source = GetSoucre();

                    source.pitch = randomPitch;
                    source.PlayOneShot(audioClips[(int)clipName]);
                }
                else
                {
                    print("AudioManager : AudioClip[" + name.ToString() + "] is not setted");
                }
            }
        }

        public void PlaySoundRandomClip(params AudioName[] name)
        {
            int clipIndex = Random.Range((int)name[0], (int)name[name.Length - 1]);

            if (audioClips[(int)clipIndex])
            {
                AudioSource source = GetSoucre();

                source.pitch = 1.0f;
                source.PlayOneShot(audioClips[(int)clipIndex]);
            }
            else
            {
                print("AudioManager : AudioClip[" + name.ToString() + "] is not setted");
            }
        }

        public void PlaySoundRandomClipAndPitch(params AudioName[] name)
        {
            int clipIndex = Random.Range((int)name[0], (int)name[name.Length - 1] + 1);
            float randomPitch = Random.Range(lowPitchRange, highPitchRange);

            if (audioClips[(int)clipIndex])
            {
                AudioSource source = GetSoucre();

                source.pitch = randomPitch;
                source.PlayOneShot(audioClips[(int)clipIndex]);
            }
            else
            {
                print("AudioManager : AudioClip[" + name.ToString() + "] is not setted");
            }
        }



    }
}
