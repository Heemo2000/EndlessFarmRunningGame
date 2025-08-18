using Game.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Gameplay
{
    public class FruitCountMultiplier : MonoBehaviour
    {
        [Min(1.0f)]
        [SerializeField] private float timeToBoostCount = 5.0f;

        public UnityEvent<int> OnMultiplierApplied;

        private HashSet<int> multipliersAlreadyDone;
        private Coroutine boostCoroutine;
        private bool shouldApplyMultiplier = false;
        
        public bool ActivateBoost(FruitType type,int count)
        {
            int multiplier = count < 100 ? 0 : count / 100 + 1;
            if (
                ServiceLocator.ForSceneOf(this).
                TryGetService<GameDataManager>(out GameDataManager gameDataManager)
               )
            {
                if(multiplier == 0)
                {
                    gameDataManager.SetFruitCount(type, count + 1);
                    return false;
                }

                if(multipliersAlreadyDone.Contains(multiplier))
                {
                    gameDataManager.SetFruitCount(type, count + 1);
                    return false;
                }

                if (boostCoroutine == null)
                {
                    boostCoroutine = StartCoroutine(ApplyBoost(multiplier, timeToBoostCount));
                }
                
                if(shouldApplyMultiplier)
                {
                    gameDataManager.SetFruitCount(type, count + multiplier);
                    OnMultiplierApplied?.Invoke(multiplier);
                }

                return true;
            }
            return false;
        }

        

        private IEnumerator ApplyBoost(int multiplier,float time)
        {
            shouldApplyMultiplier = true;
            yield return new WaitForSeconds(time);
            multipliersAlreadyDone.Add(multiplier);
            shouldApplyMultiplier = false;
            boostCoroutine = null;
        }

        private void Awake()
        {
            multipliersAlreadyDone = new HashSet<int>();
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            ServiceLocator.ForSceneOf(this).Register<FruitCountMultiplier>(this);
        }
    }
}
