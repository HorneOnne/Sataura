using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace Sataura
{
    public class PlayerSkills : NetworkBehaviour
    {
        [SerializeField] private InventoryPlayer _inventoryPlayer;
        private ItemInHand itemInHand;


        [Header("Runtime References")]
        public InventoryData weaponsData;
        public InventoryData accessoriesData;


        #region Properties
        public int Capacity
        {
            get
            {
                if (weaponsData == null)
                    return 0;
                else
                    return weaponsData.itemSlots.Count;
            }
        }

        #endregion


        public override void OnNetworkSpawn()
        {
            if (IsOwner || IsServer)
            {
                itemInHand = _inventoryPlayer.itemInHand;

                StartCoroutine(LoadCharacterData());
            }
        }

        public IEnumerator LoadCharacterData()
        {
            yield return new WaitUntil(() => _inventoryPlayer.characterData != null);

            weaponsData = _inventoryPlayer.characterData.weaponsData;
            accessoriesData = _inventoryPlayer.characterData.accessoriesData;

            for(int i = 0; i < accessoriesData.itemSlots.Count; i++)
            {
                UpdateStatsEquip(accessoriesData.itemSlots[i].ItemData, loadStatsText: false);
            }
                
        }




        public bool AddWeapons(ItemData itemData)
        {
            if (itemData.itemCategory != ItemCategory.Skill_Weapons)
            {
                Debug.LogWarning($"Item {itemData.itemName} is not a weapon.");
                return false;
            }

            bool canAddItem = false;

            for (int i = 0; i < weaponsData.itemSlots.Count; i++)
            {
                if (weaponsData.itemSlots[i].HasItemData() == false)
                {
                    weaponsData.itemSlots[i].AddNewItem(itemData);
                    canAddItem = true;
                    break;
                }
                else
                {
                    if (weaponsData.itemSlots[i].ItemData == itemData)
                    {
                        bool canAdd = weaponsData.itemSlots[i].AddItem();

                        if (canAdd == true)
                        {
                            canAddItem = true;
                            break;
                        }
                    }
                }
            }

            return canAddItem;
        }

        public bool AddAccessories(ItemData itemData)
        {
            if (itemData.itemCategory != ItemCategory.Skill_Accessories)
            {
                Debug.LogWarning($"Item {itemData.itemName} is not a accessory.");
                return false;
            }

            bool canAddItem = false;

            for (int i = 0; i < accessoriesData.itemSlots.Count; i++)
            {
                if (accessoriesData.itemSlots[i].HasItemData() == false)
                {
                    accessoriesData.itemSlots[i].AddNewItem(itemData);
                    canAddItem = true;
                    break;
                }
                else
                {
                    if (accessoriesData.itemSlots[i].ItemData == itemData)
                    {
                        bool canAdd = accessoriesData.itemSlots[i].AddItem();

                        if (canAdd == true)
                        {
                            canAddItem = true;
                            break;
                        }
                    }
                }
            }

            return canAddItem;
        }

  


        /// <summary>
        /// Returns true if the inventory slot at the given index exists, false otherwise
        /// </summary>
        /// <param name="slotIndex">The index of the inventory slot to check for existence</param>
        /// <returns>True if the inventory slot exists, false otherwise</returns>
        public bool HasSlot(int slotIndex)
        {
            try
            {
                weaponsData.itemSlots[slotIndex].HasItemData();
            }
            catch
            {
                return false;
            }

            return true;
        }


        /// <summary>
        /// Returns true if the inventory slot at the given index has an item in it, false otherwise
        /// </summary>
        /// <param name="slotIndex">The index of the inventory slot to check for an item</param>
        /// <returns>True if the inventory slot has an item, false otherwise</returns>
        public bool HasItem(int slotIndex)
        {
            if (HasSlot(slotIndex))
            {
                return weaponsData.itemSlots[slotIndex].HasItemData();
            }
            return false;
        }

        public bool UpdateStatsEquip(ItemData itemData, bool loadStatsText = true)
        {
            if (itemData == null)
                return false;

            bool canEquip = false;
            switch (itemData.itemType)
            {
                case ItemType.VitalityBelt:
                    canEquip = true;

                    int maxHealthPercentIncrease = ((VitalityBeltData)itemData).maxHealthPercentIncrease;
                    int currentMaxHealth = _inventoryPlayer.characterData._currentMaxHealth;
                    int incrementMaxHealth = 0;

                    // Update CharacterData
                    _inventoryPlayer.characterData._currentMaxHealth += (_inventoryPlayer.characterData._currentMaxHealth * maxHealthPercentIncrease / 100);
                    // ===================

                    incrementMaxHealth = _inventoryPlayer.characterData._currentMaxHealth - currentMaxHealth;
                    if (loadStatsText)
                    {
                        FloatingStatisticTextManager.Instance.ShowFloatingStatText("Max Health", incrementMaxHealth);
                    }

                    CharacterStatsManager.Instance.UpdateStatUI(CharacterStats.MaxHealth);
                    break;
                case ItemType.MagnetStone:
                    canEquip = true;

                    int magnetIncrease = ((MagnetStoneData)itemData).lootPercentIncrease;
                    float currentMagnet = _inventoryPlayer.characterData._currentMagnet;
                    float incrementMagnet = 0;

                    // Update CharacterData
                    _inventoryPlayer.characterData._currentMagnet += (_inventoryPlayer.characterData._currentMagnet * magnetIncrease / 100f);
                    // ===================

                    incrementMagnet = _inventoryPlayer.characterData._currentMagnet - currentMagnet;
                    if (loadStatsText)
                    {
                        FloatingStatisticTextManager.Instance.ShowFloatingStatText("Magnet", incrementMagnet);
                    }

                    CharacterStatsManager.Instance.UpdateStatUI(CharacterStats.Magnet);
                    break;
                case ItemType.SwiftStriders:
                    canEquip = true;

                    int moveSpeedPerecntIncrease = ((SwiftStriders)itemData).moveSpeedPercentIncrease;
                    float currentMoveSpeed = _inventoryPlayer.characterData._currentMoveSpeed;
                    float incrementMoveSpeed = 0;

                    int jumpForcePerecntIncrease = ((SwiftStriders)itemData).jumpForcePercentIncrease;
                    float currentJumpForce = _inventoryPlayer.characterData._currentJumpForce;
                    float incrementJumpForce = 0;

                    // Update CharacterData
                    _inventoryPlayer.characterData._currentMoveSpeed += (_inventoryPlayer.characterData._currentMoveSpeed * moveSpeedPerecntIncrease / 100f);
                    _inventoryPlayer.characterData._currentJumpForce += (_inventoryPlayer.characterData._currentJumpForce * jumpForcePerecntIncrease / 100f);
                    // ===================

                    incrementMoveSpeed = _inventoryPlayer.characterData._currentMoveSpeed - currentMoveSpeed;
                    incrementJumpForce = _inventoryPlayer.characterData._currentJumpForce - currentJumpForce;
                    if (loadStatsText)
                    {
                        FloatingStatisticTextManager.Instance.ShowFloatingStatText("Move Speed", incrementMoveSpeed);
                        StartCoroutine(FloatingStatisticTextManager.Instance.ShowFloatingStatTextAfter("Jump Force", incrementJumpForce, 0.5f));
                    }

                    CharacterStatsManager.Instance.UpdateStatUI(CharacterStats.MoveSpeed);
                    CharacterStatsManager.Instance.UpdateStatUI(CharacterStats.JumpForce);
                    break;
                case ItemType.ScopeLens:
                    canEquip = true;

                    int awarePercentIncrease = ((ScopeLens)itemData).awarePercentIncrease;
                    float currentAware = _inventoryPlayer.characterData._currentAware;
                    float incrementAware = 0;

                    // Update CharacterData
                    _inventoryPlayer.characterData._currentAware += (_inventoryPlayer.characterData._currentAware * awarePercentIncrease / 100f);
                    // ===================

                    incrementAware = _inventoryPlayer.characterData._currentAware - currentAware;
                    if (loadStatsText)
                    {
                        FloatingStatisticTextManager.Instance.ShowFloatingStatText("Aware", incrementAware);
                    }

                    CharacterStatsManager.Instance.UpdateStatUI(CharacterStats.Aware);
                    break;
                case ItemType.AmplifyingBand:
                    canEquip = true;

                    int areaPercentIncrease = ((AmplifyingBand)itemData).areaPercentIncrease;
                    float currentArea = _inventoryPlayer.characterData._currentArea;
                    float incrementArea = 0;

                    // Update CharacterData
                    _inventoryPlayer.characterData._currentArea += (_inventoryPlayer.characterData._currentArea * areaPercentIncrease / 100f);
                    // ===================

                    incrementArea = _inventoryPlayer.characterData._currentArea - currentArea;
                    if (loadStatsText)
                    {
                        FloatingStatisticTextManager.Instance.ShowFloatingStatText("Area", incrementArea);
                    }

                    CharacterStatsManager.Instance.UpdateStatUI(CharacterStats.Area);
                    break;
                default:
                    break;
            }

            return canEquip;
        }

        public void UpdateStatsUnequip(ItemData itemData, bool loadStatsText = true)
        {
            if (itemData == null)
                return;

            switch (itemData.itemType)
            {
                case ItemType.VitalityBelt:
                    int maxHealthPercentIncrease = ((VitalityBeltData)itemData).maxHealthPercentIncrease;
                    int currentMaxHealth = _inventoryPlayer.characterData._currentMaxHealth;
                    int decrementMaxHealth = 0;

                    // Update CharacterData
                    _inventoryPlayer.characterData._currentMaxHealth = (int)(currentMaxHealth / (1 + maxHealthPercentIncrease / 100f));
                    // ===================
    
                    decrementMaxHealth = currentMaxHealth - _inventoryPlayer.characterData._currentMaxHealth;
                    if (loadStatsText)
                    {
                        FloatingStatisticTextManager.Instance.ShowFloatingStatText("Max Health", -decrementMaxHealth);
                    }

                    CharacterStatsManager.Instance.UpdateStatUI(CharacterStats.MaxHealth);
                    break;
                case ItemType.MagnetStone:
                    int magnetIncrease = ((MagnetStoneData)itemData).lootPercentIncrease;
                    float currentMagnet = _inventoryPlayer.characterData._currentMagnet;
                    float decrementMagnet = 0;

                    // Update CharacterData
                    _inventoryPlayer.characterData._currentMagnet = (int)(currentMagnet / (1 + magnetIncrease / 100f));
                    // ===================

                    decrementMagnet = currentMagnet - _inventoryPlayer.characterData._currentMagnet;
                    if (loadStatsText)
                    {
                        FloatingStatisticTextManager.Instance.ShowFloatingStatText("Magnet", -decrementMagnet);
                    }

                    CharacterStatsManager.Instance.UpdateStatUI(CharacterStats.Magnet);
                    break;
                case ItemType.SwiftStriders:
                    int moveSpeedPerecntIncrease = ((SwiftStriders)itemData).moveSpeedPercentIncrease;
                    float currentMoveSpeed = _inventoryPlayer.characterData._currentMoveSpeed;
                    float decrementMoveSpeed = 0;

                    int jumpForcePerecntIncrease = ((SwiftStriders)itemData).jumpForcePercentIncrease;
                    float currentJumpForce = _inventoryPlayer.characterData._currentJumpForce;
                    float decrementJumpForce = 0;

                    // Update CharacterData
                    _inventoryPlayer.characterData._currentMoveSpeed = (currentMoveSpeed / (1 + moveSpeedPerecntIncrease / 100f));
                    _inventoryPlayer.characterData._currentJumpForce = (currentJumpForce / (1 + jumpForcePerecntIncrease / 100f));
                    // ===================

                    decrementMoveSpeed = currentMoveSpeed - _inventoryPlayer.characterData._currentMoveSpeed;
                    decrementJumpForce = currentJumpForce - _inventoryPlayer.characterData._currentJumpForce;
                    if (loadStatsText)
                    {
                        FloatingStatisticTextManager.Instance.ShowFloatingStatText("Move Speed", -decrementMoveSpeed);
                        StartCoroutine(FloatingStatisticTextManager.Instance.ShowFloatingStatTextAfter("Jump Force", -decrementJumpForce, 0.5f));
                    }

                    CharacterStatsManager.Instance.UpdateStatUI(CharacterStats.MoveSpeed);
                    CharacterStatsManager.Instance.UpdateStatUI(CharacterStats.JumpForce);
                    break;
                case ItemType.ScopeLens:     
                    int awarePercentIncrease = ((ScopeLens)itemData).awarePercentIncrease;
                    float currentAware = _inventoryPlayer.characterData._currentAware;
                    float decrementAware = 0;

                    // Update CharacterData
                    _inventoryPlayer.characterData._currentAware = (currentAware / (1 + awarePercentIncrease / 100f));
                    // ===================

                    decrementAware = currentAware - _inventoryPlayer.characterData._currentAware;
                    if (loadStatsText)
                    {
                        FloatingStatisticTextManager.Instance.ShowFloatingStatText("Aware", -decrementAware);
                    }

                    CharacterStatsManager.Instance.UpdateStatUI(CharacterStats.Aware);
                    break;
                case ItemType.AmplifyingBand:
                    int areaPercentIncrease = ((AmplifyingBand)itemData).areaPercentIncrease;
                    float currentArea = _inventoryPlayer.characterData._currentArea;
                    float decrementArea = 0;

                    // Update CharacterData
                    _inventoryPlayer.characterData._currentArea = (currentArea  / (1 + areaPercentIncrease / 100f));
                    // ===================

                    decrementArea = currentArea - _inventoryPlayer.characterData._currentArea;
                    if (loadStatsText)
                    {
                        FloatingStatisticTextManager.Instance.ShowFloatingStatText("Area", -decrementArea);
                    }

                    CharacterStatsManager.Instance.UpdateStatUI(CharacterStats.Area);
                    break;
                default:
                    break;
            }
        }
    }

}
