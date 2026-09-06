using UnityEngine;
using TMPro;

namespace KissAndRun
{
    public enum NPCType
    {
        Cheerleader,
        GymBro,
        GothGirl,
        Influencer,
        BusinessExec,
        Teacher,
        Granny,
        Skater,
        PoliceCadet,
        Sweetheart
    }

    public class NPCController : MonoBehaviour
    {
        [Header("NPC Profile")]
        public NPCType npcType = NPCType.Cheerleader;
        public string npcId = "npc_pedestrian";
        public string npcName = "Chloe (Cheerleader)";
        public bool isMale = false;

        [Header("Reaction Weights")]
        [Range(0f, 1f)] public float loveChance = 0.40f;
        [Range(0f, 1f)] public float kissBackChance = 0.30f;
        [Range(0f, 1f)] public float slapChance = 0.15f;
        [Range(0f, 1f)] public float chaseChance = 0.10f;
        [Range(0f, 1f)] public float policeChance = 0.05f;

        [Header("Visuals & UI")]
        [SerializeField] private GameObject kissHaloPrompt;
        [SerializeField] private GameObject speechBubbleObject;
        [SerializeField] private TextMeshPro speechText;
        [SerializeField] private ParticleSystem heartVFX;
        [SerializeField] private ParticleSystem angerVFX;

        public bool CanBeKissed { get; private set; } = true;
        public bool IsKissed { get; private set; } = false;

        private Animator animator;

        private void Awake()
        {
            animator = GetComponentInChildren<Animator>();
            if (kissHaloPrompt) kissHaloPrompt.SetActive(false);
            if (speechBubbleObject) speechBubbleObject.SetActive(false);
        }

        public void ConfigureArchetype(NPCType type)
        {
            npcType = type;

            switch (type)
            {
                case NPCType.Cheerleader:
                    npcId = "cheerleader";
                    npcName = "Chloe (Cheerleader)";
                    isMale = false;
                    loveChance = 0.50f; kissBackChance = 0.35f; slapChance = 0.08f; chaseChance = 0.05f; policeChance = 0.02f;
                    break;

                case NPCType.GymBro:
                    npcId = "gym_bro";
                    npcName = "Chad (Gym Bro)";
                    isMale = true;
                    loveChance = 0.10f; kissBackChance = 0.05f; slapChance = 0.45f; chaseChance = 0.35f; policeChance = 0.05f;
                    break;

                case NPCType.GothGirl:
                    npcId = "goth_girl";
                    npcName = "Raven (Goth Punk)";
                    isMale = false;
                    loveChance = 0.30f; kissBackChance = 0.40f; slapChance = 0.18f; chaseChance = 0.10f; policeChance = 0.02f;
                    break;

                case NPCType.Influencer:
                    npcId = "influencer";
                    npcName = "Bella (TikToker)";
                    isMale = false;
                    loveChance = 0.45f; kissBackChance = 0.30f; slapChance = 0.10f; chaseChance = 0.05f; policeChance = 0.10f;
                    break;

                case NPCType.BusinessExec:
                    npcId = "business_exec";
                    npcName = "Mr. Sterling (CEO)";
                    isMale = true;
                    loveChance = 0.08f; kissBackChance = 0.02f; slapChance = 0.35f; chaseChance = 0.25f; policeChance = 0.30f;
                    break;

                case NPCType.Teacher:
                    npcId = "strict_teacher";
                    npcName = "Mrs. Gable (Teacher)";
                    isMale = false;
                    loveChance = 0.12f; kissBackChance = 0.05f; slapChance = 0.40f; chaseChance = 0.35f; policeChance = 0.08f;
                    break;

                case NPCType.Granny:
                    npcId = "sweet_granny";
                    npcName = "Grandma Rose";
                    isMale = false;
                    loveChance = 0.40f; kissBackChance = 0.20f; slapChance = 0.30f; chaseChance = 0.08f; policeChance = 0.02f;
                    break;

                case NPCType.Skater:
                    npcId = "skater_dude";
                    npcName = "Axel (Skater Bro)";
                    isMale = true;
                    loveChance = 0.30f; kissBackChance = 0.30f; slapChance = 0.20f; chaseChance = 0.15f; policeChance = 0.05f;
                    break;

                case NPCType.PoliceCadet:
                    npcId = "police_cadet";
                    npcName = "Officer Jenny";
                    isMale = false;
                    loveChance = 0.20f; kissBackChance = 0.15f; slapChance = 0.25f; chaseChance = 0.20f; policeChance = 0.20f;
                    break;

                case NPCType.Sweetheart:
                    npcId = "sweetheart_juliet";
                    npcName = "Juliet (True Love)";
                    isMale = false;
                    loveChance = 0.70f; kissBackChance = 0.30f; slapChance = 0.0f; chaseChance = 0.0f; policeChance = 0.0f;
                    break;
            }
        }

        public void SetKissPromptActive(bool active)
        {
            if (!CanBeKissed) return;
            if (kissHaloPrompt) kissHaloPrompt.SetActive(active);
        }

        public void ExecuteKissReaction()
        {
            if (!CanBeKissed) return;

            CanBeKissed = false;
            IsKissed = true;
            if (kissHaloPrompt) kissHaloPrompt.SetActive(false);

            // Determine reaction outcome based on archetype weights
            float r = Random.value;
            if (r < loveChance)
            {
                TriggerLoveReaction();
            }
            else if (r < loveChance + kissBackChance)
            {
                TriggerKissBackReaction();
            }
            else if (r < loveChance + kissBackChance + slapChance)
            {
                TriggerSlapReaction();
            }
            else if (r < loveChance + kissBackChance + slapChance + chaseChance)
            {
                TriggerChaseReaction();
            }
            else
            {
                TriggerPoliceReaction();
            }
        }

        private void TriggerLoveReaction()
        {
            string speech = "OMG! You're so cute! 😍";
            int points = 100;
            int coins = 20;

            switch (npcType)
            {
                case NPCType.Cheerleader:
                    speech = "Take my number, Romeo! 💕✨";
                    points = 150; coins = 25;
                    break;
                case NPCType.Influencer:
                    speech = "VIRAL MOMENT! +500 Likes! 📸💖";
                    points = 250; coins = 50;
                    break;
                case NPCType.GothGirl:
                    speech = "Whatever... that was kinda nice 🖤";
                    points = 120; coins = 20;
                    break;
                case NPCType.Sweetheart:
                    speech = "I knew you'd find me, my Romeo! 💍❤️";
                    points = 500; coins = 100;
                    break;
                case NPCType.Granny:
                    speech = "What a handsome young prince! 👵🍪";
                    points = 100; coins = 30;
                    break;
            }

            ShowSpeech(speech);
            if (heartVFX) heartVFX.Play();
            CartoonVFXFactory.Instance?.SpawnHeartBurst(transform.position + Vector3.up * 1.5f);
            if (animator) animator.SetTrigger("Blush");
            SoundManager.Instance?.PlaySound("kiss");
            GameManager.Instance.OnKissSuccess(points, coins, isPositive: true);
        }

        private void TriggerKissBackReaction()
        {
            string speech = "Mwah! Take that back! 💋";
            int points = 180;
            int coins = 35;

            switch (npcType)
            {
                case NPCType.Cheerleader:
                    speech = "SMOOCH! Go team Romeo! 📣💋";
                    points = 220; coins = 40;
                    break;
                case NPCType.Influencer:
                    speech = "Double kiss! Trending #1! 📱💋";
                    points = 300; coins = 60;
                    break;
                case NPCType.GothGirl:
                    speech = "Bite back! Don't get used to it 💀💋";
                    points = 200; coins = 35;
                    break;
                case NPCType.Sweetheart:
                    speech = "Forever and always, Romeo! 💏✨";
                    points = 600; coins = 120;
                    break;
            }

            ShowSpeech(speech);
            if (heartVFX) heartVFX.Play();
            CartoonVFXFactory.Instance?.SpawnHeartBurst(transform.position + Vector3.up * 1.5f);
            if (animator) animator.SetTrigger("KissBack");
            SoundManager.Instance?.PlaySound("kiss");
            GameManager.Instance.OnKissSuccess(points, coins, isPositive: true);
        }

        private void TriggerSlapReaction()
        {
            string speech = "HOW DARE YOU!! 🤬💥";

            switch (npcType)
            {
                case NPCType.GymBro:
                    speech = "BRO! HANDS OFF THE DELTOIDS! 💢🥊";
                    break;
                case NPCType.Granny:
                    speech = "Take that with my handbag, rascal! 👵👜";
                    break;
                case NPCType.Teacher:
                    speech = "DETENTION! Zero hallway PDA! 📏😡";
                    break;
                case NPCType.BusinessExec:
                    speech = "You ruined my Armani suit! 💼💢";
                    break;
                case NPCType.PoliceCadet:
                    speech = "Resisting arrest?! WHACK! 🚨💥";
                    break;
            }

            ShowSpeech(speech);
            if (angerVFX) angerVFX.Play();
            CartoonVFXFactory.Instance?.SpawnSlapStars(transform.position + Vector3.up * 1.5f);
            if (animator) animator.SetTrigger("Slap");
            SoundManager.Instance?.PlaySound("slap");
            GameManager.Instance.OnKissSlapped(npcName);
        }

        private void TriggerChaseReaction()
        {
            string speech = "GET BACK HERE! 🏃💨";

            switch (npcType)
            {
                case NPCType.GymBro:
                    speech = "I SPRINT 5 MILES DAILY! YOU'RE DONE! 🏋️‍♂️🔥";
                    break;
                case NPCType.Teacher:
                    speech = "PRINCIPAL'S OFFICE! RIGHT NOW! 📏🏃";
                    break;
                case NPCType.Skater:
                    speech = "Nobody out-skates me, dude! 🛹💨";
                    break;
            }

            ShowSpeech(speech);
            if (angerVFX) angerVFX.Play();
            if (animator) animator.SetTrigger("Angry");
            SoundManager.Instance?.PlaySound("angry_scream");
            GameManager.Instance.OnKissTriggerChase(npcName, isPolice: false);
        }

        private void TriggerPoliceReaction()
        {
            string speech = "POLICE! HELP!! 🚨";

            if (npcType == NPCType.BusinessExec)
            {
                speech = "SECURITY! ARREST THIS SPRINTING MENACE! 🚨💼";
            }
            else if (npcType == NPCType.PoliceCadet)
            {
                speech = "CODE 10-4! ALL UNITS IN PURSUIT! 🚨🚔";
            }

            ShowSpeech(speech);
            if (angerVFX) angerVFX.Play();
            SoundManager.Instance?.PlaySound("police_whistle");
            GameManager.Instance.OnKissTriggerChase("Police Inspector", isPolice: true);
        }

        private void ShowSpeech(string text)
        {
            if (speechBubbleObject && speechText)
            {
                speechText.text = text;
                speechBubbleObject.SetActive(true);
            }
            else
            {
                // Procedural 3D floating comic speech bubble
                GameObject bubble = new GameObject("SpeechBubble");
                bubble.transform.position = transform.position + Vector3.up * 2.4f;
                var tm = bubble.AddComponent<TextMeshPro>();
                tm.text = text;
                tm.fontSize = 6.5f;
                tm.color = Color.white;
                tm.alignment = TextAlignmentOptions.Center;

                var callout = bubble.AddComponent<ComicCallout3D>();
                callout.Initialize(text, new Color(1f, 0.95f, 0.2f));
            }
        }
    }
}
