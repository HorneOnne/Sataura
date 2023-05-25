using UnityEngine;
using System.Collections;

namespace Sataura
{
    public class Golem : GroundEnemy
    {
        [SerializeField] private Sprite[] _moveSprites;
        [SerializeField] private Sprite[] _attackSprites;
        [SerializeField] private Sprite[] _dieSprites;

        [Header("Jump")]
        [SerializeField] private float _jumpForce = 3f;
        private float _jumpDelay = 2.0f; // the delay in seconds before the player can jump again
        [SerializeField] private bool canJump = true; // a flag indicating whether the player can jump

        private float _fps = 10f;
        private float _fpsCounter = 0f;
        private int _animationStep;


        // Jump movement
        private float _jumpInterval = 3.0f;
        private float _jumpIntervalCount = 0.0f;



        private enum GolemState
        {
            Move,
            Attack,
            Death
        }
        [Header("Golem Properties")]
        private GolemState _state;

        private void Start()
        {
            _state = GolemState.Move;
        }

        public override void MoveAI(Vector2 playerPosition)
        {
            base.MoveAI(playerPosition);

            _fpsCounter += Time.deltaTime;
            Vector2 direction = (playerPosition - (Vector2)transform.position);
            
            if (_state != GolemState.Death)
            {            
                if (direction.magnitude < 30)
                {
                    if (_state != GolemState.Attack)
                    {
                        _state = GolemState.Attack;
                        _animationStep = 0;
                    }
                }
                else
                {
                    if (_state != GolemState.Move)
                    {
                        _state = GolemState.Move;
                        _animationStep = 0;
                    }
                }
            }
            


            switch (_state)
            {
                case GolemState.Move:
                    // Move anim
                    if (_fpsCounter >= 1 / 10f)
                    {
                        _animationStep++;
                        if (_animationStep == _moveSprites.Length)
                        {
                            _animationStep = 0;
                        }

                        sr.sprite = _moveSprites[_animationStep];
                        _fpsCounter = 0.0f;
                    }

                    if (IsGrounded())
                    {
                        if (Time.time - _jumpIntervalCount > _jumpInterval)
                        {
                            _jumpIntervalCount = Time.time;
                            rb2D.AddForce(new Vector2(direction.normalized.x, 4) * enemyData.moveSpeed);
                        }
                        else
                        {
                            rb2D.velocity = new Vector2(direction.normalized.x * 3, rb2D.velocity.y);
                        }
                    }

                    break;
                case GolemState.Attack:
                    if (_fpsCounter >= 1 / 15f)
                    {
                        _animationStep++;
                        if (_animationStep == _attackSprites.Length)
                        {
                            _animationStep = 0;
                            var bullet = Instantiate(GameDataManager.Instance.GetProjectilePrefab(ProjectileType.GolemBullet), transform.position, Quaternion.identity);
                            bullet.GetComponent<GolemBullet>().SetShootingDirection(direction.normalized);
                            Utilities.RotateObjectTowardDirection2D(direction, bullet.transform, 0f);
                        }

                        sr.sprite = _attackSprites[_animationStep];
                        _fpsCounter = 0.0f;
                    }
                    break;
                case GolemState.Death:
                    if (isDead)
                    {
                        if (_fpsCounter >= 1 / 7f)
                        {
                            _animationStep++;
                            if (_animationStep == _dieSprites.Length)
                            {
                                _animationStep = 0;
                                sr.enabled = false;
                            }

                            sr.sprite = _dieSprites[_animationStep];
                            _fpsCounter = 0.0f;
                        }
                        return;
                    }
                    break;
                default:
                    break;
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


        private void JumpAI(Vector2 playerPosition)
        {
            float xDistance = Mathf.Abs(playerPosition.x - transform.position.x);


            if (playerPosition.y > transform.position.y + 10 && xDistance < 10)
            {
                JumpUp();
                canJump = false;
                Invoke("EnableJump", _jumpDelay);
            }

            if (playerPosition.y < transform.position.y - 10 && xDistance < 10)
            {
                JumpDown();
                canJump = false;
                Invoke("EnableJump", _jumpDelay);
            }
        }

        private void JumpUp()
        {
            rb2D.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
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
            base.OnEnemyDead();
            _state = GolemState.Death;

            _animationStep = 0;
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
