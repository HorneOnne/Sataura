using UnityEngine;

namespace Sataura
{
    [CreateAssetMenu(fileName = "New Bow Object", menuName = "Sataura/Item/Weapons/Bow", order = 51)]
    public class BowData : WeaponData
    {
        [Header("BOW SETTINGS")]
        public float releaseSpeed;  // Affect the arrow speed
    }
}

