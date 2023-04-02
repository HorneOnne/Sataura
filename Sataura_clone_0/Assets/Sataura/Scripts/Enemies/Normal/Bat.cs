using UnityEngine;
using System.Collections;

namespace Sataura
{
    public class Bat : FlyingEnemy
    {
        [SerializeField] private Sprite[] sprites;
        [SerializeField] private float fps = 30f;
        private float fpsCounter = 0f;
        private int animationStep;

 

        public override void MoveAI(Vector2 target)
        {
            Vector2 direction = target - (Vector2)transform.position;
            direction.Normalize();
            rb2D.MovePosition((Vector2)transform.position + direction * 10 * Time.fixedDeltaTime);


            // Fly anim
            fpsCounter += Time.deltaTime;
            if (fpsCounter >= 1 / fps)
            {
                animationStep++;
                if (animationStep == sprites.Length)
                {
                    animationStep = 0;
                }

                sr.sprite = sprites[animationStep];

                fpsCounter = 0.0f;
            }
        }

        protected override void ReturnToNetworkPool()
        {
            if (networkObject.IsSpawned)
                networkObject.Despawn();
        }



        

        public override void OnEnemyDead()
        {
            base.OnEnemyDead();

            rb2D.velocity = Vector2.zero;

            StartCoroutine(ChangeValueOverTime(1.5f, 0f, .5f));
            SoundManager.Instance.PlaySound(SoundType.EnemyDie, enemyData.dieSFX);
        }


        private float currentValue = 1f;

        private IEnumerator ChangeValueOverTime(float startValue, float endValue, float duration)
        {
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                currentValue = Mathf.Lerp(startValue, endValue, elapsedTime / duration);

                sr.material.SetFloat("_Dissolve_Amount", currentValue);

                yield return null;
            }

            currentValue = endValue;
        }
    }

}
