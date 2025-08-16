using Game.Core;
using UnityEngine;
using UnityEngine.Events;
using Game.DataPersistence;
using System.Collections.Generic;

namespace Game.Gameplay
{
    public class GameDataManager : MonoBehaviour
    {
        [SerializeField] private bool isInGame = true;
        public UnityEvent<int> OnCoinCountSet;
        public UnityEvent<FruitType, int> OnFruitCountSetInGame;

        private JsonDataService dataService;
        private GameData gameData = null;


        private Dictionary<FruitType, FruitInventoryData> fruitInventory;

        public void SaveData()
        {
            if(isInGame)
            {
                foreach(var key in fruitInventory.Keys)
                {
                    if(!gameData.fruitInventory.ContainsKey(key))
                    {
                        gameData.fruitInventory.Add(key,new FruitInventoryData() { fruitType = key, count = 0});
                    }

                    if(!fruitInventory.ContainsKey(key))
                    {
                        fruitInventory.Add(key, new FruitInventoryData() { fruitType = key, count = 0 });
                    }
                    gameData.fruitInventory[key].count += fruitInventory[key].count;
                }
            }

            dataService.SaveData<GameData>(Constants.GAME_DATA_RELATIVE_PATH,
                                           gameData,
                                           false);


        }

        public int GetCoinsCount()
        {
            return gameData.coinsCount;
        }

        public void SetCoinsCount(int count)
        {
            gameData.coinsCount = count;
            OnCoinCountSet?.Invoke(count);
        }

        public int GetFruitCount(FruitType type)
        {
            if(!isInGame)
            {
                if (!gameData.fruitInventory.ContainsKey(type))
                {
                    gameData.fruitInventory.Add(type,
                                                new FruitInventoryData() { fruitType = type, count = 0 });
                }

                return gameData.fruitInventory[type].count;
            }
            else
            {
                if(!fruitInventory.ContainsKey(type))
                {
                    fruitInventory.Add(type, new FruitInventoryData() { fruitType = type, count = 0 });
                }

                return fruitInventory[type].count;
            }
            
        }

        public void SetFruitCount(FruitType type, int count)
        {
            if(!isInGame)
            {
                if (!gameData.fruitInventory.ContainsKey(type))
                {
                    gameData.fruitInventory.Add(type,
                                                new FruitInventoryData() { fruitType = type, count = 0 });
                }

                gameData.fruitInventory[type].count = count;
            }
            else
            {
                if(!fruitInventory.ContainsKey(type))
                {
                    fruitInventory.Add (type, new FruitInventoryData() { fruitType = type, count = 0 });
                }

                fruitInventory[type].count = count;
                OnFruitCountSetInGame?.Invoke(type, count);
            }
            
            
        }

        private void Awake()
        {
            fruitInventory = new Dictionary<FruitType, FruitInventoryData>();
            dataService = new JsonDataService();
        }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            if(dataService.IsFileExists(Constants.GAME_DATA_RELATIVE_PATH))
            {
                gameData = dataService.LoadData<GameData>(Constants.GAME_DATA_RELATIVE_PATH, false);
            }
            else
            {
                gameData = new GameData();
            }
                
            ServiceLocator.ForSceneOf(this).Register<GameDataManager>(this);
        }

        private void OnDestroy()
        {
            SaveData();  
        }
    }
}
