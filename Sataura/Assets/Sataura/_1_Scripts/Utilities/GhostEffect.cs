using UnityEngine;

namespace Sataura
{
    public class GhostEffect : MonoBehaviour
    {
        [SerializeField] private GameObject ghostPrefab;
        [SerializeField] private SpriteRenderer sr;

        [SerializeField] private float createGhostTime = .02f;
        private float createGhostTimeCount = 0.0f;
        public bool isGhosting = false;
        [SerializeField] private float _timeExist = 0.3f; // Duration of the fade in seconds


        private void Update()
        {
            if(isGhosting)
            {
                if (Time.time - createGhostTimeCount > createGhostTime)
                {
                    createGhostTimeCount = Time.time;
                    CreateGhost();
                }
            }         
        }


        private void CreateGhost()
        {
            var ghostObject = Instantiate(ghostPrefab, transform.position, transform.rotation);
            ghostObject.transform.localScale = sr.gameObject.transform.localScale;
            Destroy(ghostObject, _timeExist + 0.1f);

            ghostObject.GetComponent<SpriteRenderer>().sprite = sr.sprite;
            StartCoroutine(FadeOut(ghostObject.GetComponent<SpriteRenderer>()));
        }




        private System.Collections.IEnumerator FadeOut(SpriteRenderer spriteRenderer)
        {          
            // Get the starting color and set the initial alpha value
            Color startColor = spriteRenderer.color;
            float startAlpha = startColor.a;

            // Calculate the target alpha value (fully transparent)
            float targetAlpha = 0f;

            // Calculate the fade increment per frame based on the duration
            float fadeIncrement = (startAlpha - targetAlpha) / _timeExist;

            // Gradually decrease the alpha value until it reaches the target
            while (spriteRenderer.color.a > targetAlpha)
            {
                // Calculate the new alpha value for the frame
                float newAlpha = spriteRenderer.color.a - (fadeIncrement * Time.deltaTime);

                // Set the new color with the updated alpha value
                spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, newAlpha);

                yield return null;
            }

            // Ensure the final color is fully transparent
            spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, targetAlpha);
        }
    }

}

