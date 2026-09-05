using System.Collections;
using UnityEngine;
using TMPro;

namespace KissAndRun
{
    public class ComicCallout3D : MonoBehaviour
    {
        [SerializeField] private TextMeshPro textMesh;
        [SerializeField] private float floatSpeed = 2.5f;
        [SerializeField] private float lifetime = 1.2f;

        public void Initialize(string text, Color color)
        {
            if (textMesh == null) textMesh = GetComponentInChildren<TextMeshPro>();

            if (textMesh != null)
            {
                textMesh.text = text;
                textMesh.color = color;
            }

            StartCoroutine(AnimateRoutine());
        }

        private IEnumerator AnimateRoutine()
        {
            Camera cam = Camera.main;

            // Elastic punch scale
            Vector3 targetScale = transform.localScale;
            transform.localScale = Vector3.zero;

            float elapsed = 0f;
            float punchDuration = 0.25f;

            // Bounce in
            while (elapsed < punchDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / punchDuration;
                // Elastic overshoot curve
                float scaleT = Mathf.Sin(t * Mathf.PI * 0.75f) * 1.35f;
                transform.localScale = targetScale * scaleT;

                if (cam) transform.rotation = cam.transform.rotation;
                yield return null;
            }

            transform.localScale = targetScale;

            // Float up and fade out
            elapsed = 0f;
            Color startColor = textMesh ? textMesh.color : Color.white;

            while (elapsed < lifetime)
            {
                elapsed += Time.unscaledDeltaTime;
                transform.position += Vector3.up * (floatSpeed * Time.unscaledDeltaTime);

                if (cam) transform.rotation = cam.transform.rotation;

                if (textMesh)
                {
                    float alpha = Mathf.Clamp01(1f - (elapsed / lifetime));
                    textMesh.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
                }

                yield return null;
            }

            Destroy(gameObject);
        }
    }
}
