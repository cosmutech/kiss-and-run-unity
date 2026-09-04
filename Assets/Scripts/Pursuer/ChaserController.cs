using UnityEngine;

namespace KissAndRun
{
    public class ChaserController : MonoBehaviour
    {
        [Header("Chaser Settings")]
        [SerializeField] private Transform playerTransform;
        [SerializeField] private float maxDistance = 25f; // Safe distance ahead
        [SerializeField] private float minDistance = 1.5f; // Caught!
        [SerializeField] private float catchUpSpeed = 6f;
        [SerializeField] private float pullAwaySpeed = 2f;

        [Header("Visuals")]
        [SerializeField] private GameObject policeModel;
        [SerializeField] private GameObject angryGuyModel;
        [SerializeField] private ParticleSystem angryVFX;

        public bool IsActive { get; private set; } = false;
        public float CurrentDistance { get; private set; } = 20f;
        public string ChaserType { get; private set; } = "Angry Guy";

        private Animator animator;

        private void Awake()
        {
            animator = GetComponentInChildren<Animator>();
            gameObject.SetActive(false);
        }

        public void ActivateChaser(string typeName, bool isPolice = false)
        {
            IsActive = true;
            ChaserType = typeName;
            CurrentDistance = 15f; // Starts moderate distance behind
            gameObject.SetActive(true);

            if (policeModel) policeModel.SetActive(isPolice);
            if (angryGuyModel) angryGuyModel.SetActive(!isPolice);
            if (angryVFX) angryVFX.Play();

            SoundManager.Instance?.PlaySound(isPolice ? "police_siren" : "angry_scream");
        }

        public void DeactivateChaser()
        {
            IsActive = false;
            gameObject.SetActive(false);
            if (angryVFX) angryVFX.Stop();
        }

        private void Update()
        {
            if (!IsActive || playerTransform == null) return;

            // Follow player's forward direction and lane
            Vector3 targetPosition = new Vector3(
                playerTransform.position.x * 0.85f,
                playerTransform.position.y,
                playerTransform.position.z - CurrentDistance
            );

            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 12f);
            transform.rotation = playerTransform.rotation;

            // Player pulls away during clean running
            CurrentDistance += pullAwaySpeed * Time.deltaTime;

            if (CurrentDistance >= maxDistance)
            {
                // Escaped pursuer!
                GameManager.Instance.OnPursuerEscaped();
                DeactivateChaser();
            }
        }

        public void OnPlayerStumbled()
        {
            if (!IsActive) return;

            // Chaser closes in abruptly!
            CurrentDistance = Mathf.Max(minDistance, CurrentDistance - 6.5f);

            if (CurrentDistance <= minDistance + 0.5f)
            {
                // Caught!
                GameManager.Instance.OnPlayerCaught();
            }
        }
    }
}
