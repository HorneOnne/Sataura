using UnityEngine;

namespace Sataura
{
    [CreateAssetMenu(fileName = "BootData", menuName = "Sataura/Item/Equipment/BootData", order = 51)]
    public class BootData : ItemData
    {
        [Header("Boot Data")]
        public int hastePercent;
        public float leapPercent;
    }
}