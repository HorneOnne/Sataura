using UnityEngine;

namespace Sataura
{
    [CreateAssetMenu(fileName = "LightningStaff", menuName = "Sataura/Item/Weapons/LightningStaff", order = 51)]
    public class LightningStaffData : WeaponData
    {
        [Header("LightningStaff")]
        public float timeExist = 1.0f;
        public float attackCycle = 0.3f;
        public float lightingDamgedZone = 1;
    }
}
