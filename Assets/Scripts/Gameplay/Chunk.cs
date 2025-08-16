using UnityEngine;

namespace Game.Gameplay
{
    public class Chunk : MonoBehaviour, IDistanceBasedToggle
    {
        [SerializeField] private ChunkType type;
        [Min(1.0f)]
        [SerializeField] private float length = 10.0f;
        [Min(1.0f)]
        [SerializeField] private float laneWidth = 5.0f;
        [SerializeField] private Fruit[] fruitsOnChunk;
        [SerializeField] private Obstacle[] obstaclesOnChunk; 
        public float Length { get => length; }
        public ChunkType Type { get => type; set => type = value; }

        public void OnPlayerEnterRange()
        {
            foreach(var fruit in fruitsOnChunk)
            {
                fruit.gameObject.SetActive(true);
            }

            foreach(var obstacle in obstaclesOnChunk)
            {
                obstacle.gameObject.SetActive(true);
            }
        }

        public void OnPlayerExitRange()
        {
            foreach (var fruit in fruitsOnChunk)
            {
                fruit.gameObject.SetActive(false);
            }

            foreach (var obstacle in obstaclesOnChunk)
            {
                obstacle.gameObject.SetActive(false);
            }
        }

        public void MoveBack(float distance)
        {
            transform.position -= Vector3.forward * distance;
        }

        private void OnEnable()
        {
            foreach (var fruit in fruitsOnChunk) 
            {
                fruit.gameObject.SetActive(true);
            }
        }

        private void OnDisable()
        {
            foreach (var fruit in fruitsOnChunk)
            {
                fruit.gameObject.SetActive(false);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, 
                            transform.position + transform.forward * length);

            Gizmos.color = Color.black;
            Vector3 left = transform.position - transform.right * laneWidth;
            Vector3 right = transform.position + transform.right * laneWidth;

            Gizmos.DrawLine(left, left + transform.forward * length);
            Gizmos.DrawLine(right, right + transform.forward * length);
            
        }

        
    }
}
