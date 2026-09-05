using UnityEngine;

namespace KissAndRun
{
    public class JumpRamp : MonoBehaviour
    {
        [SerializeField] private float launchVelocity = 14f;
        [SerializeField] private ParticleSystem rampSparksVFX;

        private void OnTriggerEnter(Collider other)
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                if (rampSparksVFX) rampSparksVFX.Play();

                // Launch player high into aerial trick trajectory
                player.Jump();

                // Trigger stunt trick
                StuntTrickSystem stunt = player.GetComponentInChildren<StuntTrickSystem>();
                if (stunt != null)
                {
                    stunt.TryPerformJumpStunt();
                }

                SoundManager.Instance?.PlaySound("ramp_launch");
            }
        }
    }
}
