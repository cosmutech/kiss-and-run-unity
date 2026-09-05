using UnityEngine;

namespace KissAndRun
{
    public class DominoCrashProp : MonoBehaviour
    {
        [SerializeField] private GameObject normalModel;
        [SerializeField] private GameObject shatteredModel;
        [SerializeField] private float explosionForce = 350f;
        [SerializeField] private float explosionRadius = 4.5f;

        private bool hasExploded = false;

        private void OnTriggerEnter(Collider other)
        {
            if (hasExploded) return;

            // Triggered if hit by Player or Chaser
            if (other.CompareTag("Player") || other.GetComponent<ChaserController>() != null)
            {
                TriggerDominoCrash();
            }
        }

        public void TriggerDominoCrash()
        {
            if (hasExploded) return;
            hasExploded = true;

            if (normalModel) normalModel.SetActive(false);

            if (shatteredModel)
            {
                shatteredModel.SetActive(true);
                Rigidbody[] pieces = shatteredModel.GetComponentsInChildren<Rigidbody>();
                foreach (var rb in pieces)
                {
                    rb.isKinematic = false;
                    rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, 1.2f, ForceMode.Impulse);
                }
            }

            SoundManager.Instance?.PlaySound("crash");
            KissCinematicDirector.Instance?.GetComponent<ThirdPersonCameraFollow>()?.TriggerShake(0.5f);
        }
    }
}
