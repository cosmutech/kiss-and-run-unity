using UnityEngine;

namespace KissAndRun
{
    public class CartoonVFXFactory : MonoBehaviour
    {
        public static CartoonVFXFactory Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public GameObject SpawnHeartBurst(Vector3 position)
        {
            GameObject vfxObj = new GameObject("VFX_HeartBurst");
            vfxObj.transform.position = position;

            ParticleSystem ps = vfxObj.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.startColor = new ParticleSystem.MinMaxGradient(new Color(1f, 0.2f, 0.6f), new Color(1f, 0.5f, 0.8f));
            main.startSize = new ParticleSystem.MinMaxCurve(0.35f, 0.75f);
            main.startLifetime = 1.2f;
            main.startSpeed = new ParticleSystem.MinMaxCurve(3f, 7f);
            main.duration = 0.5f;
            main.loop = false;

            var emission = ps.emission;
            emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 25) });

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = 0.4f;

            var colorOverLifetime = ps.colorOverLifetime;
            colorOverLifetime.enabled = true;
            Gradient grad = new Gradient();
            grad.SetKeys(
                new GradientColorKey[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(new Color(1f, 0.2f, 0.6f), 1f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) }
            );
            colorOverLifetime.color = grad;

            ps.Play();
            Destroy(vfxObj, 2f);
            return vfxObj;
        }

        public GameObject SpawnSlapStars(Vector3 position)
        {
            GameObject vfxObj = new GameObject("VFX_SlapStars");
            vfxObj.transform.position = position;

            ParticleSystem ps = vfxObj.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.startColor = new ParticleSystem.MinMaxGradient(Color.yellow, new Color(1f, 0.6f, 0.1f));
            main.startSize = new ParticleSystem.MinMaxCurve(0.4f, 0.8f);
            main.startLifetime = 0.9f;
            main.startSpeed = new ParticleSystem.MinMaxCurve(4f, 8f);
            main.duration = 0.3f;
            main.loop = false;

            var emission = ps.emission;
            emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 16) });

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Circle;
            shape.radius = 0.3f;

            ps.Play();
            Destroy(vfxObj, 1.5f);
            return vfxObj;
        }

        public void TriggerPoliceLights(Transform target)
        {
            GameObject sirenLight = new GameObject("SirenLight");
            sirenLight.transform.parent = target;
            sirenLight.transform.localPosition = new Vector3(0, 2.5f, 0);

            Light lightComp = sirenLight.AddComponent<Light>();
            lightComp.type = LightType.Point;
            lightComp.range = 14f;
            lightComp.intensity = 3.5f;

            var flasher = sirenLight.AddComponent<SirenFlasher>();
            flasher.targetLight = lightComp;
        }
    }

    public class SirenFlasher : MonoBehaviour
    {
        public Light targetLight;
        private float timer = 0f;
        private bool isRed = true;

        private void Update()
        {
            if (targetLight == null) return;

            timer += Time.deltaTime;
            if (timer >= 0.12f)
            {
                timer = 0f;
                isRed = !isRed;
                targetLight.color = isRed ? Color.red : Color.blue;
            }
        }
    }
}
