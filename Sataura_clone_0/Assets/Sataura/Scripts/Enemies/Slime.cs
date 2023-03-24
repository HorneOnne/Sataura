using System.Collections;
using UnityEngine;

namespace Sataura
{
    public class Slime : GroundEnemy
    {
        [Header("Jump")]
        [SerializeField] private float jumpForce = 3f;
        private float jumpDelay = 2.0f; // the delay in seconds before the player can jump again
        [SerializeField] private bool canJump = true; // a flag indicating whether the player can jump

        public override void MoveAI(Vector2 playerPosition)
        {
            if (Time.time - lastUpdate >= .1f)
                lastUpdate = Time.time;
            else
                return;

            if (isBeingKnockback == true || isDead == true) return;

            if (canJump && IsGrounded())
            {
                JumpAI(playerPosition);
            }


            // Move towards player
            Vector2 direction = (playerPosition - (Vector2)transform.position).normalized;        
            rb2D.velocity = new Vector2(direction.x * enemyData.moveSpeed, rb2D.velocity.y);


            // Flip sprite to face player
            if (direction.x > 0)
                sr.flipX = false;
            else if (direction.x < 0)
                sr.flipX = true;
        }


        float lastUpdate = 0.0f;
        private void JumpAI(Vector2 playerPosition)
        {          
            if (playerPosition.y > transform.position.y + 10)
            {
                JumpUp();
                canJump = false;
                Invoke("EnableJump", jumpDelay);
            }

            if (playerPosition.y < transform.position.y - 10)
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

        private IEnumerator ToggleTrigger()
        {
            boxCollider2D.isTrigger = true;
            yield return new WaitForSeconds(.5f);
            boxCollider2D.isTrigger = false;
        }





        public override void TakeDamage(int damaged)
        {
            base.TakeDamage(damaged);

            if (IsOutOfHealth() == false)
            {
                anim.SetTrigger("BeAttacked");
            }
        }

        public override void OnEnemyDead()
        {
            rb2D.velocity = Vector2.zero;
            anim.SetTrigger("Dead");

            SoundManager.Instance.PlaySound(SoundType.EnemyDie, enemyData.dieSFX);
        }

        private void EnableJump()
        {
            canJump = true;
        }

        protected override void ReturnToNetworkPool()
        {
            if (networkObject.IsSpawned)
                networkObject.Despawn();
        }
    }

}
