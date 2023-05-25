using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace Sataura
{
    public class PlayerInGameSkills : NetworkBehaviour
    {
        [Header("REFERENCES")]
        [SerializeField] private Player _player;


        [Header("Runtime References")]
        public InventoryData weaponsData;
        public InventoryData accessoriesData;



        public override void OnNetworkSpawn()
        {

        }

        public void UpdateCharacterData()
        {
            weaponsData = Instantiate(_player.characterData.weaponsData);
            accessoriesData = Instantiate(_player.characterData.accessoriesData);
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
            Debug.Log("AddAccessories");
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

        // START Item level skill methods
        // ========================
        public bool HasItemType(ItemType itemType)
        {
            for (int i = 0; i < weaponsData.itemSlots.Count; i++)
            {
                if (weaponsData.itemSlots[i].HasItemData())
                {
                    if (weaponsData.itemSlots[i].ItemData.itemType == itemType)
                    {
                        return true;
                    }
                }
            }

            for (int i = 0; i < accessoriesData.itemSlots.Count; i++)
            {
                if (accessoriesData.itemSlots[i].HasItemData())
                {
                    if (accessoriesData.itemSlots[i].ItemData.itemType == itemType)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public int FindItem(ItemData itemData)
        {
            switch (itemData.itemCategory)
            {
                case ItemCategory.Skill_Weapons:
                    for (int i = 0; i < weaponsData.itemSlots.Count; i++)
                    {
                        if (weaponsData.itemSlots[i].ItemData.itemType == itemData.itemType)
                        {
                            return i;
                        }
                    }
                    break;
                case ItemCategory.Skill_Accessories:
                    for (int i = 0; i < accessoriesData.itemSlots.Count; i++)
                    {
                        if (accessoriesData.itemSlots[i].ItemData.itemType == itemData.itemType)
                        {
                            return i;
                        }
                    }
                    break;
                default:
                    break;
            }

            throw new System.Exception($"Not found base item {itemData} in PlayerplayerIngameInv.inGameInventory.cs.");
        }

        public ItemData GetUpgradeVersionOfItem(ItemData itemData)
        {
            if (itemData.currentLevel < itemData.maxLevel)
            {
                return itemData.upgradeRecipe.outputItemSlot.itemData;
            }
            else
            {
                return null;
            }
        }

        public bool HasEvoOfBaseItem(ItemData baseItem)
        {
            if (baseItem.itemCategory == ItemCategory.Skill_Weapons)
            {
                for (int i = 0; i < weaponsData.itemSlots.Count; i++)
                {
                    if (weaponsData.itemSlots[i].HasItemData() == false)
                        continue;

                    if (weaponsData.itemSlots[i].ItemData.itemType == baseItem.itemType)
                    {
                        if (weaponsData.itemSlots[i].ItemData.Equals(ItemEvolutionManager.Instance.GetEvolutionItem(baseItem)))
                            return true;
                        else
                            return false;
                    }
                }
                return false;
            }
            else
            {
                throw new System.Exception();
            }
        }
        // END Item level skill methods
        // ========================


        public void UpdateStatsEquip(ItemData itemData)
        {
            if (itemData == null)
                return;

            Debug.Log($"UpdateStatsEquip \t{itemData.itemName}");
            switch (itemData.itemType)
            {
                case ItemType.VitalityBelt:
                    int maxHealthPercentIncrease = ((VitalityBeltData)itemData).maxHealthPercentIncrease;
                    // Update CharacterData
                    _player.characterData._currentMaxHealth += (_player.characterData._currentMaxHealth * maxHealthPercentIncrease / 100);
                    // ===================

                    _player.playerCombat.UpdateMaxHealthSlider();
                    break;
                case ItemType.MagnetStone:
                    int magnetIncrease = ((MagnetStoneData)itemData).lootPercentIncrease;

                    // Update CharacterData
                    _player.characterData._currentMagnet += (_player.characterData._currentMagnet * magnetIncrease / 100f);
                    // ===================  
                    break;
                case ItemType.SwiftStriders:
                    int moveSpeedPerecntIncrease = ((SwiftStriders)itemData).moveSpeedPercentIncrease;
                    int jumpForcePerecntIncrease = ((SwiftStriders)itemData).jumpForcePercentIncrease;

                    // Update CharacterData
                    _player.characterData._currentMoveSpeed += (_player.characterData._currentMoveSpeed * moveSpeedPerecntIncrease / 100f);
                    _player.characterData._currentJumpForce += (_player.characterData._currentJumpForce * jumpForcePerecntIncrease / 100f);
                    // ===================

  
                    break;
                case ItemType.ScopeLens:
                    int awarePercentIncrease = ((ScopeLens)itemData).awarePercentIncrease;

                    // Update CharacterData
                    _player.characterData._currentAware += (_player.characterData._currentAware * awarePercentIncrease / 100f);
                    // ===================
                    break;
                case ItemType.AmplifyingBand:
                    int areaPercentIncrease = ((AmplifyingBand)itemData).areaPercentIncrease;

                    // Update CharacterData
                    _player.characterData._currentArea += (_player.characterData._currentArea * areaPercentIncrease / 100f);
                    // ===================
                    break;
                default:
                    break;
            }
        }

        public void UpdateStatsUnequip(ItemData itemData, bool loadStatsText = true)
        {
            if (itemData == null)
                return;

            switch (itemData.itemType)
            {
                case ItemType.VitalityBelt:
                    int maxHealthPercentIncrease = ((VitalityBeltData)itemData).maxHealthPercentIncrease;
                    int currentMaxHealth = _player.characterData._currentMaxHealth;
                    int decrementMaxHealth = 0;

                    // Update CharacterData
                    _player.characterData._currentMaxHealth = (int)(currentMaxHealth / (1 + maxHealthPercentIncrease / 100f));
                    // ===================

                    decrementMaxHealth = currentMaxHealth - _player.characterData._currentMaxHealth;
                    if (loadStatsText)
                    {
                        FloatingStatisticTextManager.Instance.ShowFloatingStatText("Max Health", -decrementMaxHealth);
                    }

                    CharacterStatsManager.Instance.UpdateStatUI(CharacterStats.MaxHealth);
                    break;
                case ItemType.MagnetStone:
                    int magnetIncrease = ((MagnetStoneData)itemData).lootPercentIncrease;
                    float currentMagnet = _player.characterData._currentMagnet;
                    float decrementMagnet = 0;

                    // Update CharacterData
                    _player.characterData._currentMagnet = (int)(currentMagnet / (1 + magnetIncrease / 100f));
                    // ===================

                    decrementMagnet = currentMagnet - _player.characterData._currentMagnet;
                    if (loadStatsText)
                    {
                        FloatingStatisticTextManager.Instance.ShowFloatingStatText("Magnet", -decrementMagnet);
                    }

                    CharacterStatsManager.Instance.UpdateStatUI(CharacterStats.Magnet);
                    break;
                case ItemType.SwiftStriders:
                    int moveSpeedPerecntIncrease = ((SwiftStriders)itemData).moveSpeedPercentIncrease;
                    float currentMoveSpeed = _player.characterData._currentMoveSpeed;
                    float decrementMoveSpeed = 0;

                    int jumpForcePerecntIncrease = ((SwiftStriders)itemData).jumpForcePercentIncrease;
                    float currentJumpForce = _player.characterData._currentJumpForce;
                    float decrementJumpForce = 0;

                    // Update CharacterData
                    _player.characterData._currentMoveSpeed = (currentMoveSpeed / (1 + moveSpeedPerecntIncrease / 100f));
                    _player.characterData._currentJumpForce = (currentJumpForce / (1 + jumpForcePerecntIncrease / 100f));
                    // ===================

                    decrementMoveSpeed = currentMoveSpeed - _player.characterData._currentMoveSpeed;
                    decrementJumpForce = currentJumpForce - _player.characterData._currentJumpForce;
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
                    float currentAware = _player.characterData._currentAware;
                    float decrementAware = 0;

                    // Update CharacterData
                    _player.characterData._currentAware = (currentAware / (1 + awarePercentIncrease / 100f));
                    // ===================

                    decrementAware = currentAware - _player.characterData._currentAware;
                    if (loadStatsText)
                    {
                        FloatingStatisticTextManager.Instance.ShowFloatingStatText("Aware", -decrementAware);
                    }

                    CharacterStatsManager.Instance.UpdateStatUI(CharacterStats.Aware);
                    break;
                case ItemType.AmplifyingBand:
                    int areaPercentIncrease = ((AmplifyingBand)itemData).areaPercentIncrease;
                    float currentArea = _player.characterData._currentArea;
                    float decrementArea = 0;

                    // Update CharacterData
                    _player.characterData._currentArea = (currentArea / (1 + areaPercentIncrease / 100f));
                    // ===================

                    decrementArea = currentArea - _player.characterData._currentArea;
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