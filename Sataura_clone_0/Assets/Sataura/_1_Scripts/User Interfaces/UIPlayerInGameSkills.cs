using System.Collections.Generic;
using UnityEngine;

namespace Sataura
{
    public partial class UIPlayerInGameSkills : Singleton<UIPlayerInGameSkills>
    {
        [Header("Runtime References")]
        [SerializeField] private Player player;
        private PlayerInGameSkills playerInGameSkills;


        [Header("References")]
        public List<GameObject> weaponSlotList;
        public List<GameObject> accessorySlotList;
        



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
            playerInGameSkills = player.playerIngameSkills;

            // Update Inventory UI at the first time when start game.
            UpdateUI();
        }

        public void SetPlayer(GameObject playerObject)
        {
            this.player = playerObject.GetComponent<Player>();
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
            uiSlot.SetData(playerInGameSkills.weaponsData.itemSlots[index]);
        }

        public void UpdateUIAccessoryAt(int index)
        {
            UIItemSlot uiSlot = accessorySlotList[index].GetComponent<UIItemSlot>();
            uiSlot.SetData(playerInGameSkills.accessoriesData.itemSlots[index]);
        }
    }
}


