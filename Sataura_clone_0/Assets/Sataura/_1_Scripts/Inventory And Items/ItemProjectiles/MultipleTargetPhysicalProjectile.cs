using UnityEngine;

namespace Sataura
{
    public class MultipleTargetPhysicalProjectile : PhysicalProjectile
    {
        public override float GetKnockback()
        {
            return weaponData.knockback;
        }

        public override int GetPhysicalDamage()
        {
            return weaponData.damage;
        }

        public override void Fire(IngamePlayer fromPlayer, Vector2 targetPosition, WeaponData weaponData, bool updateProjectileSize = true)
        {           
            this.weaponData = weaponData;
            if (updateProjectileSize)
                base.UpdateProjectileSize(fromPlayer);

            NetworkSpawn();
        }
    }
}