using System.Collections.Generic;
using UnityEngine;

namespace Sataura
{
    [CreateAssetMenu(fileName = "Inventory", menuName = "UltimateItemSystem/Inventory", order = 51)]
    public class InventoryData : ScriptableObject
    {
        public List<ItemSlot> itemSlots;
    }
}

