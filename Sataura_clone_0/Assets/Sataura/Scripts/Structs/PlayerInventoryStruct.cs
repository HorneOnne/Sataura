using System.Collections.Generic;

namespace Sataura
{
    [System.Serializable]
    public struct PlayerInventoryStruct
    {
        public List<ItemSlotStruct> itemDatas;

        public PlayerInventoryStruct(PlayerInventory playerInventory)
        {
            itemDatas = new List<ItemSlotStruct>();
            for (int i = 0; i < playerInventory.inventory.Count; i++)
            {
                var itemSlot = playerInventory.inventory[i];
                itemDatas.Add(new ItemSlotStruct(GameDataManager.Instance.GetItemID(itemSlot.ItemData), itemSlot.ItemQuantity));
            }
        }
    }
}
