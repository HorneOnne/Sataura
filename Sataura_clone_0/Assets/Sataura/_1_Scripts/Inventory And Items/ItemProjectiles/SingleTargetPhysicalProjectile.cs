using UnityEngine;

namespace Sataura
{
    public class SingleTargetPhysicalProjectile : PhysicalProjectile
    {
        [Header("===== SingleTargetPhysicalProjectile =====")]
        protected bool _hasFiredAtEnemy;
        protected bool _hasKnockbackEnemy;
     

        public override void OnNetworkSpawn()
        {
            _hasFiredAtEnemy = false;
            _hasKnockbackEnemy = false;
        }

        protected void FiredAtEnemy()
        {
            if (_hasFiredAtEnemy == false)
            {
                _hasFiredAtEnemy = true;
            }          
        }

        protected void KnockbackAtEnemy()
        {
            if (_hasKnockbackEnemy == false)
            {
                _hasKnockbackEnemy = true;
            }
        }

        public override float GetKnockback()
        {         
            if (_hasKnockbackEnemy)
            {
                return 0;
            }
            else
            {
                KnockbackAtEnemy();
                return weaponData.knockback;
            }
        }

        public override int GetPhysicalDamage()
        {
            if (_hasFiredAtEnemy)
            {
                return 0;
            }
            else
            {
                FiredAtEnemy();
                return weaponData.damage;
            }
        }

        public override void Fire(IngamePlayer fromPlayer, Vector2 targetPosition, WeaponData weaponData, bool updateProjectileSize = true)
        {
            this.weaponData= weaponData;
            if (updateProjectileSize)
                base.UpdateProjectileSize(fromPlayer);

            NetworkSpawn();
        }
    }
}