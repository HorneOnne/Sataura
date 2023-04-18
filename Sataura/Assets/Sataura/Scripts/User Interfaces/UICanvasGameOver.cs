using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Sataura
{
    public class UICanvasGameOver : Singleton<UICanvasGameOver>
    {
        [Header("References")]
        [SerializeField] private Canvas canvasObject;
        [SerializeField] private Image borderImage;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private GameObject gameOverPanel;

        [SerializeField] private float fadeDuration = 1.0f; // The duration of the fade out in seconds
        private float timer = 0.0f; // A timer to keep track of the current elapsed time


        private void Start()
        {
            borderImage.color = new Color(borderImage.color.r, borderImage.color.g, borderImage.color.b, 0.0f);
            backgroundImage.color = new Color(backgroundImage.color.r, backgroundImage.color.g, backgroundImage.color.b, 0.0f);
            gameOverPanel.SetActive(false);
            canvasObject.enabled = false;
        }


        /*private IEnumerator FadeAlphaValue(Image imageToFade, float startingAlpha, float targetAlpha)
        {
            // Calculate the difference between the target alpha and starting alpha
            float alphaDifference = targetAlpha - startingAlpha;

            // Fade the alpha value over the specified duration of time
            float timer = 0.0f;
            while (timer < fadeDuration)
            {
                // Calculate the current alpha value based on the elapsed time and fade duration
                float currentAlpha = startingAlpha + (alphaDifference * (timer / fadeDuration));

                // Update the alpha value of the image
                Color imageColor = imageToFade.color;
                imageColor.a = currentAlpha;
                imageToFade.color = imageColor;

                // Wait for the next frame
                yield return null;

                // Increment the timer by the elapsed time since the last frame
                timer += Time.deltaTime;
            }

            // Update the alpha value of the image to the target value to ensure accuracy
            Color finalColor = imageToFade.color;
            finalColor.a = targetAlpha;
            imageToFade.color = finalColor;
        }*/


        private IEnumerator FadeIn(Image imageToFade, float startingAlpha, float targetAlpha)
        {
            yield return new WaitForSeconds(0.1f);
            Debug.Log("Fade in called");
            // Calculate the difference between the target alpha and starting alpha
            float alphaDifference = targetAlpha - startingAlpha;

            // Fade in the alpha value over the specified duration of time
            float timer = 0.0f;
            while (timer < 1.0f)
            {
                // Calculate the current alpha value based on the elapsed time and fade duration
                float currentAlpha = startingAlpha + (alphaDifference * (timer / fadeDuration));

                // Update the alpha value of the image
                Color imageColor = imageToFade.color;
                imageColor.a = currentAlpha;
                imageToFade.color = imageColor;

                // Wait for the next frame
                yield return null;

                // Increment the timer by the elapsed time since the last frame
                timer += Time.deltaTime;
            }

            // Update the alpha value of the image to the target value to ensure accuracy
            Color finalColor = imageToFade.color;
            finalColor.a = targetAlpha;
            imageToFade.color = finalColor;
        }


        public void SetActive()
        {
            canvasObject.enabled = true;
            
            StartCoroutine(LoadGameOverPanel());

            //Invoke(nameof(FreezeTime), 1.5f);
        }


        private void FreezeTime()
        {
            Time.timeScale = 0.0f;
        }

        private IEnumerator LoadGameOverPanel()
        {
            StartCoroutine(FadeIn(backgroundImage, 0.0f, 0.3f));
            yield return StartCoroutine(FadeIn(borderImage, 0.0f, 1.0f));

            gameOverPanel.SetActive(true);
            FreezeTime();
        }
    }
}


