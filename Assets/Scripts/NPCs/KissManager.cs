using System;
using UnityEngine;

namespace KissAndRun
{
    public class KissManager : MonoBehaviour
    {
        public static KissManager Instance { get; private set; }

        public static event Action<bool> OnKissTargetAvailable;

        [Header("Detection Settings")]
        [SerializeField] private Transform playerTransform;
        [SerializeField] private float kissMaxRange = 4.2f;
        [SerializeField] private float laneTolerance = 1.3f;

        private NPCController currentTarget;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void OnEnable()
        {
            SwipeDetector.OnTap += TryPerformKiss;
        }

        private void OnDisable()
        {
            SwipeDetector.OnTap -= TryPerformKiss;
        }

        private void Update()
        {
            // Do not search for new targets while in a cinematic close-up
            if (KissCinematicDirector.Instance != null && KissCinematicDirector.Instance.IsInCinematic)
            {
                return;
            }

            DetectNearbyNPC();
        }

        private void DetectNearbyNPC()
        {
            if (playerTransform == null) return;

            Collider[] hits = Physics.OverlapSphere(playerTransform.position + Vector3.forward * 1.5f, kissMaxRange);
            NPCController bestTarget = null;
            float closestZDist = float.MaxValue;

            foreach (var hit in hits)
            {
                NPCController npc = hit.GetComponent<NPCController>();
                if (npc != null && npc.CanBeKissed)
                {
                    float deltaX = Mathf.Abs(npc.transform.position.x - playerTransform.position.x);
                    float deltaZ = npc.transform.position.z - playerTransform.position.z;

                    if (deltaZ > 0.3f && deltaZ < kissMaxRange && deltaX <= laneTolerance)
                    {
                        if (deltaZ < closestZDist)
                        {
                            closestZDist = deltaZ;
                            bestTarget = npc;
                        }
                    }
                }
            }

            if (bestTarget != currentTarget)
            {
                if (currentTarget != null) currentTarget.SetKissPromptActive(false);
                currentTarget = bestTarget;
                if (currentTarget != null) currentTarget.SetKissPromptActive(true);

                OnKissTargetAvailable?.Invoke(currentTarget != null);
            }
        }

        public void TryPerformKiss()
        {
            if (currentTarget != null && currentTarget.CanBeKissed)
            {
                NPCController targetToKiss = currentTarget;
                currentTarget = null;
                OnKissTargetAvailable?.Invoke(false);

                // Check if this NPC reaction will be a slap
                bool isSlap = UnityEngine.Random.value < targetToKiss.slapChance;

                // Play Dramatic Slow-Mo Bullet-Time Cinematic!
                if (KissCinematicDirector.Instance != null)
                {
                    KissCinematicDirector.Instance.PlayKissCinematic(
                        playerTransform: playerTransform,
                        npcTransform: targetToKiss.transform,
                        onKissImpact: () => {
                            targetToKiss.ExecuteKissReaction();
                        },
                        onCinematicComplete: () => {
                            // Returned to full running gameplay
                        },
                        isSlap: isSlap
                    );
                }
                else
                {
                    targetToKiss.ExecuteKissReaction();
                }
            }
        }
    }
}
