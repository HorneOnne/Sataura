using System.Collections.Generic;
using UnityEngine;

namespace Sataura
{
    [System.Serializable]
    public struct WorldChest
    {
        public List<ChestInventorySaveData> allChests;

        public WorldChest(List<Chest> chests)
        {
            allChests = new List<ChestInventorySaveData>();
            for (int i = 0; i < chests.Count; i++)
            {
                allChests.Add(new ChestInventorySaveData(chests[i].Inventory, chests[i].transform.position, chests[i].transform.rotation.eulerAngles));
            }
        }



        [System.Serializable]
        public struct ChestInventorySaveData
        {
            public List<ItemSlotStruct> itemSlotData;
            public Vector2 position;
            public Vector3 rotation;

            public ChestInventorySaveData(ChestInventory chestInventory, Vector2 position, Vector3 rotation)
            {
                this.itemSlotData = new List<ItemSlotStruct>();
                this.position = position;
                this.rotation = rotation;

                for (int i = 0; i < 36; i++)
                {
                    var itemSlot = chestInventory.inventory[i];
                    itemSlotData.Add(new ItemSlotStruct(GameDataManager.Instance.GetItemID(itemSlot.ItemData), itemSlot.ItemQuantity));
                }
            }
        }
    }
}
