using UnityEngine;

namespace Sataura
{
    [CreateAssetMenu(fileName = "LightningStaff", menuName = "Sataura/Item/Weapons/LightningStaff", order = 51)]
    public class LightningStaffData : ItemData
    {
        [Header("LightningStaff")]
        public int damage;
        public float timeExist = 1.0f;
        public float attackCycle = 0.3f;
        public float size = 1;
        public float lightingDamgedZone = 1;

        [Header("Use")]
        public int useType = 1;
    }
}
