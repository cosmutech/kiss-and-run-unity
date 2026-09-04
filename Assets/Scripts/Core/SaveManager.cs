using UnityEngine;

namespace KissAndRun
{
    public static class SaveManager
    {
        private const string KEY_HIGH_SCORE = "High_Score";
        private const string KEY_COINS = "Total_Coins";
        private const string KEY_SOUND_ENABLED = "Sound_Enabled";

        public static int GetHighScore() => PlayerPrefs.GetInt(KEY_HIGH_SCORE, 0);

        public static void SaveHighScore(int score)
        {
            if (score > GetHighScore())
            {
                PlayerPrefs.SetInt(KEY_HIGH_SCORE, score);
                PlayerPrefs.Save();
            }
        }

        public static int GetTotalCoins() => PlayerPrefs.GetInt(KEY_COINS, 0);

        public static void AddCoins(int amount)
        {
            int total = GetTotalCoins() + amount;
            PlayerPrefs.SetInt(KEY_COINS, total);
            PlayerPrefs.Save();
        }

        public static bool SpendCoins(int amount)
        {
            int current = GetTotalCoins();
            if (current >= amount)
            {
                PlayerPrefs.SetInt(KEY_COINS, current - amount);
                PlayerPrefs.Save();
                return true;
            }
            return false;
        }

        public static bool IsSoundEnabled() => PlayerPrefs.GetInt(KEY_SOUND_ENABLED, 1) == 1;

        public static void SetSoundEnabled(bool enabled)
        {
            PlayerPrefs.SetInt(KEY_SOUND_ENABLED, enabled ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
}
