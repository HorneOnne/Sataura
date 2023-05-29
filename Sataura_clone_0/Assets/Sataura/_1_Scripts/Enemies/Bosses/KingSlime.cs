using Unity.Netcode;
using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;

namespace Sataura
{
    public class KingSlime : GroundEnemy
    {
        [Header("===== KINGSLIME =====")]
        [Space(20)]
        [Header("References")]
        [SerializeField] private Animator _anim;
        [SerializeField] private GhostEffect _ghostEffect;


        [Header("Jump")]
        [SerializeField] private float _jumpForce = 3f;
        private float _jumpDelay = 2.0f; // the delay in seconds before the player can jump again
        [SerializeField] private bool canJump = true; // a flag indicating whether the player can jump



        // Jump movement
        private float _jumpInterval = 3.0f;
        private float _jumpIntervalCount = 0.0f;



        private enum KingSlimeState
        {
            Move,
            Attack,
            Death,
            Fall,
            BeforeTeleport,
            AfterTeleport,
            GlancePlayer,        // For Phase 3 AI
            CrazyStand,          // For Phase 3 AI
        }
        private enum KingSlimeAttackPhase
        {
            Phase1, Phase2, Phase3
        }

        [Header("KingSlime Properties")]
        [SerializeField] private KingSlimeState _state;
        [SerializeField] private KingSlimeAttackPhase _attackPhase;


        [Header("Combat")]
        // Phase 1
        [SerializeField] private bool _canAttackJumpDown = false;
        [SerializeField] private Vector2 _targetAttackPosition;

        // Phase 2
        private GameObject _greenPortalVFXPrefab;
        private float limitYPosition = 10;
        private float _appearTeleportRadiusAroundPlayer = 10f;

        // Phase 3
        [SerializeField] private Vector2 _targetAttackPositionPhase3;
        private float _phaseThreeFlySpeed = 50f;
        private float _crazyStandTime = 1.5f;


        private void Start()
        {
            _state = KingSlimeState.Move;
            _attackPhase = KingSlimeAttackPhase.Phase1;

            _greenPortalVFXPrefab = GameDataManager.Instance.greenPortalVFX.gameObject;
        }

        float tempTimeCounter = 0.0f;
        private void Update()
        {
            // Update animation
            _anim.SetBool("isGround", IsGrounded());

            if (rb2D.velocity.y > 5.0f)
            {

                _anim.SetBool("isJump", true);
            }
            else
            {
                _anim.SetBool("isJump", false);
            }

     
            if(Time.time - tempTimeCounter > 7.0f)
            {
                tempTimeCounter = Time.time;


                if (CurrentHealth <= MaxHealth * 40 / 100f)
                {
                    _state = KingSlimeState.Attack;
                    _attackPhase = KingSlimeAttackPhase.Phase3;
                }
                else if (CurrentHealth <= MaxHealth * 70 / 100f)
                {
                    if (Random.Range(0f, 1f) < 0.5f)
                    {
                        _attackPhase = KingSlimeAttackPhase.Phase1;
                    }
                    else if (Random.Range(0f, 1f) < 0.7f)
                    {
                        _state = KingSlimeState.Attack;
                        _attackPhase = KingSlimeAttackPhase.Phase2;
                    }
                    else
                    {
                        _state = KingSlimeState.Attack;
                        _attackPhase = KingSlimeAttackPhase.Phase3;
                    }
                }
                else if (CurrentHealth <= MaxHealth * 90 / 100f)
                {
                    if (Random.Range(0f, 1f) < 0.5f)
                    {
                        _attackPhase = KingSlimeAttackPhase.Phase1;
                    }
                    else
                    {
                        _state = KingSlimeState.Attack;
                        _attackPhase = KingSlimeAttackPhase.Phase2;
                    }
                }
                else
                {
                 
                    if (Random.Range(0f, 1f) < 0.5f)
                    {
                        _state = KingSlimeState.Move;
                    }
                    else
                    {
                        _state = KingSlimeState.Attack;
                        _attackPhase = KingSlimeAttackPhase.Phase1;
                    }
                }
            }

            /*if (Input.GetKeyDown(KeyCode.L))
            {
                _state = KingSlimeState.Attack;
                _attackPhase = KingSlimeAttackPhase.Phase3;
            }*/
        }

      

        protected override void FixedUpdate()
        {
            if (!IsServer) return;
            MoveAI(playerTranform.position);
        }



        public override void MoveAI(Vector2 playerPosition)
        {
            Vector2 direction = (playerPosition - (Vector2)transform.position);


            switch (_state)
            {
                case KingSlimeState.Move:

                    if (IsGrounded())
                    {
                        if (Time.time - _jumpIntervalCount > _jumpInterval)
                        {
                            _jumpIntervalCount = Time.time;
                            rb2D.AddForce(new Vector2(direction.normalized.x, 2f) * enemyData.moveSpeed, ForceMode2D.Impulse);
                        }
                        else
                        {
                            rb2D.velocity = new Vector2(direction.normalized.x * enemyData.moveSpeed, rb2D.velocity.y);
                        }
                    }
                    break;
                case KingSlimeState.Attack:
                    switch (_attackPhase)
                    {
                        case KingSlimeAttackPhase.Phase1:
                            Phase1();
                            break;
                        case KingSlimeAttackPhase.Phase2:
                            Phase2();
                            break;
                        case KingSlimeAttackPhase.Phase3:
                            Phase3();
                            break;
                        default:
                            break;
                    }
                    break;
                case KingSlimeState.Fall:
                    if (IsGrounded() == false)
                    {
                        rb2D.AddForce(Vector2.down * 5f, ForceMode2D.Impulse);
                    }
                    else
                    {
                        StartCoroutine(WaitAfter(1.0f, () =>
                        {
                            if (_canAttackJumpDown == true)
                                _canAttackJumpDown = false;

                            _state = KingSlimeState.Move;
                        }));
                    }
                    break;
                case KingSlimeState.BeforeTeleport:

                    break;
                case KingSlimeState.AfterTeleport:
                    StartCoroutine(WaitAfter(1f, () =>
                    {
                        Vector2 aroundPlayerPosition = playerPosition + (Random.insideUnitCircle.normalized * _appearTeleportRadiusAroundPlayer);
                        if (aroundPlayerPosition.y < limitYPosition)
                        {
                            aroundPlayerPosition = new Vector2(aroundPlayerPosition.x, limitYPosition);
                        }

                        transform.position = aroundPlayerPosition;
                        var _greenPortal = CreateGreenPortal(aroundPlayerPosition, new Vector3(0.1f, 0.1f, 1f));
                        _greenPortal.ScaleOverTime(new Vector3(10f, 10f, 1), 1.0f);
                        SettingsAppearAfterTeleport(enableSr: true, false, false);

                        StartCoroutine(WaitAfter(0.5f, () =>
                        {
                            var currentLocalScale = transform.localScale;
                            ScaleOverTime(currentLocalScale, new Vector3(17f, 17f, 1f), 1.0f);

                            StartCoroutine(WaitAfter(1.0f, () =>
                            {
                                _greenPortal.StopVFX();
                                SettingsAppearAfterTeleport(enableSr: true, enableCollider: true, enableKinematic: true);
                                _state = KingSlimeState.Move;
                            }));
                        }));
                    }));

                    _state = KingSlimeState.BeforeTeleport;
                    break;
                case KingSlimeState.GlancePlayer:
                    _targetAttackPositionPhase3 = GetOppositePosition(transform.position, playerTranform.position);
                    _state = KingSlimeState.CrazyStand;
                    break;
                case KingSlimeState.CrazyStand:
                    direction = _targetAttackPositionPhase3 - (Vector2)transform.position;
                    rb2D.MovePosition((Vector2)transform.position + direction.normalized * _phaseThreeFlySpeed * Time.fixedDeltaTime);

                    // Ghost effect
                    _ghostEffect.isGhosting = true;

                    StartCoroutine(WaitAfter(_crazyStandTime, () =>
                    {                       
                        _state = KingSlimeState.Attack;
                        _attackPhase = KingSlimeAttackPhase.Phase1;

                        // Ghost effect
                        _ghostEffect.isGhosting = false;
                    }));         
                    break;
                case KingSlimeState.Death:
                    
                    break;
                default:
                    break;
            }



            // Limit fall velocity
            if (rb2D.velocity.y > 40)
                rb2D.velocity = new Vector2(rb2D.velocity.x, 40);
        }


        private void ResetAttackState()
        {
            if (_canAttackJumpDown == false)
                _canAttackJumpDown = true;
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



        private void Phase1()
        {
            if (_canAttackJumpDown == false)
            {
                if (rb2D.isKinematic == false)
                    rb2D.isKinematic = true;


                Vector2 direction = ((Vector2)playerTranform.position + new Vector2(0, 20f)) - (Vector2)transform.position;


                // Attack jump down
                if (Mathf.Abs(direction.x) < 2f)
                {
                    rb2D.velocity = Vector2.zero;
                    _targetAttackPosition = playerTranform.position;
                    StartCoroutine(WaitAfter(0.3f, () =>
                    {
                        _canAttackJumpDown = true;
                    }));
                }
                else
                {
                    rb2D.MovePosition((Vector2)transform.position + direction.normalized * 60f * Time.fixedDeltaTime);

                    // Ghost effect;
                    _ghostEffect.isGhosting = true;
                }
            }
            else
            {
                MoveToTarget(transform.position, _targetAttackPosition + new Vector2(0, 3));

            }
        }




        private void Phase2()
        {
            _state = KingSlimeState.BeforeTeleport;
            var _greenPortalObject = CreateGreenPortal(transform.position, new Vector3(10, 10, 1), this.transform);

            StartCoroutine(WaitAfter(0.5f, () =>
            {
                _greenPortalObject.ScaleOverTime(new Vector3(0.2f, 0.2f, 1), 2.0f);
                var currentLocalScale = transform.localScale;
                ScaleOverTime(currentLocalScale, new Vector3(0.2f, 0.2f, 1f), 2.0f);
            }));

            StartCoroutine(WaitAfter(2.5f, () =>
            {
                Destroy(_greenPortalObject.gameObject);
                _state = KingSlimeState.AfterTeleport;
                SettingsHideWhenTeleport();
            }));
        }



        private void Phase3()
        {
            if (rb2D.isKinematic == false)
                rb2D.isKinematic = true;
            _state = KingSlimeState.GlancePlayer;           
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

            _state = KingSlimeState.Death;
            rb2D.velocity = Vector2.zero;
            OnKingSimeDead();
        }



        protected override void Despawn()
        {
            if (networkObject.IsSpawned)
                networkObject.Despawn();
        }


        private IEnumerator WaitAfter(float time, System.Action callback)
        {
            yield return new WaitForSeconds(time);
            callback?.Invoke();
        }


        private void MoveToTarget(Vector2 initialPosition, Vector2 target)
        {
            float step = 60 * Time.fixedDeltaTime;
            rb2D.MovePosition(Vector2.MoveTowards(rb2D.position, target, step));

            if (Vector2.Distance(rb2D.position, target) <= step)
            {
                rb2D.isKinematic = false;
                _state = KingSlimeState.Fall;
                _canAttackJumpDown = false;

                // Ghost effect;
                _ghostEffect.isGhosting = false;
            }
        }


        public void ScaleOverTime(Vector3 initialScale, Vector3 targetScale, float scaleDuration)
        {
            StartCoroutine(ScaleCoroutine(initialScale, targetScale, scaleDuration));
        }

        private IEnumerator ScaleCoroutine(Vector3 initialScale, Vector3 targetScale, float scaleDuration)
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


        private GreenPortalVFX CreateGreenPortal(Vector2 spawnPosition, Vector3 scaleVector, Transform parent = null)
        {
            var _greenPortalObject = Instantiate(_greenPortalVFXPrefab, spawnPosition, Quaternion.identity).GetComponent<GreenPortalVFX>();
            _greenPortalObject.transform.localScale = scaleVector;

            if (parent != null)
            {
                _greenPortalObject.transform.SetParent(this.transform);
            }


            return _greenPortalObject;
        }

        private void SettingsHideWhenTeleport()
        {
            sr.enabled = false;
            rb2D.velocity = Vector2.zero;
            boxCollider2D.enabled = false;
            rb2D.isKinematic = true;
        }

        private void SettingsAppearAfterTeleport(bool enableSr, bool enableCollider, bool enableKinematic)
        {
            if (enableSr)
                sr.enabled = true;

            if (enableCollider)
                boxCollider2D.enabled = true;

            if (enableKinematic)
                rb2D.isKinematic = false;
        }

        private Vector2 GetOppositePosition(Vector2 point, Vector2 specificPoint)
        {
            Vector2 direction = point - specificPoint;
            Vector2 oppositeDirection = -direction;
            Vector2 oppositePosition = specificPoint + oppositeDirection;
            return oppositePosition;
        }

        private void OnKingSimeDead()
        {
            Debug.Log("king Slime dead.");

            EnemySpawnWaves.currentWaveIndex = 100;
            UIManager.Instance.upgradeItemSkillPanel.SetActive(false);
            UIManager.Instance.bossHealthBarCanvas.enabled = false;
            IngameInformationManager.Instance.SetVictoryState();          
        }
    }

}

