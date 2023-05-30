using JetBrains.Annotations;
using Mono.Cecil;
using System.Collections;
using Unity.Netcode;
using UnityEngine;


namespace Sataura
{
    public class NormalArrowProjectile : SingleTargetPhysicalProjectile
    {
        [Space(10)]
        [Header("===== NormalArrowProjectile =====")]
        [Space(3)]
        [SerializeField] private LayerMask groundLayer;

        // Cached
        private BowData _bowData;

        public override void OnNetworkSpawn()
        {
            NetworkDespawnAfter(7.0f);
        }


        public override void Fire(IngamePlayer fromPlayer, Vector2 targetPosition, WeaponData weaponData, bool updateProjectileSize = true)
        {
            base.Fire(fromPlayer, targetPosition, weaponData);
            
            _bowData = (BowData)weaponData;
            LogicalProjectile();
        }

        private void LogicalProjectile()
        {
            rb2D.velocity = Quaternion.Euler(0, 0, 45) * transform.right * _bowData.releaseSpeed;
        }


        private void Update()
        {
            if (!IsServer) return;

            Vector2 direction = rb2D.velocity;
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
                NetworkDespawnAfter(1.0f);

                SoundManager.Instance.PlaySound(SoundType.ArrowProjectileHitGround, playRandom: true, collision.transform.position);
            }

            if (enemyLayer == (enemyLayer | (1 << collision.gameObject.layer)))
            {
                ArrowPropertiesWhenCollide();
                NetworkDespawnAfter(1.0f);

            }
        }



        /// <summary>
        /// Sets the arrow properties when it collides with something.
        /// </summary>
        private void ArrowPropertiesWhenCollide()
        {
            rb2D.velocity = Vector2.zero;
            rb2D.isKinematic = true;
            spriteRenderer.enabled = false;     
        }     
    }
}
