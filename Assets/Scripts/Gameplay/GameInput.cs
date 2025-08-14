using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Game.Input;
using System;

namespace Game.Gameplay
{
    public class GameInput : MonoBehaviour
    {
        [SerializeField] private Vector2 swipeDistance = new Vector2(10, 10);
        private GameControls controls;
        private Vector2 startingPosition = Vector2.zero;
        private Vector2 endPosition = Vector2.zero;
        private Vector2 direction = Vector2.zero;
        private bool isPressed = false;
        private bool isSwiping = false;

        public UnityEvent<int> OnSwipeHorizontally;
        public UnityEvent<int> OnSwipeVertically;
        public UnityEvent OnPressStarted;
        public UnityEvent OnPressEnded;


        //private void OnSwipeStarted(InputAction.CallbackContext context)
        //{
        //    startingPosition = context.ReadValue<Vector2>();
        //}
        private void OnSwipePerformed(InputAction.CallbackContext context)
        {
            if(!isPressed || isSwiping)
            {
                return;
            }
            endPosition = context.ReadValue<Vector2>();
            direction = endPosition - startingPosition;

            float distanceX = Mathf.Abs(direction.x);
            float distanceY = Mathf.Abs(direction.y);

            if (distanceX >= swipeDistance.x)
            {
                OnSwipeHorizontally?.Invoke((int)Mathf.Sign(direction.x));
                isSwiping = true;
            }

            if (distanceY >= swipeDistance.y)
            {
                OnSwipeVertically?.Invoke((int)Mathf.Sign(direction.y));
                isSwiping = true;
            }
        }

        private void OnPressStart(InputAction.CallbackContext context)
        {
            isPressed = true;
            OnPressStarted?.Invoke();
            startingPosition = controls.Player.Swipe.ReadValue<Vector2>();
        }

        private void OnPressEnd(InputAction.CallbackContext context)
        {
            isPressed = false;
            isSwiping = false;
            OnPressEnded?.Invoke();
        }

        
        private void Awake()
        {
            controls = new GameControls();
            controls.Enable();
            controls.Player.Press.started += OnPressStart;
            controls.Player.Press.canceled += OnPressEnd;
            //controls.Player.Swipe.started += OnSwipeStarted;
            controls.Player.Swipe.performed += OnSwipePerformed;
        }

        

        private void OnDestroy()
        {
            controls.Disable();
            controls.Player.Press.started -= OnPressStart;
            controls.Player.Press.canceled -= OnPressEnd;
            //controls.Player.Swipe.started -= OnSwipeStarted;
            controls.Player.Swipe.performed -= OnSwipePerformed;
        }
    }
}
