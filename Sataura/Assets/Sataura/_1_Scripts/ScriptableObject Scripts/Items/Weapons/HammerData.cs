using UnityEngine;

namespace Sataura
{
    [CreateAssetMenu(fileName = "HammerData", menuName = "Sataura/Item/Weapons/HammerData", order = 51)]
    public class HammerData : WeaponData
    {
        [Header("HammerData SETTINGS")]
        public float releaseForce = 15f;
    }
}
