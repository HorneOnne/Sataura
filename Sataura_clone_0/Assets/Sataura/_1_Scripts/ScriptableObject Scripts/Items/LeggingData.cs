using UnityEngine;

namespace Sataura
{
    [CreateAssetMenu(fileName = "LeggingData", menuName = "Sataura/Item/Equipment/LeggingData", order = 51)]
    public class LeggingData : ItemData
    {
        [Header("Legging Properties")]
        public int armor;
    }
}
