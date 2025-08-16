using UnityEngine;
using System;
namespace Game.Gameplay
{
    [Serializable]
    public class ChunkData
    {
        public ChunkType chunkType = ChunkType.None;
        public Chunk[] prefabs;
        [Min(20)]
        public int chunkCount = 60;
    }
}
