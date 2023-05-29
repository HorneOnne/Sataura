using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace Sataura
{
    public class UIUpgradeSkill : MonoBehaviour
    {
        // Events
        public static event Action OnUpgradeButtonClicked;

        [Header("Data")]
        [SerializeField] private ItemData itemData;


        [Header("UI")]
        [SerializeField] private Image skillIcon;
        [SerializeField] private TextMeshProUGUI skillNameText;
        [SerializeField] private TextMeshProUGUI skillDescText;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private GameObject evoParent;
        [SerializeField] private Image evoIcon;


        [Header("References")]
        private static IngamePlayer _ingamePlayer;


        private void Start()
        {
            _ingamePlayer = GameDataManager.Instance.ingamePlayer;
        }

        public void SetData(ItemData _itemData)
        {
            this.itemData = _itemData;
        }

        public void UpdateData(bool hasEvo = false)
        {
            skillIcon.sprite = itemData.icon;
            skillNameText.text = itemData.name;
            skillDescText.text = itemData.hoverDescription;
            levelText.text = $"level: {itemData.currentLevel}";

            if (hasEvo)
            {
                evoParent.SetActive(true);
                evoIcon.sprite = ItemEvolutionManager.Instance.GetEvolutionItemNeeded(itemData).icon;
            }
            else
            {
                evoParent.SetActive(false);
            }
        }

        public void UpdateData(ItemData _itemData, bool hasEvo = false)
        {
            this.itemData = _itemData;

            skillIcon.sprite = itemData.icon;
            skillNameText.text = itemData.itemName;
            skillDescText.text = itemData.ingameDescription;
            levelText.text = $"level: {itemData.currentLevel}";

            if(hasEvo)
            {
                evoParent.SetActive(true);
                evoIcon.sprite = ItemEvolutionManager.Instance.GetEvolutionItemNeeded(itemData).icon;
            }
            else
            {
                evoParent.SetActive(false);
            }
        }

   
        public void OnUIUpgradeSkillButtonClicked()
        {
            Time.timeScale = 1.0f;
            UIManager.Instance.CloseUpgradeItemSkillPanel();
            var playerIngameSkills = _ingamePlayer.playerIngameSkills;


            if (ItemEvolutionManager.Instance.IsEvoItem(itemData))
            {
                Debug.Log($"{itemData} is evoItem.");
                ItemEvolutionManager.Instance.GetItemsNeededToEvol(itemData, out ItemData itemNeedToEvolA, out ItemData itemNeedToEvolB);
                
                switch(itemNeedToEvolA.itemCategory)
                {
                    case ItemCategory.Skill_Weapons:
                        for (int i = 0; i < playerIngameSkills.accessoriesData.itemSlots.Count; i++)
                        {
                            if (ItemData.IsSameItem(itemNeedToEvolA, playerIngameSkills.weaponsData.itemSlots[i].ItemData))
                            {
                                playerIngameSkills.weaponsData.itemSlots[i].ClearSlot();
                            }
                        }
                        break;
                    case ItemCategory.Skill_Accessories:
                        /*for (int i = 0; i < playerInGameInventory.accessories.Count; i++)
                        {
                            if (ItemData.IsSameItem(itemNeedToEvolA, playerInGameInventory.accessories[i].ItemData))
                            {
                                playerInGameInventory.accessories[i].ClearSlot();
                            }
                        }*/
                        break;
                    default:
                        throw new Exception();
                }

                switch (itemNeedToEvolB.itemCategory)
                {
                    case ItemCategory.Skill_Weapons:
                        for (int i = 0; i < playerIngameSkills.accessoriesData.itemSlots.Count; i++)
                        {
                            if (ItemData.IsSameItem(itemNeedToEvolB, playerIngameSkills.weaponsData.itemSlots[i].ItemData))
                            {
                                playerIngameSkills.weaponsData.itemSlots[i].ClearSlot();
                            }
                        }
                        break;
                    case ItemCategory.Skill_Accessories:
                        /*for (int i = 0; i < playerInGameInventory.accessories.Count; i++)
                        {
                            if (ItemData.IsSameItem(itemNeedToEvolB, playerInGameInventory.accessories[i].ItemData))
                            {
                                playerInGameInventory.accessories[i].ClearSlot();
                            }
                        }*/
                        break;
                    default:
                        throw new Exception();
                }


                playerIngameSkills.AddWeapons(itemData);
            }
            else
            {
                bool alreadyHasItemType = playerIngameSkills.HasItemType(itemData.itemType);
                int index;
                ItemData currentItemData;
                ItemData upgradeVersionItemData;

                if (alreadyHasItemType)
                {
                    switch (itemData.itemCategory)
                    {
                        case ItemCategory.Skill_Weapons:
                            index = playerIngameSkills.FindItem(itemData);
                            currentItemData = playerIngameSkills.weaponsData.itemSlots[index].ItemData;
                            upgradeVersionItemData = playerIngameSkills.GetUpgradeVersionOfItem(currentItemData);

                            if (upgradeVersionItemData != null)
                            {
                                playerIngameSkills.weaponsData.itemSlots[index] = new ItemSlot(upgradeVersionItemData, 1);
                            }
                            else
                            {
                                Debug.Log("Item has max level.");
                            }
                            break;
                        case ItemCategory.Skill_Accessories:
                            index = playerIngameSkills.FindItem(itemData);
                            currentItemData = playerIngameSkills.accessoriesData.itemSlots[index].ItemData;
                            upgradeVersionItemData = playerIngameSkills.GetUpgradeVersionOfItem(currentItemData);
                            
                            if (upgradeVersionItemData != null)
                            {
                                playerIngameSkills.accessoriesData.itemSlots[index] = new ItemSlot(upgradeVersionItemData, 1);
                                playerIngameSkills.UpdateStatsEquip(itemData);
                            }
                            else
                            {
                                Debug.Log("Item has max level.");
                            }
                            break;
                        default:
                            throw new System.Exception();
                    }
                }
                else
                {
                    //Debug.Log("Not have this item in ingame Inventory!!!!!!!");

                    switch (itemData.itemCategory)
                    {
                        case ItemCategory.Skill_Weapons:
                            playerIngameSkills.AddWeapons(itemData);
                            break;
                        case ItemCategory.Skill_Accessories:
                            playerIngameSkills.AddAccessories(itemData);
                            playerIngameSkills.UpdateStatsEquip(itemData);
                            break;
                        default:
                            throw new System.Exception();
                    }                 
                }      
            }

            Debug.LogWarning("Refactory code here.");
            _ingamePlayer.playerUseItem.ClearAllPassiveItemObjectInInventory();
            _ingamePlayer.playerUseItem.CreateAllPassiveItemObjectInInventory();


            UIPlayerInGameSkills.Instance.UpdateUI();
            OnUpgradeButtonClicked?.Invoke();
        }

    }
}


