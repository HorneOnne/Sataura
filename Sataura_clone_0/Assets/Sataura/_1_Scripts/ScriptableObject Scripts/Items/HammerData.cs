using UnityEngine;

namespace Sataura
{
    [CreateAssetMenu(fileName = "HammerData", menuName = "Sataura/Item/Weapons/HammerData", order = 51)]
    public class HammerData : ItemData
    {
        [Header("HammerData SETTINGS")]
        public int damage;
        public float releaseForce = 15f;
        public float size = 1.0f;

        [Header("UseType")]
        public int useType = 1;
    }
}
