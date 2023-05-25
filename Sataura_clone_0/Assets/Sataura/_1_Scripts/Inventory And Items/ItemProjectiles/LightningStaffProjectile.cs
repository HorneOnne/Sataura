using Unity.Netcode;
using System.Collections;
using UnityEngine;

namespace Sataura
{
    public class LightningStaffProjectile : NetworkBehaviour, ICanCauseDamage
    {
        [Header("References")]
        public NetworkObject _networkObject;
        [SerializeField] private LineRenderer _lr;
        [SerializeField] private Texture[] _textures;
        [SerializeField] private GameObject _lightnigBulletPrefab;

        [Header("Runtime References")]
        [SerializeField] LightningStaffData _lightningStaffData = null;



        // Cached
        private int _animationStep;
        private float _fps = 15f;
        private float _fpsCounter = 0f;
        private GameObject _lightningBulletObject;
        private ParticleSystem _psLightningBullet;
        private Vector2 _targetPosition;


        [SerializeField] private Collider2D[] enemies = new Collider2D[3]; // Array to store results of the overlap check
        [SerializeField] private LayerMask enemyLayer;
        private float _lightingDamgedZone = 3f;
        private float lastDetectionTime = 0f;

        public override void OnNetworkSpawn()
        {
            _lr = GetComponent<LineRenderer>();
            transform.position = Vector2.zero;
            StartCoroutine(Despawn());
        }


        public void SetData(LightningStaffData data, Vector2 enemyPosition, float areaSize)
        {
            _targetPosition = enemyPosition;
            _lightningStaffData = data;
            _lightningBulletObject = Instantiate(_lightnigBulletPrefab);
            _lr.startWidth *= areaSize;
            _lr.endWidth *= areaSize;

            _psLightningBullet = _lightningBulletObject.GetComponent<ParticleSystem>(); 
            _lightningBulletObject.transform.position = _targetPosition + new Vector2(0f, 35f);

            _lr.positionCount = 2;
            _lr.SetPosition(0, _lightningBulletObject.transform.position);
            _lr.SetPosition(1, _lightningBulletObject.transform.position);
        }


        private void Update()
        {
            if (_lightningBulletObject == null) return;

            // Animation
            _fpsCounter += Time.deltaTime;
            if (_fpsCounter >= 1 / _fps)
            {
                _animationStep++;
                if (_animationStep == _textures.Length)
                {
                    _animationStep = 0;
                }

                _lr.material.SetTexture("_MainTex", _textures[_animationStep]);
                _fpsCounter = 0.0f;
            }
            // =========



            if (Vector2.Distance(_lightningBulletObject.transform.position, _targetPosition) < 0.1f)
            {
                if(_psLightningBullet.isPlaying == false)
                {
                    _lightningBulletObject.GetComponent<ParticleSystem>().Play();
                    StartCoroutine(Despawn(0.2f));
                }
                
                if (Time.time - lastDetectionTime > _lightningStaffData.attackCycle)
                {
                    lastDetectionTime = Time.time;
                    int numEnemies = Physics2D.OverlapCircleNonAlloc(_lightningBulletObject.transform.position, _lightningStaffData.lightingDamgedZone, enemies, enemyLayer);
                    if (numEnemies > 0)
                    {
                        for (int i = 0; i < numEnemies; i++)
                        {
                            BaseEnemy _enemyInZone;
                            if (enemies[i].TryGetComponent(out _enemyInZone))
                            {
                                _enemyInZone.GetLightningDamaged(GetDamage());
                            }
                        }
                    }
                }
                
            }
            else
            {
                _lr.SetPosition(1, _lightningBulletObject.transform.position);
                _lightningBulletObject.transform.position = Vector2.MoveTowards(_lightningBulletObject.transform.position, _targetPosition, 90f * Time.deltaTime);
            }                  
        }

        private IEnumerator Despawn()
        {
            yield return new WaitForSeconds(_lightningStaffData.timeExist);
            Destroy(_lightningBulletObject);
            if (_networkObject.IsSpawned)
                _networkObject.Despawn();
        }

        private IEnumerator Despawn(float time)
        {
            yield return new WaitForSeconds(time);
            Destroy(_lightningBulletObject);
            if (_networkObject.IsSpawned)
                _networkObject.Despawn();
        }

        public int GetDamage()
        {
            return _lightningStaffData.damage;
        }

        public float GetKnockback()
        {
            return 0;
        }
    }
}

