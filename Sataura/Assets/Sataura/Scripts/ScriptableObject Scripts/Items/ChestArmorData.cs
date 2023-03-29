using UnityEngine;

namespace Sataura
{
    [CreateAssetMenu(fileName = "New Sword Object", menuName = "UltimateItemSystem/Item/Equipment/ChestArmor", order = 51)]
    public class ChestArmorData : ItemData
    {
        [Header("ChestArmor Data")]
        public int armor;
    }
}