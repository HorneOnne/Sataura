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



        private ItemData HasItemEvolution(Player player)
        {
            var playerInGameInventory = player.PlayerInGameInventory;


            for (int i = 0; i < playerInGameInventory.weapons.Count; i++)
            {
                if (playerInGameInventory.weapons[i].HasItemData() == false)
                    continue;
                if (IsEvoItem(playerInGameInventory.weapons[i].ItemData))
                    continue;

                if (playerInGameInventory.weapons[i].ItemData.IsMaxLevel())
                {
                    var itemEvoNeeded = GetEvolutionItemNeeded(playerInGameInventory.weapons[i].ItemData);
                    for (int j = 0; j < playerInGameInventory.accessories.Count; j++)
                    {    
                        if (playerInGameInventory.accessories[j].HasItemData() == false)
                            continue;

                        if (playerInGameInventory.accessories[j].ItemData.Equals(itemEvoNeeded))
                        {
                            //Debug.Log($"Has Evolution item: {GetEvolutionItem(playerInGameInventory.inGameInventory[i].ItemData, itemEvoNeeded)}");
                            var itemEvo = GetEvolutionItem(playerInGameInventory.weapons[i].ItemData, itemEvoNeeded);
                            return itemEvo;
                        }
                    }
                }
            }


            return null;
        }

        public Dictionary<ItemData, bool> GenerateUpgradeItemData(Player player)
        {
            Debug.Log(player.name);
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

                var itemEvo = HasItemEvolution(player);
                if (itemEvo != null)
                {
                    Debug.Log($"Item evo: {itemEvo}");

                    if (upgradeItemDataDict.ContainsKey(itemEvo) == false)
                        upgradeItemDataDict.Add(itemEvo, false);
                }

                

                if (baseItem.itemCategory == ItemCategory.Weapons)
                {
                    if (playerUseItem.HasEvoOfBaseItem(baseItem) == true)
                        continue;

                    if (playerUseItem.HasBaseItem(baseItem))
                    {
                        Debug.Log("Here 0");

                        int baseItemIndex = playerUseItem.FindBaseItemIndex(baseItem);
                        var currentItemDataAtIndex = playerInGameInventory.weapons[baseItemIndex].ItemData;
                        var itemUpgradeVersion = playerUseItem.GetUpgradeVersionOfItem(currentItemDataAtIndex);
                        if (itemUpgradeVersion != null)
                        {
                            ItemData itemNeedToEvo = GetEvolutionItemNeeded(baseItem);
                            Debug.Log($"here 1: {itemNeedToEvo.itemName}");
                            bool hasEvo = false;
                            if (itemNeedToEvo != null)
                            {
                                if (playerUseItem.HasBaseItem(itemNeedToEvo))
                                {
                                    hasEvo = true;
                                }
                            }
                            Debug.Log($"here2 :  hasEvo = {hasEvo}");
                            if (upgradeItemDataDict.ContainsKey(itemUpgradeVersion) == false)
                            {
                                upgradeItemDataDict.Add(itemUpgradeVersion, hasEvo);
                                Debug.Log($"here3 :  hasEvo = {hasEvo}");
                            }
                                
                        }
                    }
                    else
                    {
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
                else if (baseItem.itemCategory == ItemCategory.Accessories)
                {        
                    if (playerUseItem.HasBaseItem(baseItem))
                    {
                        int baseItemIndex = playerUseItem.FindBaseItemIndex(baseItem);
                        var currentItemDataAtIndex = playerInGameInventory.accessories[baseItemIndex].ItemData;
                        var itemUpgradeVersion = playerUseItem.GetUpgradeVersionOfItem(currentItemDataAtIndex);
                        if (itemUpgradeVersion != null)
                        {
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
