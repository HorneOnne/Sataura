using Unity.Netcode;
using UnityEngine;

namespace Sataura
{
    public class EnemyNetworkProjectile : NetworkBehaviour
    {
        #region Properties
        public BaseEnemy _baseEnemy;
        protected GameObject Model { get; set; }

        /// <summary>
        /// Whether the projectile uses gravity or not.
        /// </summary>
        public bool useGravity;
        #endregion


        [Header("References")]
        protected NetworkObject networkObject;
        protected ParticleControl particleControl;
        [HideInInspector] public SpriteRenderer spriteRenderer;
        [HideInInspector] protected Rigidbody2D rb;



        public override void OnNetworkSpawn()
        {
            networkObject = GetComponent<NetworkObject>();

            LoadComponents();
        }

        private void LoadComponents()
        {
            Model = GetComponentInChildren<SpriteRenderer>().gameObject;
            spriteRenderer = Model.GetComponent<SpriteRenderer>();
            rb = GetComponent<Rigidbody2D>();
            particleControl = GetComponentInChildren<ParticleControl>();
        }




        /// <summary>
        /// Enables or disables gravity on the game object.
        /// </summary>
        /// <param name="_useGravity">Whether to use gravity or not.</param>
        private void UseGravity(bool _useGravity)
        {
            this.useGravity = _useGravity;
            if (useGravity)
            {
                rb.isKinematic = false;
                rb.gravityScale = 1.0f;
            }
            else
            {
                rb.isKinematic = true;
            }
        }
    }
}