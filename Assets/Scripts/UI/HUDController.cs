using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace KissAndRun
{
    public class HUDController : MonoBehaviour
    {
        [Header("Top Stats")]
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI coinsText;
        [SerializeField] private TextMeshProUGUI comboText;
        [SerializeField] private Image[] heartIcons;

        [Header("Chaser Proximity")]
        [SerializeField] private GameObject chaserPanel;
        [SerializeField] private Slider chaserDistanceSlider;
        [SerializeField] private TextMeshProUGUI chaserLabelText;

        [Header("Hoverboard & Jetpack")]
        [SerializeField] private Button hoverboardButton;
        [SerializeField] private GameObject hoverboardActiveBar;
        [SerializeField] private Slider hoverboardDurationSlider;
        [SerializeField] private GameObject jetpackActiveBar;
        [SerializeField] private Slider jetpackProgressSlider;

        [Header("💋 Kiss Button")]
        [SerializeField] private Button kissButton;
        [SerializeField] private GameObject kissPulsingGlow;

        [Header("Comic Event Banner")]
        [SerializeField] private GameObject bannerObject;
        [SerializeField] private TextMeshProUGUI bannerText;

        private float bannerTimer = 0f;

        private void OnEnable()
        {
            GameManager.OnScoreChanged += UpdateScore;
            GameManager.OnCoinsChanged += UpdateCoins;
            GameManager.OnHealthChanged += UpdateHealth;
            GameManager.OnComboChanged += UpdateCombo;
            GameManager.OnBannerAlert += ShowBanner;
            KissManager.OnKissTargetAvailable += SetKissButtonActive;

            HoverboardSystem.OnHoverboardStateChanged += HandleHoverboardState;
            HoverboardSystem.OnHoverboardTimerUpdate += UpdateHoverboardTimer;
            JetpackSystem.OnJetpackStateChanged += HandleJetpackState;
            JetpackSystem.OnJetpackProgressUpdate += UpdateJetpackProgress;

            if (kissButton) kissButton.onClick.AddListener(OnKissButtonClicked);
            if (hoverboardButton) hoverboardButton.onClick.AddListener(OnHoverboardButtonClicked);
        }

        private void OnDisable()
        {
            GameManager.OnScoreChanged -= UpdateScore;
            GameManager.OnCoinsChanged -= UpdateCoins;
            GameManager.OnHealthChanged -= UpdateHealth;
            GameManager.OnComboChanged -= UpdateCombo;
            GameManager.OnBannerAlert -= ShowBanner;
            KissManager.OnKissTargetAvailable -= SetKissButtonActive;

            HoverboardSystem.OnHoverboardStateChanged -= HandleHoverboardState;
            HoverboardSystem.OnHoverboardTimerUpdate -= UpdateHoverboardTimer;
            JetpackSystem.OnJetpackStateChanged -= HandleJetpackState;
            JetpackSystem.OnJetpackProgressUpdate -= UpdateJetpackProgress;

            if (kissButton) kissButton.onClick.RemoveListener(OnKissButtonClicked);
            if (hoverboardButton) hoverboardButton.onClick.RemoveListener(OnHoverboardButtonClicked);
        }

        private void Start()
        {
            SetKissButtonActive(false);
            if (bannerObject) bannerObject.SetActive(false);
            if (chaserPanel) chaserPanel.SetActive(false);
            if (hoverboardActiveBar) hoverboardActiveBar.SetActive(false);
            if (jetpackActiveBar) jetpackActiveBar.SetActive(false);
        }

        private void Update()
        {
            // Banner timer
            if (bannerTimer > 0)
            {
                bannerTimer -= Time.deltaTime;
                if (bannerTimer <= 0 && bannerObject) bannerObject.SetActive(false);
            }

            // Update Chaser Tracker
            ChaserController chaser = GameManager.Instance?.Chaser;
            if (chaser != null && chaser.IsActive)
            {
                if (chaserPanel && !chaserPanel.activeSelf) chaserPanel.SetActive(true);
                if (chaserDistanceSlider) chaserDistanceSlider.value = Mathf.Clamp01(chaser.CurrentDistance / 25f);
                if (chaserLabelText) chaserLabelText.text = "PURSUER: " + chaser.ChaserType.ToUpper();
            }
            else
            {
                if (chaserPanel && chaserPanel.activeSelf) chaserPanel.SetActive(false);
            }
        }

        private void UpdateScore(int score)
        {
            if (scoreText) scoreText.text = score.ToString("N0");
        }

        private void UpdateCoins(int coins)
        {
            if (coinsText) coinsText.text = coins.ToString();
        }

        private void UpdateHealth(int health)
        {
            if (heartIcons == null) return;
            for (int i = 0; i < heartIcons.Length; i++)
            {
                heartIcons[i].color = i < health ? Color.white : new Color(0.2f, 0.2f, 0.2f, 0.5f);
            }
        }

        private void UpdateCombo(int combo)
        {
            if (comboText)
            {
                comboText.gameObject.SetActive(combo > 1);
                comboText.text = "🔥 COMBO x" + combo;
            }
        }

        private void ShowBanner(string text)
        {
            if (bannerObject && bannerText)
            {
                bannerText.text = text;
                bannerObject.SetActive(true);
                bannerTimer = 2.5f;
            }
        }

        private void HandleHoverboardState(bool isRiding)
        {
            if (hoverboardActiveBar) hoverboardActiveBar.SetActive(isRiding);
        }

        private void UpdateHoverboardTimer(float progress)
        {
            if (hoverboardDurationSlider) hoverboardDurationSlider.value = progress;
        }

        private void HandleJetpackState(bool isFlying)
        {
            if (jetpackActiveBar) jetpackActiveBar.SetActive(isFlying);
        }

        private void UpdateJetpackProgress(float progress)
        {
            if (jetpackProgressSlider) jetpackProgressSlider.value = progress;
        }

        public void SetKissButtonActive(bool active)
        {
            if (kissButton) kissButton.interactable = active;
            if (kissPulsingGlow) kissPulsingGlow.SetActive(active);
        }

        private void OnKissButtonClicked()
        {
            KissManager.Instance?.TryPerformKiss();
        }

        private void OnHoverboardButtonClicked()
        {
            GameManager.Instance?.Player?.SummonHoverboard();
        }
    }
}
