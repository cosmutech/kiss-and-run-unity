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
        [SerializeField] private float kissMaxRange = 3.5f;
        [SerializeField] private float laneTolerance = 1.2f;

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
            DetectNearbyNPC();
        }

        private void DetectNearbyNPC()
        {
            if (playerTransform == null) return;

            // Find all NPCs within range
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

                    // Must be in front of player and in same/adjacent lane
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
                currentTarget.ExecuteKissReaction();
                currentTarget = null;
                OnKissTargetAvailable?.Invoke(false);
            }
        }
    }
}
