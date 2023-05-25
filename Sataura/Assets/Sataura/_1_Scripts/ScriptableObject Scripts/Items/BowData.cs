using UnityEngine;

namespace Sataura
{
    [CreateAssetMenu(fileName = "New Bow Object", menuName = "Sataura/Item/Weapons/Bow", order = 51)]
    public class BowData : ItemData
    {
        [Header("BOW SETTINGS")]
        public int baseAttackDamage;    // used for add damage with arrow
        public float releaseSpeed;  // Affect the arrow speed
        public float size = 1;

        [Header("UPGRADE SETTINGS")]
        public int useType;
    }
}

