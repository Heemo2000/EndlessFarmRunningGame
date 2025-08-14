using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Game.Input;

namespace Game.Gameplay
{
    public class GameInput : MonoBehaviour
    {
        [SerializeField] private Vector2 swipeDistance = new Vector2(10, 10);
        private GameControls controls;
        private Vector2 startingPosition = Vector2.zero;
        private Vector2 endPosition = Vector2.zero;
        private Vector2 direction = Vector2.zero;

        public UnityEvent<int> OnSwipeHorizontally;
        public UnityEvent<int> OnSwipeVertically;
        private void Awake()
        {
            controls = new GameControls();
            controls.Player.Swipe.started += OnSwipeStarted;
            controls.Player.Swipe.canceled += OnSwipeEnded;
        }
        private void OnSwipeStarted(InputAction.CallbackContext context)
        {
            startingPosition = context.ReadValue<Vector2>();
        }
        private void OnSwipeEnded(InputAction.CallbackContext context)
        {
            endPosition = context.ReadValue<Vector2>();
            direction = endPosition - startingPosition;

            float distanceX = Mathf.Abs(direction.x);
            float distanceY = Mathf.Abs(direction.y);

            if(distanceX >= swipeDistance.x)
            {
                OnSwipeHorizontally?.Invoke((int)Mathf.Sign(direction.x));
            }

            if(distanceY >= swipeDistance.y)
            {
                OnSwipeVertically?.Invoke((int) Mathf.Sign(direction.y));
            }
        }
    }
}
