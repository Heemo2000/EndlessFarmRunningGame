using UnityEngine;

namespace Game.Gameplay
{
    [System.Serializable]
    public class DifficultyData
    {
        [Range(0,10)]
        public int startingMinute;
        [Range(0,60)]
        public int startingSecond;
        public ChunkType[] progressionOrder;
    }
}
