using System.Collections.Generic;
using UnityEngine;

namespace Sataura
{
    public partial class UIPlayerInGameInventory : Singleton<UIPlayerInGameInventory>
    {
        public PlayerType playerType;

        [Header("Runtime References")]
        [SerializeField] private Player player;
        [SerializeField] private ItemSelectionPlayer itemSelectionPlayer;
        private PlayerInGameInventory playerInGameInventory;


        [Header("References")]
        public List<GameObject> weaponSlotList;
        public List<GameObject> accessorySlotList;
        


        [Header("INVENTORY SETTINGS")]
        [SerializeField] private float pressIntervalTime = 1.0f;



        private void OnEnable()
        {
            EventManager.OnPlayerInventoryUpdated += UpdateUI;
        }

        

        private void OnDisable()
        {
            EventManager.OnPlayerInventoryUpdated -= UpdateUI;
        }

    
        public void LoadReferences()
        {
            if (playerType == PlayerType.IngamePlayer)
            {
                playerInGameInventory = player.PlayerInGameInventory;
            }
            else if(playerType == PlayerType.ItemSelectionPlayer)
            {
                playerInGameInventory = itemSelectionPlayer.PlayerInGameInventory;
            }


            // Update Inventory UI at the first time when start game.
            UpdateUI();
        }

        public void SetPlayer(GameObject playerObject)
        {
            if(playerType == PlayerType.IngamePlayer)
            {
                this.player = playerObject.GetComponent<Player>();
            }
                

            if (playerType == PlayerType.ItemSelectionPlayer)
                this.itemSelectionPlayer = playerObject.GetComponent<ItemSelectionPlayer>();
        }

  
        public void UpdateUI()
        {
            for (int i = 0; i < weaponSlotList.Count; i++)
            {
                UpdateUIWeaponAt(i);
            }

            for (int i = 0; i < accessorySlotList.Count; i++)
            {
                UpdateUIAccessoryAt(i);
            }
        }

        public void UpdateUIWeaponAt(int index)
        {
            UIItemSlot uiSlot = weaponSlotList[index].GetComponent<UIItemSlot>();
            uiSlot.SetData(playerInGameInventory.weapons[index]);
        }

        public void UpdateUIAccessoryAt(int index)
        {
            UIItemSlot uiSlot = accessorySlotList[index].GetComponent<UIItemSlot>();
            uiSlot.SetData(playerInGameInventory.accessories[index]);
        }
    }
}


