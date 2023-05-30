using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

namespace Sataura
{
    public class GolemBullet : NetworkBehaviour
    {       
        [Header("References")]
        [SerializeField] protected NetworkObject networkObject;
        [SerializeField] protected SpriteRenderer _sr;
        [SerializeField] protected Rigidbody2D _rb;
        public EnemyData _enemyData;

        [Header("GolemBullet Properties")]
        [SerializeField] private bool useGravity;
        // Cached
        private Vector2 _shootingDirection;
        private bool alreadySetShootingDirection = false;
        private float _timeExist = 0.0f;

        public override void OnNetworkSpawn()
        {
            networkObject = GetComponent<NetworkObject>();
        }



        private void FixedUpdate()
        {
            if (!alreadySetShootingDirection) return;

            UseGravity(useGravity);
            _rb.velocity = _shootingDirection * 20;


            _timeExist += Time.fixedDeltaTime;
            if(_timeExist > 4.0f)
            {
                Destroy(this.gameObject);
            }
        }


    
        public void SetShootingDirection(Vector2 shootingDirection)
        {
            _shootingDirection = shootingDirection;
            alreadySetShootingDirection = true;
        }


        private void UseGravity(bool _useGravity)
        {
            this.useGravity = _useGravity;
            if (useGravity)
            {
                _rb.isKinematic = false;
                _rb.gravityScale = 1.0f;
            }
            else
            {
                _rb.isKinematic = true;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            IngamePlayer _player;
            if (collision.TryGetComponent(out _player))
            {
                _player.playerCombat.TakeDamage(_enemyData.damage);
                _player.playerCombat.UpdateHealthBarUI();
                _player.playerCombat.PlayBloodTearVFX();

                Destroy(this.gameObject);
            }  
        }

        public int GetDamage()
        {
            return _enemyData.damage;
        }

        public float GetKnockback()
        {
            return 0;
        }
    }
}
