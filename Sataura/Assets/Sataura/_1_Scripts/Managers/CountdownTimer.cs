using UnityEngine;
using TMPro;

namespace Sataura
{
    public class CountdownTimer : Singleton<CountdownTimer> 
    {
        public float totalTime = 120f; // Total time for the countdown in seconds
        private float timeLeft; // Time left for the countdown in seconds

        public TextMeshProUGUI countdownText; // Reference to the Text component that displays the countdown


        #region Properties
        public float TimeLeft { get { return timeLeft; } }  
        #endregion

        void Start()
        {
            timeLeft = totalTime;
        }

        void Update()
        {
            timeLeft -= Time.deltaTime;

            // Calculate minutes and seconds
            int minutes = Mathf.FloorToInt(timeLeft / 60f);
            int seconds = Mathf.FloorToInt(timeLeft % 60f);

            // Update countdown text
            countdownText.text = string.Format("{0:0}:{1:00}", minutes, seconds);

            // Check if countdown is finished
            if (timeLeft <= 0f)
            {
                // Countdown is finished, do something here
                Debug.Log("Countdown finished!");
                countdownText.text = "Time's up!";
            }
        }
    }
}

