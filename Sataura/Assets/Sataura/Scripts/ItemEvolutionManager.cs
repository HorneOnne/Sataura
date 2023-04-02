using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Sataura
{
    public class ItemEvolutionManager : Singleton<ItemEvolutionManager>
    {
        [Header("Evolution Recipe")]
        public List<ItemEvolution> itemEvolutions;
        private List<ItemData> evoItems;


        [Header("Base item datas")]
        public List<ItemData> baseItems;


        private void Start()
        {
            CreateEvoItemList();
        }

        private void CreateEvoItemList()
        {
            evoItems = new List<ItemData>();
            for (int i = 0; i < itemEvolutions.Count; i++)
            {
                evoItems.Add(itemEvolutions[i].evolutionItemData);
            }
        }



        public bool IsEvoItem(ItemData itemData)
        {
            return evoItems.Contains(itemData); 
        }

       
        public ItemData GetEvolutionItemNeeded(ItemData baseItem)
        {
            for (int i = 0; i < itemEvolutions.Count; i++)
            {
                if (ItemData.IsSameName(itemEvolutions[i].firstItemData, baseItem))
                {
                    return itemEvolutions[i].secondItemData;
                }
                if (ItemData.IsSameName(itemEvolutions[i].secondItemData, baseItem))
                {
                    return itemEvolutions[i].firstItemData;
                }
            }

            return null;
        }


        public ItemData GetEvolutionItem(ItemData baseItemA, ItemData baseItemB)
        {
            for (int i = 0; i < itemEvolutions.Count; i++)
            {
                if (ItemData.IsSameItem(itemEvolutions[i].firstItemData, baseItemA) &&
                    ItemData.IsSameItem(itemEvolutions[i].secondItemData, baseItemB))
                {
                    return itemEvolutions[i].evolutionItemData;
                }
                else if (ItemData.IsSameItem(itemEvolutions[i].firstItemData, baseItemB) &&
                    ItemData.IsSameItem(itemEvolutions[i].secondItemData, baseItemA))
                {
                    return itemEvolutions[i].evolutionItemData;
                }
            }

            return null;
        }

        public ItemData GetEvolutionItem(ItemData baseItem)
        {
            for (int i = 0; i < itemEvolutions.Count; i++)
            {
                if (ItemData.IsSameName(itemEvolutions[i].firstItemData, baseItem) ||
                    ItemData.IsSameName(itemEvolutions[i].secondItemData, baseItem))
                {
                    return itemEvolutions[i].evolutionItemData;
                }
            }

            return null;
        }


        public void GetItemsNeededToEvol(ItemData evolItem, out ItemData baseItemA, out ItemData baseItemB)
        {
            baseItemA = null;
            baseItemB = null;

            for (int i = 0; i < itemEvolutions.Count; i++)
            {
                if (itemEvolutions[i].evolutionItemData.Equals(evolItem))
                {
                    baseItemA = itemEvolutions[i].firstItemData;   
                    baseItemB = itemEvolutions[i].secondItemData;

                    return;
                }
            }
        }



        

        private ItemData GetRandomBaseItem()
        {
            return baseItems[Random.Range(0, baseItems.Count)];
        }

        /*private ItemData GetRandomBaseItem(Player player)
        {
            var playerIngameInventory = player.PlayerInGameInventory;
            var baseItem = baseItems[Random.Range(0, baseItems.Count)];

            for (int i = 0; i < playerIngameInventory.inGameInventory.Count; i++)
            {

            }


            return baseItem;
        }*/


        /*public HashSet<ItemData> GenerateUpgradeItemData(Player player)
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
        }*/


        private ItemData HasItemEvolution(Player player)
        {
            var playerInGameInventory = player.PlayerInGameInventory;


            for(int i = 0; i < playerInGameInventory.inGameInventory.Count; i++)
            {
                if (playerInGameInventory.inGameInventory[i].HasItemData() == false)
                    continue;
                if (IsEvoItem(playerInGameInventory.inGameInventory[i].ItemData))
                    continue;

                if (playerInGameInventory.inGameInventory[i].ItemData.IsMaxLevel())
                {                 
                    var itemEvoNeeded = GetEvolutionItemNeeded(playerInGameInventory.inGameInventory[i].ItemData);
                    for (int j = 0; j < playerInGameInventory.inGameInventory.Count; j++)
                    {
                        if (i == j)
                            continue;

                        if (playerInGameInventory.inGameInventory[j].HasItemData() == false)
                            continue;

                        if (playerInGameInventory.inGameInventory[j].ItemData.Equals(itemEvoNeeded))
                        {
                            //Debug.Log($"Has Evolution item: {GetEvolutionItem(playerInGameInventory.inGameInventory[i].ItemData, itemEvoNeeded)}");
                            var itemEvo = GetEvolutionItem(playerInGameInventory.inGameInventory[i].ItemData, itemEvoNeeded);
                            return itemEvo;
                        }
                    }
                }
            }


            return null;
        }

        public Dictionary<ItemData, bool> GenerateUpgradeItemData(Player player)
        {
            Debug.LogWarning("Optimize here.");
            Dictionary<ItemData, bool> upgradeItemDataDict = new Dictionary<ItemData, bool>();
            var playerInGameInventory = player.PlayerInGameInventory;
            var playerUseItem = player.PlayerUseItem;

            float timeLoad = 0.0f;
            while (true)
            {
                timeLoad += Time.deltaTime;

                // Break condition
                if (upgradeItemDataDict.Count >= 3)
                    break;
                if (timeLoad > 1.0f)
                {
                    Debug.LogWarning("Out of time Load...");
                    break;
                }

                var baseItem = GetRandomBaseItem();
                if (upgradeItemDataDict.ContainsKey(baseItem) == true)
                    continue;


                Debug.Log("Being here.");
                var itemEvo = HasItemEvolution(player);
                if (itemEvo != null)
                {
                    Debug.Log($"Item evo: {itemEvo}");

                    if(upgradeItemDataDict.ContainsKey(itemEvo) == false)
                        upgradeItemDataDict.Add(itemEvo, false);
                }

                if (playerUseItem.HasEvoOfBaseItem(baseItem) == true)
                    continue;


                if (playerUseItem.HasBaseItem(baseItem))
                {
                    int baseItemIndex = playerUseItem.FindBaseItemIndex(baseItem);
                    var currentItemDataAtIndex = playerInGameInventory.inGameInventory[baseItemIndex].ItemData;
                    var itemUpgradeVersion = playerUseItem.GetUpgradeVersionOfItem(currentItemDataAtIndex);
                    if (itemUpgradeVersion != null)
                    {
                        //Debug.Log($"Has baseItem: {itemUpgradeVersion}");
                        ItemData itemNeedToEvo = GetEvolutionItemNeeded(baseItem);
                        bool hasEvo = false;
                        if (itemNeedToEvo != null)
                        {
                            if (playerUseItem.HasBaseItem(itemNeedToEvo))
                            {
                                hasEvo = true;
                            }
                        }

                        if (upgradeItemDataDict.ContainsKey(itemUpgradeVersion) == false)
                            upgradeItemDataDict.Add(itemUpgradeVersion, hasEvo);
                    }
                }
                else
                {
                    //Debug.Log($"Not has baseItem: {baseItem}");
                    ItemData itemNeedToEvo = GetEvolutionItemNeeded(baseItem);            
                    bool hasEvo = false;
                    
                    if (itemNeedToEvo != null)
                    {
                        if (playerUseItem.HasBaseItem(itemNeedToEvo))
                        {
                            hasEvo = true;
                        }
                    }

                    if (upgradeItemDataDict.ContainsKey(baseItem) == false)
                        upgradeItemDataDict.Add(baseItem, hasEvo);
                }
            }

            return upgradeItemDataDict;
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
