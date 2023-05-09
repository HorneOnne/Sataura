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



        public void SetData(ItemData _itemData)
        {
            this.itemData = _itemData;
        }

        public void UpdateData(bool hasEvo = false)
        {
            skillIcon.sprite = itemData.icon;
            skillNameText.text = itemData.name;
            skillDescText.text = itemData.description;
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
            skillNameText.text = itemData.name;
            skillDescText.text = itemData.description;
            levelText.text = $"level: {itemData.currentLevel}";

            if(hasEvo)
            {
                Debug.Log("Evo already set active.");

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
            var playerInGameInventory = GameDataManager.Instance.singleModePlayer.GetComponent<Player>().playerInGameInventory;
            var playerUseItem = GameDataManager.Instance.singleModePlayer.GetComponent<Player>().playerUseItem;


            if (ItemEvolutionManager.Instance.IsEvoItem(itemData))
            {
                Debug.Log($"{itemData} is evoItem.");
                ItemEvolutionManager.Instance.GetItemsNeededToEvol(itemData, out ItemData itemNeedToEvolA, out ItemData itemNeedToEvolB);
                
                switch(itemNeedToEvolA.itemCategory)
                {
                    case ItemCategory.Skill_Weapons:
                        for (int i = 0; i < playerInGameInventory.weapons.Count; i++)
                        {
                            if (ItemData.IsSameItem(itemNeedToEvolA, playerInGameInventory.weapons[i].ItemData))
                            {
                                playerInGameInventory.weapons[i].ClearSlot();
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
                        for (int i = 0; i < playerInGameInventory.weapons.Count; i++)
                        {
                            if (ItemData.IsSameItem(itemNeedToEvolB, playerInGameInventory.weapons[i].ItemData))
                            {
                                playerInGameInventory.weapons[i].ClearSlot();
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


                playerInGameInventory.AddWeapons(itemData);
            }
            else
            {
                bool hasBaseItemDataInPlayerIngameInv = playerUseItem.HasBaseItem(itemData);
                int index;
                ItemData currentItemData;
                ItemData upgradeVersionItemData;

                if (hasBaseItemDataInPlayerIngameInv)
                {
                    switch (itemData.itemCategory)
                    {
                        case ItemCategory.Skill_Weapons:
                            index = playerUseItem.FindBaseItemIndex(itemData);
                            currentItemData = playerInGameInventory.weapons[index].ItemData;
                            upgradeVersionItemData = playerUseItem.GetUpgradeVersionOfItem(currentItemData);

                            if (upgradeVersionItemData != null)
                            {
                                playerInGameInventory.weapons[index] = new ItemSlot(upgradeVersionItemData, 1);
                            }
                            else
                            {
                                Debug.Log("Item has max level.");
                            }
                            break;
                        case ItemCategory.Skill_Accessories:
                            index = playerUseItem.FindBaseItemIndex(itemData);
                            currentItemData = playerInGameInventory.accessories[index].ItemData;
                            upgradeVersionItemData = playerUseItem.GetUpgradeVersionOfItem(currentItemData);

                            
                            if (upgradeVersionItemData != null)
                            {
                                playerInGameInventory.accessories[index] = new ItemSlot(upgradeVersionItemData, 1);
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
                    Debug.Log("Not have this item in ingame Inventory!!!!!!!");

                    switch (itemData.itemCategory)
                    {
                        case ItemCategory.Skill_Weapons:
                            playerInGameInventory.AddWeapons(itemData);
                            break;
                        case ItemCategory.Skill_Accessories:
                            playerInGameInventory.AddAccessories(itemData);
                            break;
                        default:
                            throw new System.Exception();
                    }                 
                }      
            }

            Debug.LogWarning("Refactory code here.");
            playerUseItem.ClearAllPassiveItemObjectInInventory();
            playerUseItem.CreateAllPassiveItemObjectInInventory();


            UIPlayerInGameSkills.Instance.UpdateUI();
            OnUpgradeButtonClicked?.Invoke();
        }

    }
}


