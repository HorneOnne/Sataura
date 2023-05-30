using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Sataura
{
    public class BookOfWindProjectile : NetworkBehaviour
    {
        [Header("References")]
        public NetworkObject _networkObject;
        [SerializeField] private SnowSlashVFX _snowSlashVFX;
        [SerializeField] private Rigidbody2D _rb;
        [SerializeField] private CircleCollider2D _circileCollider2D;

        [Header("Runtime References")]
        [SerializeField] WhisperingGaleData _whisperingGaleData = null;


        // Cached
        private Vector2 _moveDirection;
        private float _moveTime = 0.0f;
        private float _timeExist = 0.0f;

        public void SetData(WhisperingGaleData data, Vector2 moveDirection)
        {
            _whisperingGaleData = data;
            _moveDirection = moveDirection;
            _moveTime = Random.Range(0.5f, 2f);

            // Set ParticleSystem properties
            _snowSlashVFX.ChangeSnowSlashVFXStartLifetime(_whisperingGaleData.timeExist);
            // =============================
        }


        public override void OnNetworkSpawn()
        {
            StartCoroutine(Despawn());
        }


        private void FixedUpdate()
        {
            _timeExist += Time.fixedDeltaTime;
            if(_timeExist < _moveTime)
            {
                _rb.MovePosition(_rb.position + _moveDirection * 10 * Time.fixedDeltaTime);
            }          
        }


        private IEnumerator Despawn()
        {
            yield return new WaitUntil(() => _whisperingGaleData != null);
            yield return new WaitForSeconds(_whisperingGaleData.timeExist * 80f / 100f);
            _circileCollider2D.enabled = false;
            yield return new WaitForSeconds(_whisperingGaleData.timeExist);
            if (_networkObject.IsSpawned)
                _networkObject.Despawn();
        }
      
        public int GetDamage()
        {
            return _whisperingGaleData.damage;
        }

        public float GetKnockback()
        {
            return 0;
        }
    }
}

