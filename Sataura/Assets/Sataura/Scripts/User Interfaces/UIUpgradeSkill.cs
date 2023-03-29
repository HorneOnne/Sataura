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



        public void SetData(ItemData _itemData)
        {
            this.itemData = _itemData;
        }

        public void UpdateData()
        {
            skillIcon.sprite = itemData.icon;
            skillNameText.text = itemData.name;
            skillDescText.text = itemData.description;
            levelText.text = $"level: {itemData.currentLevel}";
        }

        public void UpdateData(ItemData _itemData)
        {
            this.itemData = _itemData;

            skillIcon.sprite = itemData.icon;
            skillNameText.text = itemData.name;
            skillDescText.text = itemData.description;
            levelText.text = $"level: {itemData.currentLevel}";
        }

        private void OnEnable()
        {
            UpdateData();
        }


        public void OnUIUpgradeSkillButtonClicked()
        {
            Time.timeScale = 1.0f;
            UIManager.Instance.CloseUpgradeItemSkillPanel();


            OnUpgradeButtonClicked?.Invoke();



            var playerInGameInventory = GameDataManager.Instance.players[0].PlayerInGameInventory;
            bool b = playerInGameInventory.HasBaseItem(itemData);

            if(b)
            {
                //playerInGameInventory.inGameInventory[i] = new ItemSlot(inventory[i].ItemData.upgradeRecipe.outputItemSlot.itemData, 1);
                int index = playerInGameInventory.FindBaseItemIndex(itemData);
                var currentItemData = playerInGameInventory.inGameInventory[index].ItemData;
                var upgradeVersionItemData = playerInGameInventory.GetUpgradeVersionOfItem(currentItemData);

                if (upgradeVersionItemData != null)
                {
                    playerInGameInventory.inGameInventory[index] = new ItemSlot(upgradeVersionItemData, 1);
                    UIPlayerInGameInventory.Instance.UpdateInventoryUI();
                }
                else
                {
                    Debug.Log("Item has max level.");
                }
                
            }
            else
            {
                Debug.Log("Not have this item in ingame Inventory!!!!!!!");
            }           
        }

    }
}


