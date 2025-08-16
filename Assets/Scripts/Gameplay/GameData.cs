using System.Collections.Generic;

namespace Game.Gameplay
{
    [System.Serializable]
    public class GameData
    {
        public int coinsCount = 0;
        public Dictionary<FruitType, FruitInventoryData> fruitInventory =
               new Dictionary<FruitType, FruitInventoryData>();
    }
}
