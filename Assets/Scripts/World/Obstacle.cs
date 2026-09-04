using UnityEngine;

namespace KissAndRun
{
    public enum ObstacleType
    {
        RoadblockLow,      // Must JUMP over!
        OverheadBarrier,   // Must SLIDE under!
        BananaPeel,        // Slip and stumble!
        FoodCart           // Domino chain crash!
    }

    public class Obstacle : MonoBehaviour
    {
        public ObstacleType obstacleType;
        [SerializeField] private ParticleSystem collisionVFX;

        private bool hasCollided = false;

        private void OnTriggerEnter(Collider other)
        {
            if (hasCollided) return;

            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                hasCollided = true;
                if (collisionVFX) collisionVFX.Play();

                player.TakeDamage(1);

                // Alert pursuer to close in immediately!
                ChaserController chaser = FindObjectOfType<ChaserController>();
                if (chaser != null)
                {
                    chaser.OnPlayerStumbled();
                }

                if (obstacleType == ObstacleType.BananaPeel)
                {
                    SoundManager.Instance?.PlaySound("banana_slip");
                }
                else
                {
                    SoundManager.Instance?.PlaySound("crash");
                }
            }
        }
    }
}
