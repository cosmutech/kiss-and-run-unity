using System;
using UnityEngine;

namespace KissAndRun
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public static event Action<int> OnScoreChanged;
        public static event Action<int> OnCoinsChanged;
        public static event Action<int> OnHealthChanged;
        public static event Action<int> OnComboChanged;
        public static event Action<string> OnBannerAlert;
        public static event Action OnGameOver;

        [Header("References")]
        [SerializeField] private PlayerController player;
        [SerializeField] private ChaserController chaser;

        [Header("Gameplay Stats")]
        [SerializeField] private int maxHealth = 3;

        public PlayerController Player => player;
        public ChaserController Chaser => chaser;

        public int Score { get; private set; } = 0;
        public int Coins { get; private set; } = 0;
        public int Health { get; private set; } = 3;
        public int Combo { get; private set; } = 1;
        public bool IsGameOver { get; private set; } = false;

        private float comboTimer = 0f;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            Health = maxHealth;
        }

        private void Update()
        {
            if (IsGameOver) return;

            // Passive distance score
            if (player != null)
            {
                Score += Mathf.RoundToInt(Time.deltaTime * 10f);
                OnScoreChanged?.Invoke(Score);
            }

            // Combo timer decay
            if (Combo > 1)
            {
                comboTimer -= Time.deltaTime;
                if (comboTimer <= 0f)
                {
                    Combo = 1;
                    OnComboChanged?.Invoke(Combo);
                }
            }
        }

        public void OnCoinCollected(int amount)
        {
            Coins += amount;
            Score += amount * 10 * Combo;
            OnCoinsChanged?.Invoke(Coins);
            OnScoreChanged?.Invoke(Score);
        }

        public void OnKissSuccess(int points, int coinReward, bool isPositive)
        {
            Score += points * Combo;
            Coins += coinReward;
            Combo++;
            comboTimer = 5f;

            OnScoreChanged?.Invoke(Score);
            OnCoinsChanged?.Invoke(Coins);
            OnComboChanged?.Invoke(Combo);

            OnBannerAlert?.Invoke("SMOOTH KISS! 💋 +" + (points * Combo));
        }

        public void OnKissSlapped(string npcName)
        {
            OnPlayerDamaged(1);
            OnBannerAlert?.Invoke("SLAPPED BY " + npcName.ToUpper() + "! 🤬");

            // Pursuer activates immediately
            if (chaser != null)
            {
                chaser.ActivateChaser(npcName, isPolice: false);
            }
        }

        public void OnKissTriggerChase(string chaserName, bool isPolice)
        {
            OnBannerAlert?.Invoke(isPolice ? "POLICE ON YOUR TAIL! 🚨" : "RUN! ANGRY MOB CHASING! 🏃💨");

            if (chaser != null)
            {
                chaser.ActivateChaser(chaserName, isPolice);
            }
        }

        public void OnPlayerDamaged(int amount)
        {
            if (IsGameOver) return;

            Health = Mathf.Max(0, Health - amount);
            OnHealthChanged?.Invoke(Health);

            if (Health <= 0)
            {
                TriggerGameOver();
            }
        }

        public void OnPlayerCaught()
        {
            if (IsGameOver) return;
            OnBannerAlert?.Invoke("YOU GOT CAUGHT! 😂");
            TriggerGameOver();
        }

        public void OnPursuerEscaped()
        {
            Score += 250;
            OnScoreChanged?.Invoke(Score);
            OnBannerAlert?.Invoke("PURSUER ESCAPED! +250 🏃💨");
            SoundManager.Instance?.PlaySound("cheer");
        }

        private void TriggerGameOver()
        {
            IsGameOver = true;
            OnGameOver?.Invoke();
            SoundManager.Instance?.PlaySound("game_over");

            // Save High Score and Coins
            SaveManager.SaveHighScore(Score);
            SaveManager.AddCoins(Coins);
        }

        public void RevivePlayer()
        {
            IsGameOver = false;
            Health = maxHealth;
            if (player) player.InvincibleTimer = 3f;
            if (chaser) chaser.DeactivateChaser();
            OnHealthChanged?.Invoke(Health);
        }
    }
}
