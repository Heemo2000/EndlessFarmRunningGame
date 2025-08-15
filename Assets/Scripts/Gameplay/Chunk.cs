using UnityEngine;

namespace Game.Gameplay
{
    public class Chunk : MonoBehaviour
    {
        [Min(1.0f)]
        [SerializeField] private float length = 10.0f;
        public float Length { get => length; }

        public void MoveBack(float distance)
        {
            transform.position -= Vector3.forward * distance;
        }
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, 
                            transform.position + transform.forward * length);
        }
    }
}
