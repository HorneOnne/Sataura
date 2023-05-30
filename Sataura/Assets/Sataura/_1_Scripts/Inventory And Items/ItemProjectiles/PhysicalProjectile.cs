using Unity.Netcode;
using UnityEngine;

namespace Sataura
{
    public abstract class PhysicalProjectile : NetworkProjectile, IPhysicalDamage
    {      
        public override void OnNetworkSpawn()
        {
            damageType = DamageType.Physical;
        }

        public abstract void Fire(IngamePlayer fromPlayer, Vector2 targetPosition, WeaponData weaponData, bool updateProjectileSize = true);
        public abstract int GetPhysicalDamage();
        public abstract float GetKnockback();
    }


}