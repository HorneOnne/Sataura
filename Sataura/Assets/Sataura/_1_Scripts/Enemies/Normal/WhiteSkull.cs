using UnityEngine;

namespace Sataura
{
    public class WhiteSkull : FlyingEnemy
    {
        [SerializeField] private Sprite[] flySprites;
        [SerializeField] private Sprite[] dieSprites;

        private float fps = 30f;
        private float fpsCounter = 0f;
        private int animationStep;

        private enum WhiteSkullState
        {
            Move,
            Attack,
            Die
        }

        private WhiteSkullState _state;
        private Vector2 _attackDirection;

        private void Start()
        {
            _state = WhiteSkullState.Move;
        }

        public override void MoveAI(Vector2 target)
        {
            base.MoveAI(target);
            if (blackHole)
                return;

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


            if (_state == WhiteSkullState.Die) return;
            Vector2 direction = target - (Vector2)transform.position;
            if (direction.magnitude < 30)
            {
                if(_state != WhiteSkullState.Attack)
                {
                    _state = WhiteSkullState.Attack;
                    _attackDirection = direction;
                }              
            }
            else
            {
                if (_state != WhiteSkullState.Move)
                {
                    _state = WhiteSkullState.Move;
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                    Flip();
                }                
            }

            switch (_state)
            {
                case WhiteSkullState.Move:
                    rb2D.MovePosition((Vector2)transform.position + direction.normalized * enemyData.moveSpeed * Time.fixedDeltaTime);
                    break;
                case WhiteSkullState.Attack:
                    if (_attackDirection.x > 0)
                    {
                        transform.Rotate(new Vector3(0, 0, -180) * Time.fixedDeltaTime);

                        // Flip
                        if (transform.localScale.x < 0)
                            Flip();
                    }
                    else
                    {
                        transform.Rotate(new Vector3(0, 0, 180) * Time.fixedDeltaTime);

                        // Flip
                        if (transform.localScale.x > 0)
                            Flip();
                    }

                    rb2D.MovePosition((Vector2)transform.position + _attackDirection.normalized * enemyData.moveSpeed * 1.5f * Time.fixedDeltaTime);
                    break;
                default:
                    break;
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

            _state = WhiteSkullState.Die;
            animationStep = 0;
            rb2D.velocity = Vector2.zero;
            SoundManager.Instance.PlaySound(SoundType.EnemyDie, enemyData.dieSFX);
        }




        private void Flip()
        {
            if (transform.localScale.x < 0)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }

        }
    }
}
