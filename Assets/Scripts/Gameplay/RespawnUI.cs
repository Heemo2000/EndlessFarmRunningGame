using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Game.Core;

namespace Game.Gameplay
{
    public class RespawnUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text coinsUIText;
        [SerializeField] private Button yesButton;
        [SerializeField] private Button noButton;

        public void ShowRespawnUI(int coins)
        {
            coinsUIText.text = coins.ToString();
            yesButton.gameObject.SetActive(coins > 10);
        }
    }
}
