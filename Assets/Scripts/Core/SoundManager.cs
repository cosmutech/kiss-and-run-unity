using System.Collections.Generic;
using UnityEngine;

namespace KissAndRun
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; }

        [System.Serializable]
        public struct NamedAudioClip
        {
            public string name;
            public AudioClip clip;
        }

        [SerializeField] private NamedAudioClip[] audioClips;
        [SerializeField] private AudioSource sfxSource;

        private readonly Dictionary<string, AudioClip> clipMap = new Dictionary<string, AudioClip>();

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            if (audioClips != null)
            {
                foreach (var item in audioClips)
                {
                    if (!clipMap.ContainsKey(item.name))
                        clipMap.Add(item.name, item.clip);
                }
            }
        }

        public void PlaySound(string soundName)
        {
            if (!SaveManager.IsSoundEnabled()) return;

            if (clipMap.TryGetValue(soundName, out AudioClip clip) && sfxSource != null)
            {
                sfxSource.PlayOneShot(clip);
            }
        }
    }
}
