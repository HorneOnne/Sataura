using System.Collections;
using UnityEngine;

namespace Sataura
{
    public class Slime : GroundEnemy
    {
        [SerializeField] private Sprite[] moveSprites;
        [SerializeField] private Sprite[] dieSprites;

        [Header("Jump")]
        [SerializeField] private float jumpForce = 3f;
        private float jumpDelay = 2.0f; // the delay in seconds before the player can jump again
        [SerializeField] private bool canJump = true; // a flag indicating whether the player can jump
        //[SerializeField] protected Animator anim;


        [SerializeField] private float fps = 30f;
        private float fpsCounter = 0f;
        private int animationStep;

        public override void MoveAI(Vector2 playerPosition)
        {
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
            fpsCounter += Time.deltaTime;
            if (fpsCounter >= 1 / fps)
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
            rb2D.velocity = new Vector2(direction.normalized.x * enemyData.moveSpeed, rb2D.velocity.y);
            float distance = direction.magnitude;

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
            animationStep = 0;
            base.OnEnemyDead();
            rb2D.velocity = Vector2.zero;
            //anim.SetTrigger("Dead");
            SoundManager.Instance.PlaySound(SoundType.EnemyDie, enemyData.dieSFX);
        }

       

        protected override void ReturnToNetworkPool()
        {
            if (networkObject.IsSpawned)
                networkObject.Despawn();
        }
    }

}
