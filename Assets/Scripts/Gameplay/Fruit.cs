using Game.Core;
using UnityEngine;

namespace Game.Gameplay
{
    public class Fruit : MonoBehaviour, IInteractable
    {
        [SerializeField] private FruitType type;
        [Min(0.0f)]
        [SerializeField] private float rotatingSpeed = 2.0f;

        public FruitType Type { get => type;}

        public void Interact()
        {
            if(ServiceLocator.ForSceneOf(this).
               TryGetService<FruitManager>(out FruitManager fruitManager))
            {
                fruitManager.Unspawn(this);
            }
        }

        private void FixedUpdate()
        {
            transform.Rotate(Vector3.up * rotatingSpeed * Time.fixedDeltaTime);
        }
    }
}
