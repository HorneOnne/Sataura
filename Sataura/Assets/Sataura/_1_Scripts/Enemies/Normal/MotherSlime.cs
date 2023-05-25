﻿using System.Collections;
using UnityEngine;
using Unity.Netcode;

namespace Sataura
{
    public class MotherSlime : GroundEnemy
    {
        [SerializeField] private Sprite[] moveSprites;
        [SerializeField] private Sprite[] dieSprites;

        [Header("Jump")]
        [SerializeField] private float jumpForce = 3f;
        private float jumpDelay = 2.0f; // the delay in seconds before the player can jump again
        [SerializeField] private bool canJump = true; // a flag indicating whether the player can jump

        private float fps = 15f;
        private float fpsCounter = 0f;
        private int animationStep;


        // Jump movement
        private float jumpInterval = 3.0f;
        private float jumpIntervalCount = 0.0f;


        public override void MoveAI(Vector2 playerPosition)
        {
            base.MoveAI(playerPosition);

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

            // Move anim
            if (fpsCounter >= 1 / 10f)
            {
                animationStep++;
                if (animationStep == moveSprites.Length)
                {
                    animationStep = 0;
                }

                sr.sprite = moveSprites[animationStep];
                fpsCounter = 0.0f;
            }



            // Move towards player
            Vector2 direction = (playerPosition - (Vector2)transform.position);

            if (IsGrounded())
            {
                if (Time.time - jumpIntervalCount > jumpInterval)
                {
                    jumpIntervalCount = Time.time;
                    rb2D.AddForce(new Vector2(direction.normalized.x, 4) * enemyData.moveSpeed);
                }
                else
                {
                    rb2D.velocity = new Vector2(direction.normalized.x * 3, rb2D.velocity.y);
                }
            }

            // Jump
            if (canJump && IsGrounded())
            {
                JumpAI(playerPosition);
            }


            // Flip sprite to face player
            if (direction.x > 0)
                sr.flipX = false;
            else if (direction.x < 0)
                sr.flipX = true;

            // Limit fall velocity
            if (rb2D.velocity.y > 40)
                rb2D.velocity = new Vector2(rb2D.velocity.x, 40);
        }


        float lastUpdate = 0.0f;
        private void JumpAI(Vector2 playerPosition)
        {
            float xDistance = Mathf.Abs(playerPosition.x - transform.position.x);


            if (playerPosition.y > transform.position.y + 10 && xDistance < 10)
            {
                JumpUp();
                canJump = false;
                Invoke("EnableJump", jumpDelay);
            }

            if (playerPosition.y < transform.position.y - 10 && xDistance < 10)
            {
                JumpDown();
                canJump = false;
                Invoke("EnableJump", jumpDelay);
            }
        }

        private void JumpUp()
        {
            rb2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        private void JumpDown()
        {
            StartCoroutine(ToggleTrigger());
        }

        private void EnableJump()
        {
            canJump = true;
        }

        private IEnumerator ToggleTrigger()
        {
            boxCollider2D.isTrigger = true;
            yield return new WaitForSeconds(.5f);
            boxCollider2D.isTrigger = false;
        }


        public override void OnEnemyDead()
        {
            // Spawn tiny black slime
            var tinyBlackSlimePrefab = GameDataManager.Instance.GetEnemyPrefab(EnemyType.BabySlime);
            for (int i = 0; i < 3; i++)
            {
                var tinyBlackSlimeObject = Instantiate(tinyBlackSlimePrefab, transform.position + new Vector3(Random.Range(-1f, 1f), 0f), Quaternion.identity);
                tinyBlackSlimeObject.GetComponent<BaseEnemy>().SetFollowTarget(base.playerTranform);

                var tinyBlackSlimeNetworkObject = tinyBlackSlimeObject.GetComponent<NetworkObject>();
                tinyBlackSlimeNetworkObject.Spawn();
                IngameInformationManager.Instance.currentTotalEnemiesIngame++;
            }


            base.OnEnemyDead();
            animationStep = 0;
            rb2D.velocity = Vector2.zero;
            SoundManager.Instance.PlaySound(SoundType.EnemyDie, enemyData.dieSFX);                
        }



        protected override void Despawn()
        {
            if (networkObject.IsSpawned)
                networkObject.Despawn();
        }
    }
}
