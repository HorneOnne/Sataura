using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Sataura
{
    public class ItemEvolutionManager : Singleton<ItemEvolutionManager>
    {
        [Header("Evolution Recipe")]
        public List<ItemEvolution> itemEvolutions;

        [Header("Base item datas")]
        public List<ItemData> baseItems;


        private ItemData GetRandomBaseItem()
        {
            return baseItems[Random.Range(0, baseItems.Count)];
        }


        public HashSet<ItemData> GenerateUpgradeItemData(Player player)
        {
            HashSet<ItemData> upgradeItemDataSet = new HashSet<ItemData>();
            var playerInGameInventory = player.PlayerInGameInventory;

            float timeLoad = 0.0f;
            while (true)
            {
                timeLoad += Time.deltaTime;

                // Break condition
                if (upgradeItemDataSet.Count >= 3)
                    break;
                if (timeLoad > 1.0f)
                {
                    Debug.LogWarning("Out of time Load...");
                    break;
                }

                var baseItem = GetRandomBaseItem();
                if (upgradeItemDataSet.Contains(baseItem) == true)
                    continue;


                if (playerInGameInventory.HasBaseItem(baseItem))
                {
                    int baseItemIndex = playerInGameInventory.FindBaseItemIndex(baseItem);
                    var currentItemDataAtIndex = playerInGameInventory.inGameInventory[baseItemIndex].ItemData;
                    var itemUpgradeVersion = playerInGameInventory.GetUpgradeVersionOfItem(currentItemDataAtIndex);
                    if (itemUpgradeVersion != null)
                    {
                        Debug.Log($"Has baseItem: {itemUpgradeVersion}");
                        upgradeItemDataSet.Add(itemUpgradeVersion);
                    }
                }
                else
                {

                    upgradeItemDataSet.Add(baseItem);
                    Debug.Log($"Not has baseItem: {baseItem}");
                }

            }

            return upgradeItemDataSet;
        }


        [System.Serializable]
        public struct ItemEvolution
        {
            public string itemName;
            public ItemData firstItemData;
            public ItemData secondItemData;

            public ItemData evolutionItemData;
        }
    }


}
