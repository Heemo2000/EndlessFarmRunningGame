using Game.Core;
using System.Collections.Generic;
using UnityEngine;
using Game.ObjectPoolHandling;

namespace Game.Gameplay
{
    public class PathSpawner : MonoBehaviour
    {
        [SerializeField] private Runner runner; // Player reference
        [SerializeField] private Chunk[] startingChunks;
        [SerializeField] private Chunk[] pathPrefabs; // Different types of paths (some with gaps)
        [Min(100)]
        [SerializeField] private int initialChunkCount = 100;
        [SerializeField] private float distanceBetweenChunks = 3.0f; // Length of each path tile
        [Min(1)]
        [SerializeField] private int startingTilesCount = 7; // Number of tiles with obstacles at start
        [Min(1)]
        [SerializeField] private int tilesCountAfterResettingOrigin = 5;
        [Min(0.1f)]
        [SerializeField] private float pathDistance = 50.0f;

        private Vector3 currentSpawnPosition = Vector3.zero;
        private List<Chunk> activeTiles = new List<Chunk>();
        private ObjectPool<Chunk> chunkPool;
        private int chunkNo = 1;
        void Start()
        {
            if (chunkPool == null)
            {
                chunkPool = new ObjectPool<Chunk>(CreateChunk,
                                                  OnGetChunk,
                                                  OnReturnChunk,
                                                  OnDestroyChunk,
                                                  initialChunkCount,
                                                  true);
            }

            ServiceLocator.ForSceneOf(this).Register<PathSpawner>(this);
            Random.InitState((int)System.DateTime.Now.Ticks);

            currentSpawnPosition = transform.position;
            
            //Adjust spawnZ for initial tiles
            AdjustSpawnZ();


            //Then spawn tiles
            for (int i = 0; i < startingTilesCount; i++)
            {
                SpawnTile();
            }
        }

        void FixedUpdate()
        {
            float playerZ = 0;
            if (runner != null) playerZ = runner.transform.position.z;

            /*
            Check if we need to reset the origin.
            If yes, then, delete the tiles behind the player.
            then reset origin of all the tiles.
            then, reset player to it's initial position,
            And spawn some new ones
            */

            // Debug.Log("Path Distance: " + pathDistance + ", PlayerZ: " + playerZ);
            if (playerZ > pathDistance)
            {
                DeleteTiles();
                ResetOriginOfTiles(playerZ);
                runner.MoveBack(playerZ);
                for (int i = 0; i < tilesCountAfterResettingOrigin; i++)
                {
                    SpawnTile();
                }
            }

        }

        private Chunk CreateChunk()
        {
            int randomIndex = Random.Range(0, pathPrefabs.Length);
            var instance = Instantiate(pathPrefabs[randomIndex], Vector3.zero, Quaternion.identity);
            instance.transform.parent = transform;
            instance.gameObject.SetActive(false);
            instance.transform.name = "Chunk " + chunkNo.ToString();
            chunkNo++;
            return instance;
        }

        private void OnGetChunk(Chunk chunk)
        {
            chunk.gameObject.SetActive(true);
        }

        private void OnReturnChunk(Chunk chunk)
        {
            chunk.gameObject.SetActive(false);
        }

        private void OnDestroyChunk(Chunk chunk)
        {
            Destroy(chunk.gameObject);
        }

        private void ResetOriginOfTiles(float distance)
        {
            foreach (Chunk chunk in activeTiles)
            {
                chunk.MoveBack(distance);
            }

            currentSpawnPosition.z -= distance;
        }

        private void AdjustSpawnZ()
        {
            for (int i = 0; i < startingChunks.Length; i++)
            {
                Chunk current = startingChunks[i];
                Chunk next = (i + 1 < startingChunks.Length) ? startingChunks[i + 1] : null;
                currentSpawnPosition.z += current.Length;
                if (next != null)
                {
                    float gap = (next.transform.position.z - (current.transform.position.z + current.Length));
                    currentSpawnPosition.z += gap;
                }

                activeTiles.Add(current);
            }

            currentSpawnPosition.z += distanceBetweenChunks;
        }

        void SpawnTile()
        {
            // Randomly pick a prefab from the list
            Chunk prefab = pathPrefabs[Random.Range(0, pathPrefabs.Length)];
            Chunk newTile = chunkPool.Get();//Instantiate(prefab, Vector3.forward * spawnZ, Quaternion.identity);
            newTile.gameObject.transform.position = currentSpawnPosition;
            newTile.gameObject.transform.rotation = Quaternion.identity;

            activeTiles.Add(newTile);
            currentSpawnPosition.z += newTile.Length + distanceBetweenChunks;
        }

        void DeleteTiles()
        {
            int lastTileIndexBehindThePlayer = -1;
            for (int i = 0; i < activeTiles.Count; i++)
            {
                Chunk chunk = activeTiles[i];

                if (chunk.transform.position.z < runner.transform.position.z)
                {
                    Debug.Log("Chunk position: " + chunk.transform.position);
                    Debug.Log("Chunk to delete: " + chunk.transform.name);
                    lastTileIndexBehindThePlayer = i;
                }
                else
                {
                    break;
                }
            }

            for (int i = 0; i < lastTileIndexBehindThePlayer; i++)
            {
                chunkPool.ReturnToPool(activeTiles[0]);
                activeTiles.RemoveAt(0);
            }
        }
    }

}
