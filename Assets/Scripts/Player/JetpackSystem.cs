using System;
using System.Collections;
using UnityEngine;

namespace KissAndRun
{
    public class JetpackSystem : MonoBehaviour
    {
        public static JetpackSystem Instance { get; private set; }

        public static event Action<bool> OnJetpackStateChanged;
        public static event Action<float> OnJetpackProgressUpdate;

        [Header("Jetpack Flight Settings")]
        [SerializeField] private float flightHeight = 8.5f;
        [SerializeField] private float flightDuration = 8f;
        [SerializeField] private float ascentSpeed = 7f;
        [SerializeField] private float flightSpeedMultiplier = 1.35f;

        [Header("Visuals")]
        [SerializeField] private GameObject jetpackModel;
        [SerializeField] private ParticleSystem leftExhaustVFX;
        [SerializeField] private ParticleSystem rightExhaustVFX;

        public bool IsFlying { get; private set; } = false;

        private PlayerController player;
        private CharacterController charController;
        private float originalY;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            player = GetComponentInParent<PlayerController>();
            charController = GetComponentInParent<CharacterController>();

            if (jetpackModel) jetpackModel.SetActive(false);
            StopExhaust();
        }

        public void ActivateJetpack()
        {
            if (IsFlying) return;
            StartCoroutine(JetpackFlightRoutine());
        }

        private IEnumerator JetpackFlightRoutine()
        {
            IsFlying = true;
            OnJetpackStateChanged?.Invoke(true);

            if (jetpackModel == null) CreateProceduralJetpack();
            if (jetpackModel) jetpackModel.SetActive(true);
            StartExhaust();
            SoundManager.Instance?.PlaySound("jetpack_boost");

            // Ascend to sky altitude
            float targetY = flightHeight;
            float elapsed = 0f;

            while (player.transform.position.y < targetY - 0.2f)
            {
                player.transform.position += Vector3.up * (ascentSpeed * Time.deltaTime);
                yield return null;
            }

            // Fly forward high in the sky collecting coins safely
            elapsed = 0f;
            while (elapsed < flightDuration)
            {
                elapsed += Time.deltaTime;
                OnJetpackProgressUpdate?.Invoke(1f - (elapsed / flightDuration));

                // Lock height in sky
                Vector3 pos = player.transform.position;
                pos.y = flightHeight;
                player.transform.position = pos;

                yield return null;
            }

            // Smooth descent back to road surface
            StopExhaust();
            SoundManager.Instance?.PlaySound("jetpack_down");

            while (player.transform.position.y > 0.05f)
            {
                player.transform.position += Vector3.down * (ascentSpeed * 0.8f * Time.deltaTime);
                yield return null;
            }

            if (jetpackModel) jetpackModel.SetActive(false);
            IsFlying = false;
            OnJetpackStateChanged?.Invoke(false);
        }

        private void StartExhaust()
        {
            if (leftExhaustVFX) leftExhaustVFX.Play();
            if (rightExhaustVFX) rightExhaustVFX.Play();
        }

        private void StopExhaust()
        {
            if (leftExhaustVFX) leftExhaustVFX.Stop();
            if (rightExhaustVFX) rightExhaustVFX.Stop();
        }

        private void CreateProceduralJetpack()
        {
            jetpackModel = new GameObject("JetpackModel");
            jetpackModel.transform.parent = transform;
            jetpackModel.transform.localPosition = new Vector3(0, 0.4f, -0.3f);

            // Left tank
            GameObject leftTank = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            leftTank.transform.parent = jetpackModel.transform;
            leftTank.transform.localPosition = new Vector3(-0.25f, 0, 0);
            leftTank.transform.localScale = new Vector3(0.18f, 0.45f, 0.18f);
            Destroy(leftTank.GetComponent<Collider>());

            // Right tank
            GameObject rightTank = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            rightTank.transform.parent = jetpackModel.transform;
            rightTank.transform.localPosition = new Vector3(0.25f, 0, 0);
            rightTank.transform.localScale = new Vector3(0.18f, 0.45f, 0.18f);
            Destroy(rightTank.GetComponent<Collider>());

            Material mat = new Material(Shader.Find("Standard"));
            mat.color = new Color(0.9f, 0.2f, 0.1f);
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", new Color(0.6f, 0.1f, 0.05f));
            leftTank.GetComponent<Renderer>().material = mat;
            rightTank.GetComponent<Renderer>().material = mat;
        }
    }
}
