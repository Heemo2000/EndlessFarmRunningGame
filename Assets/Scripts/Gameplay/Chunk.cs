using UnityEngine;

namespace Game.Gameplay
{
    public class Chunk : MonoBehaviour
    {
        [SerializeField] private ChunkType type;
        [Min(1.0f)]
        [SerializeField] private float length = 10.0f;
        [Min(1.0f)]
        [SerializeField] private float laneWidth = 5.0f;
        public float Length { get => length; }
        public ChunkType Type { get => type; set => type = value; }

        public void MoveBack(float distance)
        {
            transform.position -= Vector3.forward * distance;
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
