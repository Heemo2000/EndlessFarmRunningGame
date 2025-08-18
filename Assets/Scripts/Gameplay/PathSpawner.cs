using System.Collections.Generic;
using UnityEngine;
using Game.ObjectPoolHandling;
using Game.Core;
using System.Diagnostics;
using System.Linq;

using Debug = UnityEngine.Debug;

namespace Game.Gameplay
{
    public class PathSpawner : MonoBehaviour
    {
        [SerializeField] private Runner runner; // Player reference
        [SerializeField] private ChunkData[] chunkData;
        [SerializeField] private DifficultyData[] difficulties;
        [Min(1)]
        [SerializeField] private int startingTileCollectionCount = 2;
        [SerializeField] private float distanceBetweenChunks = 3.0f; // Length of each path tile
        [Min(0.1f)]
        [SerializeField] private float pathDistance = 50.0f;
        [Min(50.0f)]
        [SerializeField] private float chunkVisibilityDistance = 200.0f;

        private Vector3 currentSpawnPosition = Vector3.zero;
        private List<Chunk> activeChunks = new List<Chunk>();
        private Dictionary<ChunkType, ObjectPool<Chunk>> poolDict;
        private Stopwatch timer;


        public void StartTimer()
        {
            timer.Start();
        }

        public void StopTimer()
        {
            timer.Stop();
        }

        private void Awake()
        {
            poolDict = new Dictionary<ChunkType, ObjectPool<Chunk>>();
            timer = new Stopwatch();
        }
        void Start()
        {
            if (poolDict.Count == 0)
            {
                foreach(var data in chunkData)
                {
                    var pool = new ObjectPool<Chunk>(() => CreateChunk(data.prefabs),
                                                     OnGetChunk,
                                                     OnReturnChunk,
                                                     OnDestroyChunk,
                                                     data.chunkCount,
                                                     true);

                    poolDict.Add(data.chunkType, pool);
                                    
                }
            }

            ServiceLocator.ForSceneOf(this).Register<PathSpawner>(this);
            UnityEngine.Random.InitState((int)System.DateTime.Now.Ticks);

            currentSpawnPosition = transform.position;

            //Spawn initial tiles
            for(int i = 0; i < startingTileCollectionCount; i++)
            {
                SpawnTiles(difficulties[0]);
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
            And spawn some new ones,
            And then, disable the child objects of further chunks w.r.t player
            */

            // Debug.Log("Path Distance: " + pathDistance + ", PlayerZ: " + playerZ);
            if (playerZ > pathDistance)
            {
                DeleteTiles();
                ResetOriginOfTiles(playerZ);
                runner.MoveBack(playerZ);
                SpawnTiles((int)timer.Elapsed.TotalMinutes, timer.Elapsed.Seconds);
                HideChildObjectsOfChunks();
            }

        }

        private void HideChildObjectsOfChunks()
        {
            foreach(var chunk in activeChunks)
            {
                float distanceZ = chunk.transform.position.z - runner.transform.position.z;
                if (distanceZ >= chunkVisibilityDistance)
                {
                    chunk.OnPlayerExitRange();
                }
                else if (distanceZ >= 0.0f)
                {
                    chunk.OnPlayerEnterRange();
                }
            }
        }

        private Chunk CreateChunk(Chunk[] prefabs)
        {
            int randomIndex = UnityEngine.Random.Range(0, prefabs.Length);
            var instance = Instantiate(prefabs[randomIndex], Vector3.zero, Quaternion.identity);
            instance.transform.parent = transform;
            instance.gameObject.SetActive(false);
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
            foreach (Chunk chunk in activeChunks)
            {
                chunk.MoveBack(distance);
            }

            currentSpawnPosition.z -= distance;
        }
        
        private void SpawnTiles(int minutes, int seconds)
        {
            DifficultyData data = difficulties.FirstOrDefault((element) => 
                                          element.startingMinute >= minutes && 
                                          element.startingSecond >= seconds);
            Debug.Log("Running SpawnTiles method with time parameters");
            SpawnTiles(data);
        }

        private void SpawnTiles(DifficultyData data)
        {
            Debug.Log("Running SpawnData with difficultyData parameters");
            foreach(var element in data.progressionOrder)
            {
                Chunk newTile = poolDict[element].Get();
                newTile.gameObject.transform.position = currentSpawnPosition;
                newTile.gameObject.transform.rotation = Quaternion.identity;

                activeChunks.Add(newTile);
                currentSpawnPosition.z += newTile.Length + distanceBetweenChunks;
            }
            
        }

        void DeleteTiles()
        {
            int lastTileIndexBehindThePlayer = -1;
            for (int i = 0; i < activeChunks.Count; i++)
            {
                Chunk chunk = activeChunks[i];

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

            for (int i = 0; i < lastTileIndexBehindThePlayer - 2; i++)
            {
                poolDict[activeChunks[0].Type].ReturnToPool(activeChunks[0]);
                activeChunks.RemoveAt(0);
            }
        }
    }

}
