using UnityEngine;
using UnityEngine.Events;
using Game.Core;
using System.Collections;

namespace Game.Gameplay
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private Runner runner;
        
        public UnityEvent OnBeforeGameStarts;
        public UnityEvent<int> OnStartingTimeDepleting;
        public UnityEvent OnGameStart;
        public UnityEvent OnGameOver;

        private Coroutine startGameCoroutine;
        private IEnumerator StartGameSlowly()
        {
            int secondsLeft = 3;
            OnBeforeGameStarts?.Invoke();
            yield return new WaitForSeconds(1.0f);
            OnStartingTimeDepleting?.Invoke(secondsLeft);
            while (secondsLeft > 0)
            {
                yield return new WaitForSeconds(1.0f);
                secondsLeft--;
                OnStartingTimeDepleting?.Invoke(secondsLeft);
            }
            OnGameStart?.Invoke();
            startGameCoroutine = null;
        }

        private void StartGame() 
        {
            
            if (startGameCoroutine == null) 
            {
                startGameCoroutine = StartCoroutine(StartGameSlowly());
            }
        }

        private void DisablePlayer()
        {
            runner.gameObject.SetActive(false);
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            OnGameOver.AddListener(DisablePlayer);
            OnBeforeGameStarts.AddListener(DisablePlayerMovmement);
            OnGameStart.AddListener(EnablePlayerMovement);
            ServiceLocator.ForSceneOf(this).Register<GameManager>(this);
            StartGame();
        }

        private void OnDestroy()
        {
            OnGameOver.RemoveAllListeners();
            OnBeforeGameStarts.RemoveAllListeners();
            OnStartingTimeDepleting.RemoveAllListeners();
            OnGameStart.RemoveAllListeners();
        }

        private void EnablePlayerMovement()
        {
            runner.ShouldMove = true;
        }

        private void DisablePlayerMovmement()
        {
            runner.ShouldMove = false;
        }
    }
}
