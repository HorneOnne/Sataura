using UnityEngine;

namespace Sataura
{
    [CreateAssetMenu(fileName = "New Bow Object", menuName = "UltimateItemSystem/Item/Weapons/Bow", order = 51)]
    public class BowData : ItemData
    {
        [Header("BOW SETTINGS")]
        public int baseAttackDamage;    // used for add damage with arrow
        public float releaseSpeed;  // Affect the arrow speed

        [Header("UPGRADE SETTINGS")]
        public int useType;
    }
}

