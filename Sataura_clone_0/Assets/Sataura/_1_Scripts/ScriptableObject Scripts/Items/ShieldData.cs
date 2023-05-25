using UnityEngine;

namespace Sataura
{
    [CreateAssetMenu(fileName = "ShieldData", menuName = "Sataura/Item/Shield", order = 51)]
    public class ShieldData : ItemData
    {
        [Header("Shield Data")]
        public int armor;
    }
}