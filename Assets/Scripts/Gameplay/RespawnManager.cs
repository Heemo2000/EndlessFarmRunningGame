using Game.Core;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Gameplay
{
    public class RespawnManager : MonoBehaviour
    {
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

        // Your GDD states that player starts with 0 coins; handle elsewhere as needed.
    }
}
