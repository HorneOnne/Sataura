using System.Collections.Generic;

namespace Sataura
{
    [System.Serializable]
    public struct InventoryStruct
    {
        public List<ItemSlotStruct> itemDatas;

        public InventoryStruct(InventoryData inventoryData)
        {
            int size = inventoryData.itemSlots.Count;
            itemDatas = new List<ItemSlotStruct>();
            for (int i = 0; i < size; i++)
            {
                var itemSlot = inventoryData.itemSlots[i];
                itemDatas.Add(new ItemSlotStruct(GameDataManager.Instance.GetItemID(itemSlot.ItemData), itemSlot.ItemQuantity));
            }
        }
    }
}
