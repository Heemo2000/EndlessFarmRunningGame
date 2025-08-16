using Game.Core;
using UnityEngine;

namespace Game.Gameplay
{
    public class Obstacle : MonoBehaviour, IInteractable
    {
        [SerializeField] private bool allowInteraction = true;
        public void Interact()
        {
            if(!allowInteraction)
            {
                return;
            }

            if (ServiceLocator.ForSceneOf(this).
                   TryGetService<GameManager>(out GameManager gameManager))
            {
                gameManager.OnGameOver?.Invoke();
            }
        }
    }
}
