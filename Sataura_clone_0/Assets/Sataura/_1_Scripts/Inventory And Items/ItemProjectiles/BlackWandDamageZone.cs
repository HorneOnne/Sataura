using Unity.Netcode;
using UnityEngine;
using System.Collections;

namespace Sataura
{
    public class BlackWandDamageZone : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private NetworkObject _networkObject;
        [SerializeField] private ParticleSystem _ps;

        [Header("Runtime References")]
        [SerializeField] LightningStaffData _lightningStaffData = null;

        [Header("BlackWandDamageZone Properties")]
        [SerializeField] private Collider2D[] enemies = new Collider2D[7]; // Array to store results of the overlap check
        [SerializeField] private LayerMask enemyLayer;
        private float lastDetectionTime = 0f;
  

        #region Properties
        public NetworkObject NetworkObj { get { return _networkObject; } }
        #endregion


        public override void OnNetworkSpawn()
        {
            StartCoroutine(Despawn());
        }


        private IEnumerator Despawn()
        {
            yield return new WaitForSeconds(3.5f);
            if (_networkObject.IsSpawned)
                _networkObject.Despawn();
        }


        public void SetData(LightningStaffData lightningStaffData)
        {
            this._lightningStaffData = lightningStaffData;
            if(_ps.isPlaying == false)
            {
                _ps.Play();
            }
        }

        private void Update()
        {
            if (_lightningStaffData == null) return;


            if (Time.time - lastDetectionTime > 0.5f)
            {
                lastDetectionTime = Time.time;
                int numEnemies = Physics2D.OverlapCircleNonAlloc(transform.position, transform.localScale.x, enemies, enemyLayer);
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

