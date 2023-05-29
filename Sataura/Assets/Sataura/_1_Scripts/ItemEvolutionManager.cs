using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

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

        public ItemData GetEvolutionItem(ItemType itemType)
        {
            for (int i = 0; i < itemEvolutions.Count; i++)
            {
                if (itemEvolutions[i].evolutionItemData.itemType == itemType)
                {
                    return itemEvolutions[i].evolutionItemData;
                }

                if(itemEvolutions[i].firstItemData.itemType == itemType)
                {
                    return itemEvolutions[i].evolutionItemData;
                }


                if (itemEvolutions[i].secondItemData.itemType == itemType)
                {
                    return itemEvolutions[i].evolutionItemData;
                }
            }

            return null;
        }

        public ItemData GetEvolutionItem(ItemData weaponData, ItemData accessoryData)
        {
            for (int i = 0; i < itemEvolutions.Count; i++)
            {
                if (ItemData.IsSameItem(itemEvolutions[i].firstItemData, weaponData) &&
                    ItemData.IsSameItem(itemEvolutions[i].secondItemData, accessoryData))
                {
                    return itemEvolutions[i].evolutionItemData;
                }
                else if (ItemData.IsSameItem(itemEvolutions[i].firstItemData, accessoryData) &&
                    ItemData.IsSameItem(itemEvolutions[i].secondItemData, weaponData))
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



        private ItemData HasItemEvolution(IngamePlayer player)
        {
            var playerIngameSkills = player.playerIngameSkills;


            for (int i = 0; i < playerIngameSkills.weaponsData.itemSlots.Count; i++)
            {
                if (playerIngameSkills.weaponsData.itemSlots[i].HasItemData() == false)
                    continue;
                if (IsEvoItem(playerIngameSkills.weaponsData.itemSlots[i].ItemData))
                    continue;

                if (playerIngameSkills.weaponsData.itemSlots[i].ItemData.IsMaxLevel())
                {
                    var itemEvoNeeded = GetEvolutionItemNeeded(playerIngameSkills.weaponsData.itemSlots[i].ItemData);
                    for (int j = 0; j < playerIngameSkills.accessoriesData.itemSlots.Count; j++)
                    {    
                        if (playerIngameSkills.accessoriesData.itemSlots[j].HasItemData() == false)
                            continue;

                        if (playerIngameSkills.accessoriesData.itemSlots[j].ItemData.Equals(itemEvoNeeded))
                        {
                            var itemEvo = GetEvolutionItem(playerIngameSkills.weaponsData.itemSlots[i].ItemData, itemEvoNeeded);
                            return itemEvo;
                        }
                    }
                }
            }


            return null;
        }

        public Dictionary<ItemData, bool> GenerateUpgradeItemData(IngamePlayer player)
        {
            Debug.LogWarning("Optimize here. \t GenerateUpgradeItemData");
            Dictionary<ItemData, bool> upgradeItemDataDict = new Dictionary<ItemData, bool>();
            var playerIngameSkills = player.playerIngameSkills;


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
                    if (upgradeItemDataDict.ContainsKey(itemEvo) == false)
                        upgradeItemDataDict.Add(itemEvo, false);
                }

                
                switch(baseItem.itemCategory)
                {
                    case ItemCategory.Skill_Weapons:
                        if (playerIngameSkills.HasEvoOfBaseItem(baseItem) == true)
                            continue;

                        if (playerIngameSkills.HasItemType(baseItem.itemType))
                        {
                            int baseItemIndex = playerIngameSkills.FindItem(baseItem);
                            ItemData currentItemDataAtIndex = playerIngameSkills.weaponsData.itemSlots[baseItemIndex].ItemData;
                            var itemUpgradeVersion = playerIngameSkills.GetUpgradeVersionOfItem(currentItemDataAtIndex);
                            if (itemUpgradeVersion != null)
                            {                              
                                ItemData itemNeedToEvo = GetEvolutionItemNeeded(baseItem);
                                bool hasEvo = false;
                                if (itemNeedToEvo != null)
                                {
                                    if (playerIngameSkills.HasItemType(itemNeedToEvo.itemType))
                                    {
                                        hasEvo = true;
                                    }
                                }

                                if (upgradeItemDataDict.ContainsKey(itemUpgradeVersion) == false)
                                {
                                    upgradeItemDataDict.Add(itemUpgradeVersion, hasEvo);
                                }

                            }
                        }
                        else
                        {
                            ItemData itemNeedToEvo = GetEvolutionItemNeeded(baseItem);
                            bool hasEvo = false;

                            if (itemNeedToEvo != null)
                            {
                                if (playerIngameSkills.HasItemType(itemNeedToEvo.itemType))
                                {
                                    hasEvo = true;
                                }
                            }

                            if (upgradeItemDataDict.ContainsKey(baseItem) == false)
                                upgradeItemDataDict.Add(baseItem, hasEvo);
                        }
                        break;
                    case ItemCategory.Skill_Accessories:
                        if (playerIngameSkills.HasItemType(baseItem.itemType))
                        {
                            int baseItemIndex = playerIngameSkills.FindItem(baseItem);
                            var currentItemDataAtIndex = playerIngameSkills.accessoriesData.itemSlots[baseItemIndex].ItemData;
                            var itemUpgradeVersion = playerIngameSkills.GetUpgradeVersionOfItem(currentItemDataAtIndex);
                            if (itemUpgradeVersion != null)
                            {
                                ItemData itemNeedToEvo = GetEvolutionItemNeeded(baseItem);
                                bool hasEvo = false;
                                if (itemNeedToEvo != null)
                                {
                                    if (playerIngameSkills.HasItemType(itemNeedToEvo.itemType))
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
                                if (playerIngameSkills.HasItemType(itemNeedToEvo.itemType))
                                {
                                    hasEvo = true;
                                }
                            }

                            if (upgradeItemDataDict.ContainsKey(baseItem) == false)
                                upgradeItemDataDict.Add(baseItem, hasEvo);
                        }
                        break;
                    default:
                        break;
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
