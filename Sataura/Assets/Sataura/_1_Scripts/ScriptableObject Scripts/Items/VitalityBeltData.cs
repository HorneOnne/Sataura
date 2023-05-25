using UnityEngine;

namespace Sataura
{
    [CreateAssetMenu(fileName = "VitalityBelt", menuName = "Sataura/Item/Accessories/VitalityBelt", order = 51)]
    public class VitalityBeltData : ItemData
    {
        [Header("VitalityBelt Data")]
        public int maxHealthPercentIncrease;
    }
}