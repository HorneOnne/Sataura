using UnityEngine;

namespace Sataura
{
    [CreateAssetMenu(fileName = "BootData", menuName = "Sataura/Item/Equipment/BootData", order = 51)]
    public class BootData : ItemData
    {
        [Header("Boot Data")]
        public float dashForce = 50f;
        public float dashTime = 0.2f;
        public float dashCooldown = 1.0f;
        public SpecialBootType specialBootType;
    }
}