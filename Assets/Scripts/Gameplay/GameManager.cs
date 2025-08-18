using UnityEngine;
using UnityEngine.Events;
using Game.Core;
using System.Collections;

namespace Game.Gameplay
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private Runner runner;
        [SerializeField] private FruitType levelType = FruitType.None;
        
        public UnityEvent OnBeforeGameStarts;
        public UnityEvent<int> OnStartingTimeDepleting;
        public UnityEvent OnGameStart;
        public UnityEvent OnGameOver;

        private Coroutine startGameCoroutine;

        public FruitType LevelType { get => levelType;}

        public void StartGame()
        {
            if (startGameCoroutine == null)
            {
                startGameCoroutine = StartCoroutine(StartGameSlowly());
            }
        }

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
        
        private void DisablePlayer()
        {
            runner.gameObject.SetActive(false);
        }

        private void TryToDoRespawn()
        {
            ServiceLocator.ForSceneOf(this).
               TryGetService<GameDataManager>(out GameDataManager gameDataManager);

            ServiceLocator.ForSceneOf(this).
               TryGetService<RespawnManager>(out RespawnManager respawnManager);

            if(gameDataManager == null)
            {
                Debug.Log("Game Data Manager is null");
            }

            if(respawnManager == null)
            {
                Debug.Log("Respawn Manager is null");
            }
            if (gameDataManager != null && respawnManager != null)
            {
                int coins = gameDataManager.GetCoinsCount();
                respawnManager.OnTryToDoRespawnBasedOnCoins?.Invoke(coins);
            }
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            OnGameOver.AddListener(DisablePlayer);
            OnGameOver.AddListener(TryToDoRespawn);
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
            runner.gameObject.SetActive(true);
            runner.ShouldMove = true;
        }

        private void DisablePlayerMovmement()
        {
            runner.gameObject.SetActive(true);
            runner.ShouldMove = false;
        }
    }
}
