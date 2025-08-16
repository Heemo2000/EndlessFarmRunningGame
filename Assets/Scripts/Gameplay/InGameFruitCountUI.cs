using UnityEngine;
using TMPro;

namespace Game.Gameplay
{
    public class InGameFruitCountUI : MonoBehaviour
    {
        private TMP_Text fruitCountUI;

        public void SetFruitCount(FruitType type, int count)
        {
            fruitCountUI.text = count.ToString();
        }

        private void Awake()
        {
            fruitCountUI = GetComponent<TMP_Text>();
        }
    }

}
