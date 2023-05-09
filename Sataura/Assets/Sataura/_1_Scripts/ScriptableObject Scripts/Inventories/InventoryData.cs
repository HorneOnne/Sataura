using System.Collections.Generic;
using UnityEngine;

namespace Sataura
{
    [CreateAssetMenu(fileName = "Inventory", menuName = "Sataura/Inventory", order = 51)]
    [System.Serializable]
    public class InventoryData : ScriptableObject
    {
        public List<ItemSlot> itemSlots;
    }
}

