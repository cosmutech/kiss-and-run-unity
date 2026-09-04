using UnityEngine;

namespace KissAndRun
{
    public enum PowerUpType
    {
        HeartShield,
        SpeedBoost,
        HeartMagnet,
        SlowMotion,
        AngelMode
    }

    public class PowerUpItem : MonoBehaviour
    {
        [SerializeField] private PowerUpType type;
        [SerializeField] private float duration = 7f;
        [SerializeField] private float bobbingSpeed = 4f;
        [SerializeField] private float bobbingHeight = 0.3f;
        [SerializeField] private ParticleSystem pickupVFX;

        private Vector3 startPos;
        private bool isCollected = false;

        private void Start()
        {
            startPos = transform.position;
        }

        private void Update()
        {
            // Hover bob and rotate
            transform.Rotate(Vector3.up, 90f * Time.deltaTime, Space.World);
            float newY = startPos.y + Mathf.Sin(Time.time * bobbingSpeed) * bobbingHeight;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (isCollected) return;

            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                isCollected = true;
                ApplyPowerUp(player);

                if (pickupVFX)
                {
                    pickupVFX.transform.parent = null;
                    pickupVFX.Play();
                    Destroy(pickupVFX.gameObject, 1f);
                }

                SoundManager.Instance?.PlaySound("powerup");
                Destroy(gameObject);
            }
        }

        private void ApplyPowerUp(PlayerController player)
        {
            switch (type)
            {
                case PowerUpType.HeartShield:
                    player.HasShield = true;
                    GameManager.Instance?.OnCoinCollected(5);
                    break;
                case PowerUpType.SpeedBoost:
                    player.SpeedBoostTimer = duration;
                    break;
                case PowerUpType.HeartMagnet:
                    player.MagnetTimer = duration;
                    break;
                case PowerUpType.SlowMotion:
                    player.SlowMoTimer = duration;
                    break;
                case PowerUpType.AngelMode:
                    player.InvincibleTimer = duration;
                    break;
            }
        }
    }
}
