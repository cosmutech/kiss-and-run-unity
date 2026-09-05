using System;
using UnityEngine;

namespace KissAndRun
{
    public enum HoverboardType
    {
        CyberBoard,
        FlameGlider,
        HeartCruiser,
        GoldenVoyager
    }

    public class HoverboardSystem : MonoBehaviour
    {
        public static HoverboardSystem Instance { get; private set; }

        public static event Action<bool> OnHoverboardStateChanged;
        public static event Action<float> OnHoverboardTimerUpdate;

        [Header("Settings")]
        [SerializeField] private float boardDuration = 25f;
        [SerializeField] private float hoverHeightOffset = 0.35f;

        [Header("Visuals & Models")]
        [SerializeField] private GameObject boardModel;
        [SerializeField] private Light underglowLight;
        [SerializeField] private ParticleSystem trailVFX;

        public bool IsRiding { get; private set; } = false;
        public float RemainingTime { get; private set; } = 0f;

        private PlayerController player;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            player = GetComponentInParent<PlayerController>();
            if (boardModel) boardModel.SetActive(false);
            if (underglowLight) underglowLight.enabled = false;
        }

        private void Update()
        {
            if (!IsRiding) return;

            RemainingTime -= Time.deltaTime;
            OnHoverboardTimerUpdate?.Invoke(RemainingTime / boardDuration);

            if (RemainingTime <= 0f)
            {
                DeactivateBoard();
            }
        }

        public void ActivateBoard(HoverboardType boardType = HoverboardType.CyberBoard)
        {
            IsRiding = true;
            RemainingTime = boardDuration;

            if (boardModel == null)
            {
                CreateProceduralBoard(boardType);
            }

            if (boardModel) boardModel.SetActive(true);
            if (underglowLight)
            {
                underglowLight.enabled = true;
                underglowLight.color = GetBoardColor(boardType);
            }

            if (trailVFX)
            {
                trailVFX.Play();
            }

            SoundManager.Instance?.PlaySound("hoverboard_on");
            OnHoverboardStateChanged?.Invoke(true);
        }

        private void CreateProceduralBoard(HoverboardType boardType)
        {
            boardModel = GameObject.CreatePrimitive(PrimitiveType.Cube);
            boardModel.name = "HoverboardMesh";
            boardModel.transform.parent = transform;
            boardModel.transform.localPosition = new Vector3(0, -0.92f, 0);
            boardModel.transform.localScale = new Vector3(0.65f, 0.08f, 1.6f);

            Renderer rend = boardModel.GetComponent<Renderer>();
            Material mat = new Material(Shader.Find("Standard"));
            Color c = GetBoardColor(boardType);
            mat.color = c;
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", c * 0.8f);
            rend.material = mat;
            Destroy(boardModel.GetComponent<Collider>());

            if (underglowLight == null)
            {
                GameObject lightObj = new GameObject("BoardUnderglow");
                lightObj.transform.parent = boardModel.transform;
                lightObj.transform.localPosition = new Vector3(0, -0.2f, 0);
                underglowLight = lightObj.AddComponent<Light>();
                underglowLight.type = LightType.Point;
                underglowLight.range = 3.5f;
                underglowLight.intensity = 2.5f;
            }
        }

        public bool TryAbsorbCrash()
        {
            if (!IsRiding) return false;

            // Hoverboard absorbs the hit!
            DeactivateBoard();
            SoundManager.Instance?.PlaySound("board_break");

            // Grant brief invulnerability and trauma shake
            if (player)
            {
                player.InvincibleTimer = 1.5f;
            }

            KissCinematicDirector.Instance?.GetComponent<ThirdPersonCameraFollow>()?.TriggerShake(0.6f);
            return true;
        }

        public void DeactivateBoard()
        {
            IsRiding = false;
            RemainingTime = 0f;

            if (boardModel) boardModel.SetActive(false);
            if (underglowLight) underglowLight.enabled = false;
            if (trailVFX) trailVFX.Stop();

            OnHoverboardStateChanged?.Invoke(false);
        }

        private Color GetBoardColor(HoverboardType type)
        {
            switch (type)
            {
                case HoverboardType.CyberBoard: return Color.cyan;
                case HoverboardType.FlameGlider: return new Color(1f, 0.4f, 0f);
                case HoverboardType.HeartCruiser: return new Color(1f, 0.2f, 0.6f);
                case HoverboardType.GoldenVoyager: return Color.yellow;
                default: return Color.cyan;
            }
        }
    }
}
