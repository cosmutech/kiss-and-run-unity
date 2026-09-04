using UnityEngine;

namespace KissAndRun
{
    public class HeartCoin : MonoBehaviour
    {
        [SerializeField] private float rotationSpeed = 120f;
        [SerializeField] private float magnetAttractSpeed = 15f;
        [SerializeField] private ParticleSystem pickupVFX;

        private bool isCollected = false;

        private void Update()
        {
            // Spin on Y axis
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);

            // Magnet attraction
            PlayerController player = GameManager.Instance?.Player;
            if (player != null && player.HasMagnet && !isCollected)
            {
                float distance = Vector3.Distance(transform.position, player.transform.position);
                if (distance < 12f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, player.transform.position, magnetAttractSpeed * Time.deltaTime);
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (isCollected) return;

            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                isCollected = true;
                if (pickupVFX)
                {
                    pickupVFX.transform.parent = null;
                    pickupVFX.Play();
                    Destroy(pickupVFX.gameObject, 1f);
                }

                SoundManager.Instance?.PlaySound("coin_pickup");
                GameManager.Instance.OnCoinCollected(1);
                Destroy(gameObject);
            }
        }
    }
}
