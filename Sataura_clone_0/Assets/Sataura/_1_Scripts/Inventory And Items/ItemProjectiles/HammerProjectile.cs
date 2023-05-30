using UnityEngine;
using System.Collections;

namespace Sataura
{
    public class HammerProjectile : MultipleTargetPhysicalProjectile
    {
        public override void Fire(IngamePlayer fromPlayer, Vector2 targetPosition, WeaponData weaponData, bool updateProjectileSize = true)
        {
            base.Fire(fromPlayer, targetPosition, weaponData);
            
            ThrowUp();
            Invoke(nameof(base.NetworkDespawn), 7.0f);
        }


        private void ThrowUp()
        {
            Vector2 throwUpVector = Vector2.up + new Vector2(Random.Range(-1f, 1f), 0f);
            rb2D.AddForce(throwUpVector.normalized * ((HammerData)weaponData).releaseForce, ForceMode2D.Impulse);
            rb2D.AddTorque(10f, ForceMode2D.Impulse);
        }
    }
}

