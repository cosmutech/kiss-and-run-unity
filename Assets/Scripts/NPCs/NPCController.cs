using UnityEngine;
using TMPro;

namespace KissAndRun
{
    public class NPCController : MonoBehaviour
    {
        [Header("NPC Data")]
        public string npcId = "npc_pedestrian";
        public string npcName = "Happy Girl";
        public bool isMale = false;

        [Header("Reaction Weights")]
        [Range(0f, 1f)] public float loveChance = 0.35f;
        [Range(0f, 1f)] public float kissBackChance = 0.25f;
        [Range(0f, 1f)] public float slapChance = 0.20f;
        [Range(0f, 1f)] public float chaseChance = 0.15f;
        [Range(0f, 1f)] public float policeChance = 0.05f;

        [Header("UI & Visuals")]
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

            // Determine reaction outcome
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
            ShowSpeech("OMG! You're so cute! 😍");
            if (heartVFX) heartVFX.Play();
            if (animator) animator.SetTrigger("Blush");
            SoundManager.Instance?.PlaySound("kiss");
            GameManager.Instance.OnKissSuccess(points: 80, coins: 15, isPositive: true);
        }

        private void TriggerKissBackReaction()
        {
            ShowSpeech("Mwah! Take that! 💋");
            if (heartVFX) heartVFX.Play();
            if (animator) animator.SetTrigger("KissBack");
            SoundManager.Instance?.PlaySound("kiss");
            GameManager.Instance.OnKissSuccess(points: 120, coins: 25, isPositive: true);
        }

        private void TriggerSlapReaction()
        {
            ShowSpeech("HOW DARE YOU!! 🤬");
            if (angerVFX) angerVFX.Play();
            if (animator) animator.SetTrigger("Slap");
            SoundManager.Instance?.PlaySound("slap");
            GameManager.Instance.OnKissSlapped(npcName);
        }

        private void TriggerChaseReaction()
        {
            ShowSpeech("GET BACK HERE! 🏃💨");
            if (angerVFX) angerVFX.Play();
            if (animator) animator.SetTrigger("Angry");
            SoundManager.Instance?.PlaySound("angry_scream");
            GameManager.Instance.OnKissTriggerChase(npcName, isPolice: false);
        }

        private void TriggerPoliceReaction()
        {
            ShowSpeech("POLICE! HELP!! 🚨");
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
        }
    }
}
