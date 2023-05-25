using UnityEngine;
using System.Collections;

namespace Sataura
{
    public class GreenPortalVFX : MonoBehaviour
    {
        [Header("Particle System")]
        public ParticleSystem _mainPs;
        public ParticleSystem _sparkPs;
        public ParticleSystem _smokePs;
        public ParticleSystem _distortionPs;

        [Header("Scale")]
        private Vector3 initialScale;

        private void Start()
        {
            initialScale = transform.localScale;          
        }


        public void ScaleOverTime(Vector3 targetScale, float scaleDuration)
        {
            StartCoroutine(ScaleCoroutine(targetScale, scaleDuration));
        }

        private IEnumerator ScaleCoroutine(Vector3 targetScale, float scaleDuration)
        {
            float elapsedTime = 0f;

            while (elapsedTime < scaleDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / scaleDuration);
                transform.localScale = Vector3.Lerp(initialScale, targetScale, t);
                yield return null;
            }

            // Ensure that the scale reaches the exact target scale
            transform.localScale = targetScale;
        }

        public void StopVFX()
        {
            if (_mainPs.isPlaying || _mainPs.isEmitting == true)
            {
                _mainPs.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }

            /*if (_sparkPs.isPlaying || _sparkPs.isEmitting == true)
            {
                _sparkPs.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }


            if (_smokePs.isPlaying || _smokePs.isEmitting == true)
            {
                _smokePs.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }

            if (_distortionPs.isPlaying || _distortionPs.isEmitting == true)
            {
                _distortionPs.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }*/
        }
    }
}

