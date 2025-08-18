using Game.Core;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Gameplay
{
    public class RespawnManager : MonoBehaviour
    {
        [SerializeField] private Runner runner;
        [Min(0.01f)]
        [SerializeField] private float checkRadius = 2.0f;
        [SerializeField] private float aheadDistance = 0.2f;
        [SerializeField] private LayerMask obstacleMask;

        public UnityEvent<int> OnTryToDoRespawnBasedOnCoins;
        public UnityEvent<int> OnRespawnCostCalculated; // For UI feedback
        public UnityEvent OnRespawnSucceeded;
        public UnityEvent OnRespawnFailed;

        
        // Call this on crash:
        public void AttemptRespawn()
        {
             if(ServiceLocator.ForSceneOf(this).TryGetService<GameManager>(out GameManager gameManager)
                &&
                ServiceLocator.ForSceneOf(this).TryGetService<GameDataManager>(out GameDataManager gameDataManager))
            {
                int totalFruits = gameDataManager.GetFruitCount(gameManager.LevelType);

                int respawnCost = CalculateRespawnCost(totalFruits);
                OnRespawnCostCalculated?.Invoke(respawnCost);

                if (totalFruits < 100)
                {
                    // Free respawn
                    PerformRespawn();
                }
                else
                {
                    // Try to pay cash or trigger ad flow
                    if (TrySpendCoins(respawnCost) || ShowAdForRespawn())
                    {
                        PerformRespawn();
                    }
                    else
                    {
                        // Not enough coins and/or ad declined
                        OnRespawnFailed?.Invoke();
                    }
                }
            }
            
        }

        public int CalculateRespawnCost(int fruitCount)
        {
            if (fruitCount < 100)
                return 0;

            int bracketStart = 100;
            int respawnCost = 10;

            while (fruitCount >= bracketStart * 2)
            {
                bracketStart *= 2;
                respawnCost += 10;
            }
            return respawnCost;
        }

        private void SendPlayerAtAppropriatePosition()
        {
            float distanceZ = 0.0f;
            Vector3 checkPosition = runner.transform.position;

            while(Physics.CheckSphere(checkPosition, checkRadius, obstacleMask.value))
            {
                Debug.Log("Increment spawn position");
                distanceZ += aheadDistance;
                checkPosition.z += aheadDistance;
            }

            runner.gameObject.SetActive(true);
            runner.MoveForward(distanceZ);
            runner.MoveForward(distanceZ);

            if (ServiceLocator.ForSceneOf(this).TryGetService<GameManager>(out GameManager gameManager))
            {
                gameManager.StartGame();
            }
        }

        private bool TrySpendCoins(int cost)
        {
            if(ServiceLocator.ForSceneOf(this).TryGetService<GameDataManager>(out GameDataManager gameDataManager))
            {
                int count = gameDataManager.GetCoinsCount();
                if (count >= cost)
                {
                    gameDataManager.SetCoinsCount(count - cost);
                    return true;
                }
            }
            
            return false;
        }

        private bool ShowAdForRespawn()
        {
            // Call your ad manager and return true if the ad is watched.
            // Return false otherwise.
            return false; // Stub: Replace with actual ad logic
        }

        private void PerformRespawn()
        {
            // Put the player back in game (reset position, etc.)
            // Reset necessary states like speed, etc.
            OnRespawnSucceeded?.Invoke();
        }

        private void Start()
        {
            ServiceLocator.ForSceneOf(this).Register<RespawnManager>(this);
            OnRespawnSucceeded.AddListener(SendPlayerAtAppropriatePosition);
        }
    }
}
