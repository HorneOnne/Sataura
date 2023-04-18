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
                evoParent.SetActive(true);
                evoIcon.sprite = ItemEvolutionManager.Instance.GetEvolutionItemNeeded(itemData).icon;
            }
            else
            {
                evoParent.SetActive(false);
            }
        }

        private void OnEnable()
        {
            UpdateData(hasEvo: false);
        }


        public void OnUIUpgradeSkillButtonClicked()
        {
            Time.timeScale = 1.0f;
            UIManager.Instance.CloseUpgradeItemSkillPanel();
            var playerInGameInventory = GameDataManager.Instance.singleModePlayer.GetComponent<Player>().PlayerInGameInventory;
            var playerUseItem = GameDataManager.Instance.singleModePlayer.GetComponent<Player>().PlayerUseItem;


            if (ItemEvolutionManager.Instance.IsEvoItem(itemData))
            {
                ItemEvolutionManager.Instance.GetItemsNeededToEvol(itemData, out ItemData itemNeedToEvolA, out ItemData itemNeedToEvolB);
                
                for(int i = 0; i < playerInGameInventory.inGameInventory.Count; i++)
                {
                    if (ItemData.IsSameItem(itemNeedToEvolA, playerInGameInventory.inGameInventory[i].ItemData))
                    {
                        playerInGameInventory.inGameInventory[i].ClearSlot();
                    }

                    if (ItemData.IsSameItem(itemNeedToEvolB, playerInGameInventory.inGameInventory[i].ItemData))
                    {
                        playerInGameInventory.inGameInventory[i].ClearSlot();
                    }
                }

                playerInGameInventory.AddItem(itemData);
            }
            else
            {              
                bool hasBaseItemDataInPlayerIngameInv = playerUseItem.HasBaseItem(itemData);

                if (hasBaseItemDataInPlayerIngameInv)
                {
                    //playerInGameInventory.inGameInventory[i] = new ItemSlot(inventory[i].ItemData.upgradeRecipe.outputItemSlot.itemData, 1);
                    int index = playerUseItem.FindBaseItemIndex(itemData);
                    var currentItemData = playerInGameInventory.inGameInventory[index].ItemData;
                    var upgradeVersionItemData = playerUseItem.GetUpgradeVersionOfItem(currentItemData);

                    if (upgradeVersionItemData != null)
                    {
                        playerInGameInventory.inGameInventory[index] = new ItemSlot(upgradeVersionItemData, 1);
                    }
                    else
                    {
                        Debug.Log("Item has max level.");
                    }

                }
                else
                {
                    Debug.Log("Not have this item in ingame Inventory!!!!!!!");
                    playerInGameInventory.AddItem(itemData);
                    
                }
            }

            Debug.LogWarning("Refactory code here.");
            playerUseItem.ClearAllPassiveItemObjectInInventory();
            playerUseItem.CreateAllPassiveItemObjectInInventory();


            UIPlayerInGameInventory.Instance.UpdateInventoryUI();
            OnUpgradeButtonClicked?.Invoke();
        }

    }
}


