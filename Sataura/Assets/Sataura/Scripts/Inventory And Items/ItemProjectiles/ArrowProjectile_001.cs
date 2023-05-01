using System.Collections;
using Unity.Netcode;
using UnityEngine;


namespace Sataura
{
    /// <summary>
    /// Represents an arrow projectile that can cause damage on collision.
    /// </summary>
    [RequireComponent(typeof(BoxCollider2D))]
    public class ArrowProjectile_001 : NetworkProjectile, ICanCauseDamage
    {
        [Header("REFERENCES")]
        [SerializeField] private GameObject explosionObjectPrefab;
        [SerializeField] private Transform explosionSpawnPosition;


        [Header("Properties")]
        [SerializeField] private LayerMask groundLayer;


        // Cached variables
        private BoxCollider2D arrowCollider;
        private float timeElapsedSinceShot = 0.0f;
        private const float TIME_TO_RETURN = 3.0f;
        private const float TIME_TO_RETURN_WHEN_COLLIDE = 2.0f;
        private WaitForSeconds waitForReturnOnCollision;
        private BowData currentBowData;
        private ArrowData currentArrowData;


        public override void OnNetworkSpawn()
        {
            arrowCollider = GetComponent<BoxCollider2D>();
            waitForReturnOnCollision = new WaitForSeconds(TIME_TO_RETURN_WHEN_COLLIDE);
        }


        /// <summary>
        /// Shoots the arrow with given bow and arrow data.
        /// </summary>
        /// <param name="bowData">The bow data to use for the shot.</param>
        /// <param name="arrowData">The arrow data to use for the shot.</param>
        public void Shoot(BowData bowData, ArrowData arrowData)
        {
            timeElapsedSinceShot = 0.0f;
            this.currentBowData = bowData;
            this.currentArrowData = arrowData;
            //SetDust(arrowData.particle);
            SetDustServerRpc(arrowData.particle);

            if (rb == null)
                rb = GetComponent<Rigidbody2D>();

            rb.velocity = Quaternion.Euler(0, 0, 45) * transform.right * bowData.releaseSpeed;
        }


        private void Update()
        {
            if (!IsServer) return;

            timeElapsedSinceShot += Time.deltaTime;
            if (timeElapsedSinceShot > TIME_TO_RETURN)
            {
                ResetArrowPropertiesServerRpc();
                ReturnToNetworkPoolServerRpc();
            }

            Vector2 direction = rb.velocity;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, -45) * Quaternion.AngleAxis(angle, Vector3.forward);
        }


        private void OnCollisionEnter2D(Collision2D collision)
        {
            ArrowPropertiesWhenCollideServerRpc();
            StartCoroutine(PerformReturnToPool());
     
            
        }


        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!IsServer) return;

            ArrowPropertiesWhenCollideServerRpc();
            StartCoroutine(PerformReturnToPool());

            if (groundLayer == (groundLayer | (1 << collision.gameObject.layer)))
            {
                // Collision detected with a game object that is in the canHookLayers LayerMask
                // You can add your desired code or logic here

                SoundManager.Instance.PlaySound(SoundType.ArrowProjectileHitGround, playRandom: true, collision.transform.position);
            }
        }

        /// <summary>
        /// Performs the return of the arrow projectile to the pool after a delay.
        /// </summary>
        IEnumerator PerformReturnToPool()
        {
            yield return waitForReturnOnCollision;
            ResetArrowPropertiesServerRpc();
            ReturnToNetworkPoolServerRpc();
        }




        /// <summary>
        /// Returns the arrow projectile to the pool.
        /// </summary>
        private void ReturnToNetworkPool()
        {
            //ResetArrowProperties();

            if(networkObject.IsSpawned)
                networkObject.Despawn(false);

            var projectilePrefab = GameDataManager.Instance.GetProjectilePrefab("PP_ArrowProjectile_001");
            NetworkObjectPool.Singleton.ReturnNetworkObject(networkObject, projectilePrefab);
            
        }

        [ServerRpc]
        private void ReturnToNetworkPoolServerRpc()
        {
            ReturnToNetworkPool();

            ReturnToNetworkPoolClientRpc();  
        }

        [ClientRpc]
        private void ReturnToNetworkPoolClientRpc()
        {
            if (IsServer) return;
            ReturnToNetworkPool();
        }



        /// <summary>
        /// Resets the arrow properties to default.
        /// </summary>
        private void ResetArrowProperties()
        {
            rb.isKinematic = false;
            spriteRenderer.enabled = true;
            rb.velocity = Vector2.zero;
            arrowCollider.enabled = true;
        }

        [ServerRpc]
        private void ResetArrowPropertiesServerRpc()
        {
            ResetArrowProperties();
            ResetArrowPropertiesClientRpc();
        }

        [ClientRpc]
        private void ResetArrowPropertiesClientRpc()
        {
            if (IsServer) return;
            ResetArrowProperties();
        }



        /// <summary>
        /// Sets the arrow properties when it collides with something.
        /// </summary>
        private void ArrowPropertiesWhenCollide()
        {
            rb.isKinematic = true;
            spriteRenderer.enabled = false;
            rb.velocity = Vector2.zero;
            arrowCollider.enabled = false;
        }

        [ServerRpc]
        private void ArrowPropertiesWhenCollideServerRpc()
        {
            ArrowPropertiesWhenCollide();
            ArrowPropertiesWhenCollideClientRpc();
        }

        [ClientRpc]
        private void ArrowPropertiesWhenCollideClientRpc()
        {
            if (IsServer) return;
            ArrowPropertiesWhenCollide();
        }


        public int GetDamage()
        {
            return currentArrowData.damage + currentBowData.baseAttackDamage;
        }

        public float GetKnockback()
        {
            return 0.0f;
        }
    }
}
