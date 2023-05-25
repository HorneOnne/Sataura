using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Sataura
{
    public class BlackHoleProjectile : NetworkBehaviour
    {
        [Header("References")]
        public NetworkObject _networkObject;
        [SerializeField] private BlackHoldVFX _blackHoldVFX;

        [Header("Runtime References")]
        [SerializeField] WhisperingGaleData _whisperingGaleData = null;


        [SerializeField] private Collider2D[] enemies = new Collider2D[7]; // Array to store results of the overlap check
        [SerializeField] private LayerMask enemyLayer;
        private float _blackHoleRadius = 10f;
        private float _blackHoleGravity = -10f;
        private float lastDetectionTime = 0f;
        private int _numOfEnemies = 0;

        public void SetData(WhisperingGaleData data)
        {
            _whisperingGaleData = data;
            _blackHoldVFX.ChangeStartSize(1f);
        }


        public override void OnNetworkSpawn()
        {
            StartCoroutine(Despawn());
        }

        private void Update()
        {
            if (Time.time - lastDetectionTime > 0.5f)
            {
                lastDetectionTime = Time.time;
                _numOfEnemies = Physics2D.OverlapCircleNonAlloc(transform.position, _blackHoleRadius, enemies, enemyLayer);
            }
        }

        private void FixedUpdate()
        {
            if (_numOfEnemies > 0)
            {
                for (int i = 0; i < _numOfEnemies; i++)
                {
                    if(enemies[i] != null)
                    {
                        BaseEnemy _enemyInZone;
                        if (enemies[i].TryGetComponent(out _enemyInZone))
                        {
                            if (_enemyInZone != null)
                            {
                                var direction = _enemyInZone.transform.position - transform.position;
                                _enemyInZone.blackHole = true;
                                _enemyInZone.Rb2D.velocity = direction.normalized * _blackHoleGravity;
                            }
                        }
                    }                  
                }
            }
        }


        private IEnumerator Despawn()
        {
            yield return new WaitUntil(() => _whisperingGaleData != null);
            yield return new WaitForSeconds(_whisperingGaleData.timeExist * 80/100f);
            _blackHoldVFX.StopVFX();
            yield return new WaitForSeconds(_whisperingGaleData.timeExist);
            if (_networkObject.IsSpawned)
                _networkObject.Despawn();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.black;
            Gizmos.DrawWireSphere(transform.position, _blackHoleRadius);
        }
    }
}

