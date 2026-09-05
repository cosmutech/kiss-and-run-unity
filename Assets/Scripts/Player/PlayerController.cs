using System.Collections;
using UnityEngine;

namespace KissAndRun
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement & 3-Lane Grid")]
        [SerializeField] private float forwardSpeed = 12.5f;
        [SerializeField] private float laneDistance = 2.5f; // Distance between lanes
        [SerializeField] private float laneChangeSpeed = 16f;
        [SerializeField] private float laneBankAngle = 14f; // Tilt when banking

        [Header("Jump & Slide")]
        [SerializeField] private float jumpForce = 9.5f;
        [SerializeField] private float gravity = 24f;
        [SerializeField] private float slideDuration = 0.7f;

        [Header("Subsystems")]
        [SerializeField] private HoverboardSystem hoverboard;
        [SerializeField] private JetpackSystem jetpack;
        [SerializeField] private StuntTrickSystem stuntSystem;

        private CharacterController controller;
        private Animator animator;

        // Lane tracking: -1 (Left), 0 (Center), 1 (Right)
        private int currentLane = 0;
        private float verticalVelocity = 0f;
        private bool isSliding = false;
        private float originalHeight;
        private Vector3 originalCenter;

        // Double-tap timer for hoverboard
        private float lastTapTime = 0f;
        private const float doubleTapThreshold = 0.35f;

        // Buff Timers
        public bool HasShield { get; set; }
        public float MagnetTimer { get; set; }
        public float SpeedBoostTimer { get; set; }
        public float SlowMoTimer { get; set; }
        public float InvincibleTimer { get; set; }

        public bool IsInvincible => InvincibleTimer > 0;
        public bool HasMagnet => MagnetTimer > 0;
        public bool HasSpeedBoost => SpeedBoostTimer > 0;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            animator = GetComponentInChildren<Animator>();
            if (hoverboard == null) hoverboard = GetComponentInChildren<HoverboardSystem>();
            if (jetpack == null) jetpack = GetComponentInChildren<JetpackSystem>();
            if (stuntSystem == null) stuntSystem = GetComponentInChildren<StuntTrickSystem>();

            originalHeight = controller.height;
            originalCenter = controller.center;
        }

        private void OnEnable()
        {
            SwipeDetector.OnSwipeLeft += MoveLeft;
            SwipeDetector.OnSwipeRight += MoveRight;
            SwipeDetector.OnSwipeUp += Jump;
            SwipeDetector.OnSwipeDown += Slide;
            SwipeDetector.OnTap += HandleTap;
        }

        private void OnDisable()
        {
            SwipeDetector.OnSwipeLeft -= MoveLeft;
            SwipeDetector.OnSwipeRight -= MoveRight;
            SwipeDetector.OnSwipeUp -= Jump;
            SwipeDetector.OnSwipeDown -= Slide;
            SwipeDetector.OnTap -= HandleTap;
        }

        private void Update()
        {
            UpdateBuffs();
            HandleMovement();
        }

        private void UpdateBuffs()
        {
            if (InvincibleTimer > 0) InvincibleTimer -= Time.deltaTime;
            if (MagnetTimer > 0) MagnetTimer -= Time.deltaTime;
            if (SpeedBoostTimer > 0) SpeedBoostTimer -= Time.deltaTime;
            if (SlowMoTimer > 0) SlowMoTimer -= Time.deltaTime;
        }

        private void HandleMovement()
        {
            // If flying with Jetpack, vertical position is handled by JetpackSystem
            if (jetpack != null && jetpack.IsFlying) return;

            // 1. Calculate target lane position
            float targetX = currentLane * laneDistance;
            float newX = Mathf.Lerp(transform.position.x, targetX, Time.deltaTime * laneChangeSpeed);

            // 2. Dynamic tilt roll into turns (Subway Surfers dynamic banking!)
            float deltaX = targetX - transform.position.x;
            float targetTilt = -deltaX * laneBankAngle;
            transform.rotation = Quaternion.Euler(0, 0, targetTilt);

            // 3. Gravity and Jump
            if (controller.isGrounded)
            {
                if (verticalVelocity < 0) verticalVelocity = -1f;
            }
            else
            {
                verticalVelocity -= gravity * Time.deltaTime;
            }

            // 4. Forward speed
            float speed = forwardSpeed;
            if (HasSpeedBoost) speed *= 1.45f;
            if (hoverboard != null && hoverboard.IsRiding) speed *= 1.15f;
            if (SlowMoTimer > 0) speed *= 0.8f;

            Vector3 moveDirection = new Vector3(newX - transform.position.x, verticalVelocity * Time.deltaTime, speed * Time.deltaTime);
            controller.Move(moveDirection);
        }

        public void MoveLeft()
        {
            if (currentLane > -1)
            {
                currentLane--;
                PlaySound("whoosh");
            }
        }

        public void MoveRight()
        {
            if (currentLane < 1)
            {
                currentLane++;
                PlaySound("whoosh");
            }
        }

        public void Jump()
        {
            if (controller.isGrounded && !isSliding)
            {
                verticalVelocity = jumpForce;
                if (animator) animator.SetTrigger("Jump");
                PlaySound("jump");

                // Chance to trigger stunt trick in air!
                if (stuntSystem != null && Random.value < 0.45f)
                {
                    stuntSystem.TryPerformJumpStunt();
                }
            }
        }

        public void Slide()
        {
            if (!controller.isGrounded)
            {
                // Fast dive down from jump (Subway Surfers feel!)
                verticalVelocity = -jumpForce * 1.6f;
            }
            if (!isSliding)
            {
                StartCoroutine(SlideRoutine());
            }
        }

        private void HandleTap()
        {
            float timeSinceLastTap = Time.time - lastTapTime;
            if (timeSinceLastTap <= doubleTapThreshold)
            {
                // DOUBLE-TAP: Summon Hoverboard! (Subway Surfers signature move!)
                SummonHoverboard();
            }
            lastTapTime = Time.time;
        }

        public void SummonHoverboard()
        {
            if (hoverboard != null && !hoverboard.IsRiding)
            {
                hoverboard.ActivateBoard();
            }
        }

        private IEnumerator SlideRoutine()
        {
            isSliding = true;
            if (animator) animator.SetBool("IsSliding", true);
            PlaySound("slide");

            // Shrink collider so player slides under clearance signs
            controller.height = originalHeight * 0.45f;
            controller.center = new Vector3(originalCenter.x, originalCenter.y * 0.45f, originalCenter.z);

            yield return new WaitForSeconds(slideDuration);

            controller.height = originalHeight;
            controller.center = originalCenter;
            isSliding = false;
            if (animator) animator.SetBool("IsSliding", false);
        }

        public void TakeDamage(int amount = 1)
        {
            if (IsInvincible) return;

            // 1. Hoverboard Crash Protection (Saves the player's life!)
            if (hoverboard != null && hoverboard.IsRiding)
            {
                hoverboard.TryAbsorbCrash();
                return;
            }

            // 2. Heart Shield Protection
            if (HasShield)
            {
                HasShield = false;
                InvincibleTimer = 1.5f;
                PlaySound("shield_break");
                return;
            }

            // 3. Stumble damage
            InvincibleTimer = 1.8f;
            if (animator) animator.SetTrigger("Stumble");
            PlaySound("slap");

            GameManager.Instance.OnPlayerDamaged(amount);
        }

        private void PlaySound(string soundName)
        {
            SoundManager.Instance?.PlaySound(soundName);
        }
    }
}
