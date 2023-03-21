using UnityEngine;

namespace Sataura
{
    public class Slime : GroundEnemy
    {
        [Header("Jump")]
        [SerializeField] private float jumpForce = 5f;
        [SerializeField] private float jumpRange = 20f;

        public AudioClip deadAudio;

        private float jumpDelay = 2.0f; // the delay in seconds before the player can jump again
        private bool canJump = true; // a flag indicating whether the player can jump

        public override void MoveAI(Vector2 playerPosition)
        {
            if (isBeingKnockback == true || isDead == true) return;
       
            if (canJump && IsGrounded())
            {
                JumpAI(playerPosition);
                //anim.SetFloat("VelocityY", rb2D.velocity.y);
            }

            //anim.SetBool("IsGround", IsGrounded());
            

            // Move towards player
            Vector2 direction = (playerPosition - (Vector2)transform.position).normalized;
            rb2D.velocity = new Vector2(direction.x * enemyData.moveSpeed, rb2D.velocity.y);


            // Flip sprite to face player
            if (direction.x > 0)
                sr.flipX = false;
            else if (direction.x < 0)
                sr.flipX = true;
        }


        private void JumpAI(Vector2 playerPosition)
        {
            // Check if player is within range
            float distanceToPlayer = Vector2.Distance(transform.position, playerPosition);
            if (distanceToPlayer <= jumpRange)
            {
                // Check if player is above enemy
                if (playerPosition.y > transform.position.y + 0.5f)
                {
                    // Jump towards player
                    rb2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                }
            }
            canJump = false;
            Invoke("EnableJump", jumpDelay);
        }


        public override void TakeDamage(int damaged)
        {
            base.TakeDamage(damaged);

            if(IsOutOfHealth() == false)
            {
                anim.SetTrigger("BeAttacked");
            }
        }

        public override void OnEnemyDead()
        {
            rb2D.velocity = Vector2.zero;
            anim.SetTrigger("Dead");
            SoundManager.PlaySound(deadAudio);
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
