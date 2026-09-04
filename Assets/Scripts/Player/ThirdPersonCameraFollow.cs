using UnityEngine;

namespace KissAndRun
{
    public class ThirdPersonCameraFollow : MonoBehaviour
    {
        [Header("Target & Offsets")]
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset = new Vector3(0f, 4.2f, -6.5f);
        [SerializeField] private float smoothSpeedX = 12f;
        [SerializeField] private float smoothSpeedY = 6f;

        [Header("Camera Shake")]
        [SerializeField] private float shakeDecay = 3f;

        private float shakeIntensity = 0f;

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }

        public void TriggerShake(float intensity = 0.5f)
        {
            shakeIntensity = intensity;
        }

        private void LateUpdate()
        {
            if (target == null) return;

            // 1. Follow Z strictly to never lag behind player speed
            float targetZ = target.position.z + offset.z;

            // 2. Smoothly follow X lane switches
            float targetX = Mathf.Lerp(transform.position.x, target.position.x * 0.7f + offset.x, Time.deltaTime * smoothSpeedX);

            // 3. Follow Y with subtle damping
            float targetY = Mathf.Lerp(transform.position.y, target.position.y + offset.y, Time.deltaTime * smoothSpeedY);

            Vector3 finalPos = new Vector3(targetX, targetY, targetZ);

            // 4. Screen Shake offset
            if (shakeIntensity > 0)
            {
                finalPos += (Vector3)Random.insideUnitCircle * shakeIntensity;
                shakeIntensity = Mathf.MoveTowards(shakeIntensity, 0f, Time.deltaTime * shakeDecay);
            }

            transform.position = finalPos;

            // Look slightly ahead of player's head
            Vector3 lookTarget = target.position + Vector3.up * 1.5f + Vector3.forward * 4f;
            transform.rotation = Quaternion.LookRotation(lookTarget - transform.position);
        }
    }
}
