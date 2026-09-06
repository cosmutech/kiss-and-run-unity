using UnityEngine;

namespace KissAndRun
{
    public class SpectatorCheer : MonoBehaviour
    {
        private float initialY;
        private float bobSpeed;
        private float bobHeight;
        private float phaseOffset;

        private void Start()
        {
            initialY = transform.position.y;
            bobSpeed = Random.Range(3.5f, 6.5f);
            bobHeight = Random.Range(0.12f, 0.28f);
            phaseOffset = Random.Range(0f, Mathf.PI * 2f);
        }

        private void Update()
        {
            // Cheering bounce animation
            float newY = initialY + Mathf.Abs(Mathf.Sin(Time.time * bobSpeed + phaseOffset)) * bobHeight;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
    }
}
