using UnityEngine;
using TMPro;

namespace Game
{
    public class CountdownUI : MonoBehaviour
    {
        private TMP_Text countdownText;

        private void Awake()
        {
            countdownText = GetComponent<TMP_Text>();
        }

        public void SetCountdownText(int seconds)
        {
            countdownText.text = seconds.ToString();
        }
    }
}
