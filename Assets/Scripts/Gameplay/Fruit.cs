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
            //Increase the fruit count
            if(ServiceLocator.ForSceneOf(this).TryGetService<GameDataManager>(out GameDataManager gameDataManager))
            {
                int count = gameDataManager.GetFruitCount(type);
                gameDataManager.SetFruitCount(type, count + 1);
            }
            //Disable the gameobject.
            gameObject.SetActive(false);
        }

        private void FixedUpdate()
        {
            transform.Rotate(Vector3.up * rotatingSpeed * Time.fixedDeltaTime);
        }
    }
}
