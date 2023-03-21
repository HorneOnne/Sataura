using Unity.Netcode;
using UnityEngine;

namespace Sataura
{
    /// <summary>
    /// A class that represents a sword projectile that can cause damage to an enemy.
    /// </summary>
    public class SwordProjectile_001 : NetworkProjectile, ICanCauseDamage
    {
        private SwordData swordData;
        private Vector3 startRotationAngle;
        [SerializeField] private float swingDuration;
        const float maxSwingRotation = 177.0f; // degrees
        private float swingTimer = 0.0f;


        private EdgeCollider2D projectileEdgeCollider;
        private int cachedPlayerFacingDirection;

        public override void OnNetworkSpawn()
        {
            if (NetworkManager.Singleton.IsServer == false) return;
            projectileEdgeCollider = GetComponent<EdgeCollider2D>();
           
        }


        [ServerRpc]
        public void LoadSwordProjectileDataServerRpc(Vector2 mousePosition)
        {
            this.swordData = (SwordData)ItemData;
            swingDuration = 1.0f / (swordData.usageVelocity + 0.001f);
            SetSwingDirection(mousePosition);


            LoadSwordProjectileDataClientRpc(mousePosition);
        }

        [ClientRpc]
        private void LoadSwordProjectileDataClientRpc(Vector2 mousePosition)
        {
            this.swordData = (SwordData)ItemData;
            swingDuration = 1.0f / (swordData.usageVelocity + 0.001f);
        }


        [ClientRpc]
        private void PredictSwordLocalPositionClientRpc()
        {
            transform.localPosition = Vector3.zero;
        }

        /// <summary>
        /// Update method called once per frame to rotate the sword while the timer is less than the rotation time.
        /// </summary>
        void Update()
        {
            if (!IsServer) return;
            
            // increment timer
            swingTimer += Time.deltaTime;
            // rotate the object
            if (swingTimer < swingDuration)
            {
                PredictSwordLocalPositionClientRpc();
                Swing();          
            }
            else
            {               
                GetComponent<NetworkObject>().Despawn();
                //Destroy(gameObject);

            }

        }


        /// <summary>
        /// Sets the direction of the sword swing based on the direction the player is facing.
        /// </summary>
        private void SetSwingDirection(Vector2 mousePosition)
        {
            if (mousePosition.x - transform.position.x > 0)
                cachedPlayerFacingDirection = 1;
            else
                cachedPlayerFacingDirection = -1;
        }


        /// <summary>
        /// Rotates the sword based on the direction the player is facing and the rotation time.
        /// </summary>
        private void Swing()
        {
            if (cachedPlayerFacingDirection == 1)
                transform.Rotate(-Vector3.forward * Time.deltaTime * maxSwingRotation / swingDuration);
            else
                transform.Rotate(Vector3.forward * Time.deltaTime * maxSwingRotation / swingDuration);
        }


        public int GetDamage()
        {
            return ((SwordData)ItemData).damage;
        }

        public float GetKnockback()
        {
            return ((SwordData)ItemData).knockback;
        }
    }
}

