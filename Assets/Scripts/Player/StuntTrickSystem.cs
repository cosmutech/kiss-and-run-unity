using System.Collections;
using UnityEngine;

namespace KissAndRun
{
    public class StuntTrickSystem : MonoBehaviour
    {
        [SerializeField] private Transform characterVisualMesh;
        [SerializeField] private ParticleSystem stuntSparklesVFX;

        private bool isPerformingStunt = false;

        private void Awake()
        {
            if (characterVisualMesh == null)
            {
                Renderer rend = GetComponentInChildren<Renderer>();
                if (rend != null && rend.transform != transform)
                {
                    characterVisualMesh = rend.transform;
                }
                else
                {
                    characterVisualMesh = transform;
                }
            }
        }

        public void TryPerformJumpStunt()
        {
            if (isPerformingStunt || characterVisualMesh == null) return;
            StartCoroutine(StuntRoutine());
        }

        private IEnumerator StuntRoutine()
        {
            isPerformingStunt = true;

            int trickType = Random.Range(0, 3);
            string trickName = "SICK FLIP! +150 ⭐";
            int bonus = 150;

            Quaternion originalRot = characterVisualMesh.localRotation;
            float duration = 0.55f;
            float elapsed = 0f;

            if (stuntSparklesVFX) stuntSparklesVFX.Play();
            SoundManager.Instance?.PlaySound("stunt_whoosh");

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                if (trickType == 0)
                {
                    // 360 Spin
                    trickName = "360 SPIN! +200 🌀";
                    bonus = 200;
                    characterVisualMesh.localRotation = originalRot * Quaternion.Euler(0, t * 360f, 0);
                }
                else if (trickType == 1)
                {
                    // Full Backflip
                    trickName = "RADICAL BACKFLIP! +250 🤸";
                    bonus = 250;
                    characterVisualMesh.localRotation = originalRot * Quaternion.Euler(t * 360f, 0, 0);
                }
                else
                {
                    // Barrel Roll
                    trickName = "BARREL ROLL! +300 🚀";
                    bonus = 300;
                    characterVisualMesh.localRotation = originalRot * Quaternion.Euler(0, 0, t * 360f);
                }

                yield return null;
            }

            characterVisualMesh.localRotation = originalRot;
            isPerformingStunt = false;

            // Award Stunt points & Comic text
            GameManager.Instance?.OnCoinCollected(5);
            GameManager.Instance?.Player?.GetComponent<PlayerController>()?.HasShield.ToString();
            SoundManager.Instance?.PlaySound("cheer");
        }
    }
}
