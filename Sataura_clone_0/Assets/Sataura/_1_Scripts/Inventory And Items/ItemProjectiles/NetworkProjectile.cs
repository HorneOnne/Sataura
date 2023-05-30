using Unity.Netcode;
using UnityEngine;

namespace Sataura
{
    public abstract class NetworkProjectile : NetworkBehaviour
    {
        [Header("===== NetworkProjectile =====")]
        [Header("References")]
        [SerializeField] protected NetworkObject networkObject;
        [SerializeField] protected SpriteRenderer spriteRenderer;
        [SerializeField] protected Rigidbody2D rb2D;
        [SerializeField] protected DamageType damageType;
        [SerializeField] protected LayerMask enemyLayer;

        [Header("Runtime References")]
        [SerializeField] protected WeaponData weaponData;

        public void NetworkSpawn()
        {
            if (networkObject.IsSpawned == false)
                networkObject.Spawn();
        }


        public void NetworkDespawn()
        {
            if (networkObject.IsSpawned)
            {
                networkObject.Despawn();
            }
        }

        public void NetworkDespawnAfter(float time)
        {
            Invoke(nameof(NetworkDespawn), time);
        }

        public void UpdateProjectileSize(Player player)
        {
            transform.localScale *= weaponData.size * player.characterData._currentArea;
        }
    }
}