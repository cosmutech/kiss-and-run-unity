using System;
using System.Collections;
using UnityEngine;

namespace KissAndRun
{
    public class KissCinematicDirector : MonoBehaviour
    {
        public static KissCinematicDirector Instance { get; private set; }

        [Header("Cinematic Camera Settings")]
        [SerializeField] private Camera mainCamera;
        [SerializeField] private float slowMoScale = 0.2f;
        [SerializeField] private float cinematicDuration = 0.65f; // Real-time duration of the close-up
        [SerializeField] private float closeUpFov = 36f;
        [SerializeField] private Vector3 closeUpOffset = new Vector3(1.8f, 1.4f, 1.2f); // 3/4 side angle

        [Header("Cinematic Visual FX Prefabs")]
        [SerializeField] private GameObject lipstickDecalPrefab;
        [SerializeField] private GameObject heartExplosionPrefab;
        [SerializeField] private GameObject slapImpactStarsPrefab;
        [SerializeField] private GameObject comicTextCalloutPrefab;
        [SerializeField] private GameObject speedLinesOverlay;

        public bool IsInCinematic { get; private set; } = false;

        private Vector3 originalCamOffset;
        private float originalFov;
        private ThirdPersonCameraFollow cameraFollow;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            if (mainCamera == null) mainCamera = Camera.main;
            if (mainCamera != null)
            {
                originalFov = mainCamera.fieldOfView;
                cameraFollow = mainCamera.GetComponent<ThirdPersonCameraFollow>();
            }

            if (speedLinesOverlay) speedLinesOverlay.SetActive(false);
        }

        public void PlayKissCinematic(Transform playerTransform, Transform npcTransform, Action onKissImpact, Action onCinematicComplete, bool isSlap)
        {
            if (IsInCinematic) return;
            StartCoroutine(KissCinematicRoutine(playerTransform, npcTransform, onKissImpact, onCinematicComplete, isSlap));
        }

        private IEnumerator KissCinematicRoutine(
            Transform playerTransform,
            Transform npcTransform,
            Action onKissImpact,
            Action onCinematicComplete,
            bool isSlap)
        {
            IsInCinematic = true;

            // 1. Temporarily disable runner camera follow so we can control camera motion
            if (cameraFollow) cameraFollow.enabled = false;

            // 2. Slow down time (Matrix / Anime Bullet-Time feel!)
            Time.timeScale = slowMoScale;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;

            if (speedLinesOverlay) speedLinesOverlay.SetActive(true);

            // 3. Smoothly swoop camera to dramatic 3/4 close-up of faces
            Vector3 midpoint = (playerTransform.position + npcTransform.position) * 0.5f + Vector3.up * 1.5f;
            Vector3 targetCamPos = midpoint + playerTransform.right * closeUpOffset.x + Vector3.up * closeUpOffset.y - playerTransform.forward * closeUpOffset.z;

            float elapsed = 0f;
            float swoopDuration = 0.18f; // fast cinematic swoop
            Vector3 initialCamPos = mainCamera.transform.position;

            while (elapsed < swoopDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.SmoothStep(0, 1, elapsed / swoopDuration);

                mainCamera.transform.position = Vector3.Lerp(initialCamPos, targetCamPos, t);
                mainCamera.transform.LookAt(midpoint);
                mainCamera.fieldOfView = Mathf.Lerp(originalFov, closeUpFov, t);

                yield return null;
            }

            // 4. THE IMPACT MOMENT (Kiss or Slap lands!)
            onKissImpact?.Invoke();

            // Spawn Graphic Effects on Impact Point
            Vector3 contactPoint = midpoint + Vector3.up * 0.2f;

            if (isSlap)
            {
                // SLAP: Screen Shake, Comic Stars & POW Callout
                if (cameraFollow) cameraFollow.TriggerShake(0.85f);
                SpawnComicCallout("WHACK!! 💥", contactPoint + Vector3.up * 0.8f, Color.red);

                if (slapImpactStarsPrefab)
                {
                    Instantiate(slapImpactStarsPrefab, contactPoint, Quaternion.identity);
                }
                else
                {
                    CartoonVFXFactory.Instance?.SpawnSlapStars(contactPoint);
                }
            }
            else
            {
                // ROMANTIC KISS: Glowing Lipstick Stamp, Explosive Hearts & MWAH Callout
                SpawnLipstickStamp(npcTransform);
                SpawnComicCallout("MWAH!! 💋", contactPoint + Vector3.up * 0.8f, new Color(1f, 0.2f, 0.6f));

                if (heartExplosionPrefab)
                {
                    Instantiate(heartExplosionPrefab, contactPoint, Quaternion.identity);
                }
                else
                {
                    CartoonVFXFactory.Instance?.SpawnHeartBurst(contactPoint);
                }
            }

            // 5. Hold dramatic close-up for a fraction of a second in real-time
            yield return new WaitForSecondsRealtime(cinematicDuration);

            // 6. Restore Real-Time Game Speed & Transition back to runner camera
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;

            if (speedLinesOverlay) speedLinesOverlay.SetActive(false);

            // Smooth restore camera FOV
            elapsed = 0f;
            float restoreDuration = 0.22f;

            while (elapsed < restoreDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.SmoothStep(0, 1, elapsed / restoreDuration);
                mainCamera.fieldOfView = Mathf.Lerp(closeUpFov, originalFov, t);
                yield return null;
            }

            mainCamera.fieldOfView = originalFov;
            if (cameraFollow) cameraFollow.enabled = true;

            IsInCinematic = false;
            onCinematicComplete?.Invoke();
        }

        private void SpawnLipstickStamp(Transform targetTransform)
        {
            Vector3 cheekPos = targetTransform.position + Vector3.up * 1.55f + targetTransform.right * 0.25f + targetTransform.forward * 0.25f;

            if (lipstickDecalPrefab)
            {
                GameObject stamp = Instantiate(lipstickDecalPrefab, cheekPos, targetTransform.rotation, targetTransform);
                Destroy(stamp, 6f);
            }
            else
            {
                // Procedural lipstick kiss mark: glowing pink-red lipstick stamp on cheek!
                GameObject stamp = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                stamp.name = "LipstickKissMark";
                stamp.transform.parent = targetTransform;
                stamp.transform.position = cheekPos;
                stamp.transform.localScale = new Vector3(0.2f, 0.12f, 0.04f);
                stamp.transform.localRotation = Quaternion.Euler(15f, -20f, -10f);

                Renderer rend = stamp.GetComponent<Renderer>();
                Material lipMat = new Material(Shader.Find("Standard"));
                lipMat.color = new Color(1f, 0.08f, 0.42f);
                lipMat.EnableKeyword("_EMISSION");
                lipMat.SetColor("_EmissionColor", new Color(0.9f, 0.1f, 0.35f));
                rend.material = lipMat;

                Destroy(stamp.GetComponent<Collider>());
                Destroy(stamp, 7f);
            }
        }

        private void SpawnComicCallout(string text, Vector3 worldPosition, Color color)
        {
            if (comicTextCalloutPrefab)
            {
                GameObject callout = Instantiate(comicTextCalloutPrefab, worldPosition, Quaternion.identity);
                var calloutScript = callout.GetComponent<ComicCallout3D>();
                if (calloutScript != null)
                {
                    calloutScript.Initialize(text, color);
                }
            }
            else
            {
                // Procedural 3D comic text callout
                GameObject callout = new GameObject("ComicCallout");
                callout.transform.position = worldPosition;
                var tm = callout.AddComponent<TMPro.TextMeshPro>();
                tm.text = text;
                tm.color = color;
                tm.fontSize = 8;
                tm.alignment = TMPro.TextAlignmentOptions.Center;
                var calloutScript = callout.AddComponent<ComicCallout3D>();
                calloutScript.Initialize(text, color);
            }
        }
    }
}
