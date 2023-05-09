using UnityEngine;

namespace Sataura
{
    [CreateAssetMenu(fileName = "ShieldData", menuName = "UltimateItemSystem/Item/Equipment/Shield", order = 51)]
    public class ShieldData : ItemData
    {
        [Header("Shield Data")]
        public int armor;
    }
}