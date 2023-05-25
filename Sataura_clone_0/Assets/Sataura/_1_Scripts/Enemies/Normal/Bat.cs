﻿using UnityEngine;
using System.Collections;

namespace Sataura
{
    public class Bat : FlyingEnemy
    {
        [SerializeField] private Sprite[] flySprites;
        [SerializeField] private Sprite[] dieSprites;

        [SerializeField] private float fps = 30f;
        private float fpsCounter = 0f;
        private int animationStep;



        public override void MoveAI(Vector2 target)
        {
            base.MoveAI(target);

            fpsCounter += Time.deltaTime;
            if (isDead)
            {
                if (fpsCounter >= 1 / fps)
                {
                    animationStep++;
                    if (animationStep == dieSprites.Length)
                    {
                        animationStep = 0;
                        sr.enabled = false;
                    }

                    sr.sprite = dieSprites[animationStep];
                    fpsCounter = 0.0f;
                }
                return;
            }

            Vector2 direction = target - (Vector2)transform.position;
            direction.Normalize();
            rb2D.MovePosition((Vector2)transform.position + direction * enemyData.moveSpeed * Time.fixedDeltaTime);

            // Fly anim
            if (fpsCounter >= 1 / fps)
            {
                animationStep++;
                if (animationStep == flySprites.Length)
                {
                    animationStep = 0;
                }

                sr.sprite = flySprites[animationStep];
                fpsCounter = 0.0f;
            }
        }





        protected override void Despawn()
        {
            if (networkObject.IsSpawned)
                networkObject.Despawn();
        }



       
        public override void OnEnemyDead()
        {
            base.OnEnemyDead();

            animationStep = 0;         
            rb2D.velocity = Vector2.zero;
            //StartCoroutine(ChangeValueOverTime(1.5f, 0f, .5f));
            SoundManager.Instance.PlaySound(SoundType.EnemyDie, enemyData.dieSFX);
        }
    }

}
