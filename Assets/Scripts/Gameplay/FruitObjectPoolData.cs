using UnityEngine;

namespace Game.Gameplay
{
    [System.Serializable]
    public class FruitObjectPoolData
    {
        public FruitType fruitType = FruitType.None;
        public Fruit prefab;
        [Min(50)]
        public int poolCount = 100;
    }
}
