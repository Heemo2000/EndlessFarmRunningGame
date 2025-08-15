using Game.Core;
using UnityEngine;

namespace Game.Gameplay
{
    public class Obstacle : MonoBehaviour, IInteractable
    {
        public void Interact()
        {
            if (ServiceLocator.ForSceneOf(this).
                   TryGetService<GameManager>(out GameManager gameManager))
            {
                gameManager.OnGameOver?.Invoke();
            }
        }
    }
}
