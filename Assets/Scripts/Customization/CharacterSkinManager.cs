using System;
using System.Collections.Generic;
using UnityEngine;

namespace KissAndRun
{
    [Serializable]
    public class CharacterSkinData
    {
        public string skinId;
        public string skinName;
        public int unlockPriceCoins;
        public GameObject characterMeshPrefab;
        public Material skinMaterial;
        public ParticleSystem auraVFX;
    }

    public class CharacterSkinManager : MonoBehaviour
    {
        public static CharacterSkinManager Instance { get; private set; }

        [SerializeField] private List<CharacterSkinData> availableSkins = new List<CharacterSkinData>();
        [SerializeField] private Transform characterSpawnPoint;

        public CharacterSkinData CurrentSkin { get; private set; }

        private GameObject currentMeshInstance;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            string equippedId = PlayerPrefs.GetString("Equipped_Skin", "romeo_default");
            EquipSkin(equippedId);
        }

        public void EquipSkin(string skinId)
        {
            CharacterSkinData target = availableSkins.Find(s => s.skinId == skinId);
            if (target == null && availableSkins.Count > 0)
            {
                target = availableSkins[0];
            }

            if (target != null)
            {
                CurrentSkin = target;
                PlayerPrefs.SetString("Equipped_Skin", target.skinId);
                PlayerPrefs.Save();

                if (currentMeshInstance != null)
                {
                    Destroy(currentMeshInstance);
                }

                if (target.characterMeshPrefab != null && characterSpawnPoint != null)
                {
                    currentMeshInstance = Instantiate(target.characterMeshPrefab, characterSpawnPoint);
                }
            }
        }

        public bool UnlockSkin(string skinId)
        {
            CharacterSkinData target = availableSkins.Find(s => s.skinId == skinId);
            if (target == null) return false;

            if (SaveManager.SpendCoins(target.unlockPriceCoins))
            {
                PlayerPrefs.SetInt("Skin_Unlocked_" + skinId, 1);
                PlayerPrefs.Save();
                return true;
            }
            return false;
        }

        public bool IsSkinUnlocked(string skinId)
        {
            if (skinId == "romeo_default" || skinId == "juliet_default") return true;
            return PlayerPrefs.GetInt("Skin_Unlocked_" + skinId, 0) == 1;
        }
    }
}
