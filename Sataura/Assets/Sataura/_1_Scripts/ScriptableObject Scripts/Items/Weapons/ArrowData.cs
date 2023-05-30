using UnityEngine;

namespace Sataura
{
    [CreateAssetMenu(fileName = "Arrow", menuName = "Sataura/Item/Projectiles/Arrow", order = 51)]
    public class ArrowData : WeaponData
    {
        [Tooltip("Index for getting particles frames in ProjectileParticleDataManager")]
        public int particle;
    }
}