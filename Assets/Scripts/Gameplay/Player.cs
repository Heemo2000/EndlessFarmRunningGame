using UnityEngine;

namespace Game.Gameplay
{
    public class Player : MonoBehaviour
    {
        private GameInput gameInput;
        private Runner runner;

        private void Awake()
        {
            gameInput = GetComponent<GameInput>();
            runner = GetComponent<Runner>();
        }

        private void OnEnable()
        {
            if (gameInput != null)
            {
                gameInput.OnSwipeHorizontally.AddListener(runner.Swipe);
                gameInput.OnPressStarted.AddListener(runner.StartJumpBuildup);
                gameInput.OnPressEnded.AddListener(runner.LeaveJumpBuildup);
            }
        }

        private void OnDisable()
        {
            if (gameInput != null)
            {
                gameInput.OnSwipeHorizontally.RemoveListener(runner.Swipe);
                gameInput.OnPressStarted.RemoveListener(runner.StartJumpBuildup);
                gameInput.OnPressEnded.RemoveListener(runner.LeaveJumpBuildup);
            }
        }
    }
}
