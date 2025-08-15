using UnityEngine;
using Game.Core;
using Game.ObjectPoolHandling;
using Game.DataPersistence;

namespace Game.Gameplay
{
    public class FruitManager : MonoBehaviour
    {
        [SerializeField] private FruitObjectPoolData poolData;
        private ObjectPool<Fruit> fruitPool;
        private JsonDataService jsonDataService;
        private InventoryData inventoryData;
        private string relativePath = "/inventory_data.json";

        public void SaveData()
        {
            jsonDataService.SaveData<InventoryData>(relativePath, 
                                                    inventoryData, 
                                                    false);
        }

        public void IncreaseCount(FruitType fruitType)
        {
            if (!inventoryData.data.ContainsKey(fruitType))
            {
                inventoryData.data[fruitType] = new FruitInventoryData();
            }
            inventoryData.data[fruitType].count++;
        }

        public void SetCount(FruitType fruitType, int count)
        {
            if (!inventoryData.data.ContainsKey(fruitType))
            {
                inventoryData.data[fruitType] = new FruitInventoryData();
            }
            inventoryData.data[fruitType].count = count;
        }

        public int GetCount(FruitType fruitType)
        {
            if (!inventoryData.data.ContainsKey(fruitType))
            {
                inventoryData.data[fruitType] = new FruitInventoryData();
            }
            return inventoryData.data[fruitType].count;
        }

        //Responsible for spawning fruit at specific position and location.
        public void Spawn(Vector3 position, Quaternion rotation)
        {
            var instance = fruitPool.Get();
            if (instance != null)
            {
                instance.transform.position = position;
                instance.transform.rotation = rotation;
            }
        }


        //Responsible for unspawning fruit
        public void Unspawn(Fruit fruit)
        {
            fruitPool.ReturnToPool(fruit);
        }

        private void Awake()
        {
            jsonDataService = new JsonDataService();

            if (jsonDataService.IsFileExists(relativePath))
            {
                inventoryData = jsonDataService.LoadData<InventoryData>(relativePath, false);
            }
            else
            {
                inventoryData = new InventoryData();
            }
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            if (fruitPool == null)
            {
                fruitPool = new ObjectPool<Fruit>
                                (
                                    CreateFruit,
                                    OnGetFruit,
                                    OnReturnFruit,
                                    OnDestroyFruit,
                                    poolData.poolCount,
                                    true
                                );
            }

            ServiceLocator.ForSceneOf(this).Register<FruitManager>(this);
        }

        private void OnDestroyFruit(Fruit fruit)
        {
            Destroy(fruit.gameObject);
        }

        private void OnReturnFruit(Fruit fruit)
        {
            fruit.gameObject.SetActive(false);
        }

        private void OnGetFruit(Fruit fruit)
        {
            fruit.gameObject.SetActive(true);
        }

        private Fruit CreateFruit()
        {
            var instance = Instantiate(poolData.prefab, 
                                       Vector3.zero, 
                                       Quaternion.identity);
            instance.gameObject.SetActive(false);
            instance.transform.parent = transform;
            return instance;
        }
    }
}
