using System.Collections.Generic;
using UnityEngine;

namespace Sataura
{
    public class PurpleSkull : FlyingEnemy
    {
        [SerializeField] private Sprite[] flySprites;
        [SerializeField] private Sprite[] dieSprites;

        private float fps = 30f;
        private float fpsCounter = 0f;
        private int animationStep;

        private enum PurplekullState
        {
            Move,
            Attack,
            Die
        }

        private PurplekullState _state;


        [Header("Purple skull properties")]
        [SerializeField] private List<Sprite> _effectTextures;
        [SerializeField] private LineRenderer _lr;
        [SerializeField] private Texture[] textures;
        private int _effectAnimationStep;
        private float effectFpsCounter = 0f;
        // Cached
        private Player _playerCached;
        private static Debuff _slowlyDebuff;



        public override void SetFollowTarget(Transform target)
        {
            base.SetFollowTarget(target);

            if (_playerCached == null)
                _playerCached = base.playerTranform.GetComponent<Player>();

            if(_slowlyDebuff == null)
            {
                _slowlyDebuff = Instantiate(GameDataManager.Instance.GetDebuffEffectVFXPrefabs(DebuffEffect.Slowly));
            }
        }


        private void Start()
        {
            _state = PurplekullState.Attack;
        }


        private void Update()
        {
            if (_state != PurplekullState.Attack) return;

            effectFpsCounter += Time.deltaTime;
            if (effectFpsCounter >= 1 / 15f)
            {
                _effectAnimationStep++;
                if (_effectAnimationStep == textures.Length)
                {
                    _effectAnimationStep = 0;
                }

                _lr.material.SetTexture("_MainTex", textures[_effectAnimationStep]);

                effectFpsCounter = 0.0f;
            }
        }

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

            if (_state == PurplekullState.Die) return;
            Vector2 direction = target - (Vector2)transform.position;
            if (direction.magnitude < 30)
            {
                if (_state != PurplekullState.Attack)
                {
                    _state = PurplekullState.Attack;
                }
            }
            else if(direction.magnitude > 40)
            {
                if (_state != PurplekullState.Move)
                {
                    _state = PurplekullState.Move;
                }
            }

            switch (_state)
            {
                case PurplekullState.Move:
                    rb2D.MovePosition((Vector2)transform.position + direction.normalized * enemyData.moveSpeed * Time.fixedDeltaTime);
                    if (direction.x > 0)
                    {
                        if (transform.localScale.x < 0)
                            Flip();
                    }
                    else
                    {
                        if (transform.localScale.x > 0)
                            Flip();
                    }
                    StopCausingSlowlyEffect();
                    break;
                case PurplekullState.Attack:
                    if (direction.x > 0)
                    {
                        if (transform.localScale.x < 0)
                            Flip();
                    }
                    else
                    {
                        if (transform.localScale.x > 0)
                            Flip();
                    }
                    CausingSlowlyEffect();
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

            _state = PurplekullState.Die;
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


        // PurpleSkull methods
        private void CausingSlowlyEffect()
        {
            // Line renderer
            _lr.positionCount = 2;
            _lr.SetPosition(0, transform.position);
            _lr.SetPosition(1, base.playerTranform.position);
            // -------------

            // Slow player
            _playerCached.playerMovement.LimitVelocity(5f);
            // ---------------------


            // Play slow effect VFX
            if(_slowlyDebuff._Ps.isPlaying == false)
            {
                _slowlyDebuff._Ps.Play(true);               
            }
            _slowlyDebuff.transform.position = base.playerTranform.position;
            // --------------------
        }

        private void StopCausingSlowlyEffect()
        {
            _lr.positionCount = 0;

            if (_slowlyDebuff._Ps.isPlaying == true || _slowlyDebuff._Ps.isEmitting == true)
            {
                _slowlyDebuff._Ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
        }
    }

}
