using JetBrains.Annotations;
using System.Collections;
using Unity.Netcode;
using UnityEngine;


namespace Sataura
{
    public class NormalArrowProjectile : NetworkProjectile, ICanCauseDamage
    {
        [Header("REFERENCES")]
        [SerializeField] private GameObject explosionObjectPrefab;
        [SerializeField] private Transform explosionSpawnPosition;


        [Header("Properties")]
        [SerializeField] private LayerMask groundLayer;


        // Cached variables
        private BoxCollider2D arrowCollider;
        [SerializeField] private BowData currentBowData;


        public override void OnNetworkSpawn()
        {
            arrowCollider = GetComponent<BoxCollider2D>();

            StartCoroutine(Despawn(7f));
        }


        /// <summary>
        /// Shoots the arrow with given bow and arrow data.
        /// </summary>
        /// <param name="bowData">The bow data to use for the shot.</param>
        /// <param name="arrowData">The arrow data to use for the shot.</param>
        public void Shoot(BowData bowData, ArrowData arrowData)
        {
            this.currentBowData = bowData;
            SetDustServerRpc(arrowData.particle);

            if (rb == null)
                rb = GetComponent<Rigidbody2D>();

            rb.velocity = Quaternion.Euler(0, 0, 45) * transform.right * bowData.releaseSpeed;
        }


        private void Update()
        {
            if (!IsServer) return;

            Vector2 direction = rb.velocity;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, -45) * Quaternion.AngleAxis(angle, Vector3.forward);
        }




        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!IsServer) return;
         
            if (groundLayer == (groundLayer | (1 << collision.gameObject.layer)))
            {
                // Collision detected with a game object that is in the canHookLayers LayerMask
                // You can add your desired code or logic here
                ArrowPropertiesWhenCollide();
                StartCoroutine(Despawn(1f));

                SoundManager.Instance.PlaySound(SoundType.ArrowProjectileHitGround, playRandom: true, collision.transform.position);
            }
        }

    
        private IEnumerator Despawn(float time)
        {
            yield return new WaitForSeconds(time);
            if (_networkObject.IsSpawned)
                _networkObject.Despawn();
        }




        /// <summary>
        /// Returns the arrow projectile to the pool.
        /// </summary>
        private void ReturnToNetworkPool()
        {
            if(_networkObject.IsSpawned)
                _networkObject.Despawn();       
        }

  

        /// <summary>
        /// Sets the arrow properties when it collides with something.
        /// </summary>
        private void ArrowPropertiesWhenCollide()
        {
            rb.isKinematic = true;
            spriteRenderer.enabled = false;
            arrowCollider.enabled = false;
            rb.velocity = Vector2.zero;           
        }


        public int GetDamage()
        {
            ArrowPropertiesWhenCollide();
            var returnDamage = currentBowData.baseAttackDamage;
            
            if (_networkObject.IsSpawned)
                _networkObject.Despawn();

            return returnDamage;
        }

        public float GetKnockback()
        {
            return 5.0f;
        }
    }
}
