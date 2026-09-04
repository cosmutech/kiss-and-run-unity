using System;
using UnityEngine;

namespace KissAndRun
{
    public class SwipeDetector : MonoBehaviour
    {
        public static event Action OnSwipeLeft;
        public static event Action OnSwipeRight;
        public static event Action OnSwipeUp;
        public static event Action OnSwipeDown;
        public static event Action OnTap;

        [Header("Swipe Settings")]
        [SerializeField] private float minSwipeDistance = 40f;
        [SerializeField] private float maxSwipeTime = 0.8f;

        private Vector2 touchStartPos;
        private float touchStartTime;
        private bool isSwiping = false;

        private void Update()
        {
            HandleTouchInput();
            HandleKeyboardFallback();
        }

        private void HandleTouchInput()
        {
            if (Input.touchCount == 0) return;

            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touchStartPos = touch.position;
                    touchStartTime = Time.time;
                    isSwiping = true;
                    break;

                case TouchPhase.Ended:
                    if (!isSwiping) return;
                    isSwiping = false;

                    float duration = Time.time - touchStartTime;
                    Vector2 delta = touch.position - touchStartPos;

                    if (duration <= maxSwipeTime && delta.magnitude >= minSwipeDistance)
                    {
                        DetectSwipeDirection(delta);
                    }
                    else if (delta.magnitude < minSwipeDistance)
                    {
                        OnTap?.Invoke();
                    }
                    break;
            }
        }

        private void HandleKeyboardFallback()
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
                OnSwipeLeft?.Invoke();
            else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
                OnSwipeRight?.Invoke();
            else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space))
                OnSwipeUp?.Invoke();
            else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
                OnSwipeDown?.Invoke();
        }

        private void DetectSwipeDirection(Vector2 delta)
        {
            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            {
                // Horizontal Swipe (Lane Change)
                if (delta.x > 0)
                    OnSwipeRight?.Invoke();
                else
                    OnSwipeLeft?.Invoke();
            }
            else
            {
                // Vertical Swipe (Jump or Slide)
                if (delta.y > 0)
                    OnSwipeUp?.Invoke();
                else
                    OnSwipeDown?.Invoke();
            }
        }
    }
}
