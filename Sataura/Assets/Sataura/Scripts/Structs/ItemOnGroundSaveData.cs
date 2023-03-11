using System.Collections.Generic;
using UnityEngine;

namespace Sataura
{
    [System.Serializable]
    public struct ItemOnGroundSaveData
    {
        public List<ItemObjectData> itemDatas;

        public ItemOnGroundSaveData(List<Item> itemsOnGround)
        {
            itemDatas = new List<ItemObjectData>();
            for (int i = 0; i < itemsOnGround.Count; i++)
            {
                int itemID = GameDataManager.Instance.GetItemID(itemsOnGround[i].ItemData);

                itemDatas.Add(new ItemObjectData(itemID, itemsOnGround[i].transform.position, itemsOnGround[i].transform.eulerAngles));
            }

        }

        [System.Serializable]
        public struct ItemObjectData
        {
            public int itemID;
            public Vector2 position;
            public Vector3 rotation;

            public ItemObjectData(int itemID, Vector2 position, Vector3 rotation)
            {
                this.itemID = itemID;
                this.position = position;
                this.rotation = rotation;
            }
        }
    }
}
