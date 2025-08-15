using UnityEngine;
using Game.Core;
using System.Collections;
namespace Game.Gameplay
{
    public class FlowerPot : Obstacle
    {
        [SerializeField] private bool shouldSpawnObstacleOnTop = false;
        [Min(0.1f)]
        [SerializeField] private float fruitOffsetY = 1.0f;

        private Coroutine spawnCoroutine;
        private IEnumerator SpawnFruit()
        {
            if(!shouldSpawnObstacleOnTop)
            {
                yield break;
            }

            FruitManager fruitManager;
            while(!ServiceLocator.ForSceneOf(this).
                              TryGetService<FruitManager>
                              (out fruitManager))
            {
                yield return null;
            }

            fruitManager.Spawn(transform.position + Vector3.up * fruitOffsetY,
                                   Quaternion.identity);
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            if (spawnCoroutine == null)
            {
                spawnCoroutine = StartCoroutine(SpawnFruit());
            }
        }

        private void OnDisable()
        {
            spawnCoroutine = null;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.black;
            Gizmos.DrawLine(transform.position, 
                            transform.position + Vector3.up * fruitOffsetY);
        }
    }
}
