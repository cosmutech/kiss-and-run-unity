using System;
using System.Collections.Generic;
using UnityEngine;

namespace KissAndRun
{
    [Serializable]
    public class WardrobeItem
    {
        public string itemId;
        public string itemName;
        public string itemCategory; // "Character", "Headgear", "Shoes", "TrailVFX", "Hoverboard"
        public int coinPrice;
        public Color themeColor;
        public GameObject itemMeshPrefab;
        public Material itemMaterial;
    }

    public class CharacterWardrobeCatalog : MonoBehaviour
    {
        public static CharacterWardrobeCatalog Instance { get; private set; }

        public List<WardrobeItem> characters = new List<WardrobeItem>();
        public List<WardrobeItem> headgear = new List<WardrobeItem>();
        public List<WardrobeItem> trailingFX = new List<WardrobeItem>();
        public List<WardrobeItem> hoverboards = new List<WardrobeItem>();

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            InitializeDefaultCatalog();
        }

        private void InitializeDefaultCatalog()
        {
            // 1. Playable 3D Characters
            characters.Add(new WardrobeItem { itemId = "romeo_default", itemName = "Romeo (Streetwear)", itemCategory = "Character", coinPrice = 0, themeColor = new Color(1f, 0.25f, 0.55f) });
            characters.Add(new WardrobeItem { itemId = "juliet_default", itemName = "Juliet (Speed Skater)", itemCategory = "Character", coinPrice = 0, themeColor = new Color(0.2f, 0.7f, 1f) });
            characters.Add(new WardrobeItem { itemId = "cyber_runner", itemName = "Cyber Neon", itemCategory = "Character", coinPrice = 250, themeColor = Color.cyan });
            characters.Add(new WardrobeItem { itemId = "golden_king", itemName = "Golden King", itemCategory = "Character", coinPrice = 500, themeColor = Color.yellow });
            characters.Add(new WardrobeItem { itemId = "disco_superstar", itemName = "Disco Star", itemCategory = "Character", coinPrice = 300, themeColor = Color.magenta });
            characters.Add(new WardrobeItem { itemId = "prankster_clown", itemName = "Prankster Clown", itemCategory = "Character", coinPrice = 400, themeColor = Color.green });

            // 2. Headgear & Accessories
            headgear.Add(new WardrobeItem { itemId = "thug_shades", itemName = "Thug Life Shades", itemCategory = "Headgear", coinPrice = 100, themeColor = Color.black });
            headgear.Add(new WardrobeItem { itemId = "dj_headphones", itemName = "DJ Kitty Headphones", itemCategory = "Headgear", coinPrice = 150, themeColor = Color.cyan });
            headgear.Add(new WardrobeItem { itemId = "golden_crown", itemName = "Royal Crown", itemCategory = "Headgear", coinPrice = 350, themeColor = Color.yellow });
            headgear.Add(new WardrobeItem { itemId = "neon_visor", itemName = "Neon Cyber Visor", itemCategory = "Headgear", coinPrice = 200, themeColor = Color.red });
            headgear.Add(new WardrobeItem { itemId = "bunny_ears", itemName = "Pink Bunny Ears", itemCategory = "Headgear", coinPrice = 120, themeColor = new Color(1f, 0.4f, 0.7f) });
            headgear.Add(new WardrobeItem { itemId = "hero_bandana", itemName = "Ninja Bandana", itemCategory = "Headgear", coinPrice = 80, themeColor = Color.red });

            // 3. Trailing Particle VFX
            trailingFX.Add(new WardrobeItem { itemId = "trail_hearts", itemName = "Fluttering Hearts", itemCategory = "TrailVFX", coinPrice = 150, themeColor = Color.magenta });
            trailingFX.Add(new WardrobeItem { itemId = "trail_flames", itemName = "Fire Sprint", itemCategory = "TrailVFX", coinPrice = 250, themeColor = new Color(1f, 0.4f, 0f) });
            trailingFX.Add(new WardrobeItem { itemId = "trail_cyber", itemName = "Cyber Matrix", itemCategory = "TrailVFX", coinPrice = 200, themeColor = Color.cyan });
            trailingFX.Add(new WardrobeItem { itemId = "trail_dollars", itemName = "Cash Storm", itemCategory = "TrailVFX", coinPrice = 400, themeColor = Color.green });

            // 4. Hoverboards
            hoverboards.Add(new WardrobeItem { itemId = "board_cyber", itemName = "Cyber Cruiser", itemCategory = "Hoverboard", coinPrice = 200, themeColor = Color.cyan });
            hoverboards.Add(new WardrobeItem { itemId = "board_flame", itemName = "Flame Rocket", itemCategory = "Hoverboard", coinPrice = 300, themeColor = new Color(1f, 0.3f, 0f) });
            hoverboards.Add(new WardrobeItem { itemId = "board_heart", itemName = "Heart Glider", itemCategory = "Hoverboard", coinPrice = 250, themeColor = Color.pink });
            hoverboards.Add(new WardrobeItem { itemId = "board_gold", itemName = "Midas Goldboard", itemCategory = "Hoverboard", coinPrice = 600, themeColor = Color.yellow });
        }

        public bool IsItemUnlocked(string itemId)
        {
            if (itemId == "romeo_default" || itemId == "juliet_default") return true;
            return PlayerPrefs.GetInt("Wardrobe_Unlocked_" + itemId, 0) == 1;
        }

        public bool PurchaseItem(string itemId, int price)
        {
            if (IsItemUnlocked(itemId)) return true;

            if (SaveManager.SpendCoins(price))
            {
                PlayerPrefs.SetInt("Wardrobe_Unlocked_" + itemId, 1);
                PlayerPrefs.Save();
                SoundManager.Instance?.PlaySound("coin_pickup");
                return true;
            }
            return false;
        }
    }
}
