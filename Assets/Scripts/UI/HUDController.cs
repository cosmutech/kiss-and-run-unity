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

            if (kissButton)
            {
                kissButton.onClick.AddListener(OnKissButtonClicked);
            }
        }

        private void OnDisable()
        {
            GameManager.OnScoreChanged -= UpdateScore;
            GameManager.OnCoinsChanged -= UpdateCoins;
            GameManager.OnHealthChanged -= UpdateHealth;
            GameManager.OnComboChanged -= UpdateCombo;
            GameManager.OnBannerAlert -= ShowBanner;
            KissManager.OnKissTargetAvailable -= SetKissButtonActive;

            if (kissButton)
            {
                kissButton.onClick.RemoveListener(OnKissButtonClicked);
            }
        }

        private void Start()
        {
            SetKissButtonActive(false);
            if (bannerObject) bannerObject.SetActive(false);
            if (chaserPanel) chaserPanel.SetActive(false);
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

        public void SetKissButtonActive(bool active)
        {
            if (kissButton) kissButton.interactable = active;
            if (kissPulsingGlow) kissPulsingGlow.SetActive(active);
        }

        private void OnKissButtonClicked()
        {
            KissManager.Instance?.TryPerformKiss();
        }
    }
}
