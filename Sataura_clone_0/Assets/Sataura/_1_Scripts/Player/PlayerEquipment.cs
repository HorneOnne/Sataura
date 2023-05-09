using UnityEngine;

namespace Sataura
{
    /// <summary>
    /// Manages the player's equipment, including equipping and unequipping items, and updating the player's appearance based on their equipment.
    /// </summary>
    public class PlayerEquipment : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ItemSelectionPlayer _player;

        [Header("Runtime References")]
        [Header("Equipement Data")]
        public BootData _bootsData;
        public HookData _hookData;
        public HelmetData _helmetData;
        public ChestplateData _chestplateData;
        public LeggingData _leggingData;
        public ItemData _accessoryData;






        public bool TryEquip(ItemData equipmentData, UIEquipSlot.EquipmentSlotType equipmentSlotType)
        {
            bool canEquip = false;

            switch (equipmentSlotType)
            {
                case UIEquipSlot.EquipmentSlotType.Boots:
                    if(equipmentData.itemType == ItemType.Boots)
                    {
                        canEquip = true;
                        _bootsData = (BootData)equipmentData;


                        // Update CharacterData
                        _player.characterData.bootsDataID = GameDataManager.Instance.GetItemID(_bootsData);
                        _player.characterData._currentMoveSpeed += _bootsData.additionMoveSpeed;
                        _player.characterData._currentJumpForce += _bootsData.additionJumpForce;
                        // --------------------



                        // Show floating statistics popup UI
                        StartCoroutine(FloatingStatisticTextManager.Instance.ShowFloatingStatTextAfter("Move Speed", _bootsData.additionMoveSpeed));
                        StartCoroutine(FloatingStatisticTextManager.Instance.ShowFloatingStatTextAfter("Jump Force", _bootsData.additionJumpForce, 0.5f));
                        // ---------------------------------



                        // Update Stats UI
                        CharacterStatsManager.Instance.UpdateStatUI(CharacterStats.MoveSpeed);
                        CharacterStatsManager.Instance.UpdateStatUI(CharacterStats.JumpForce);
                        // ---------------
                    }              
                    break;
                case UIEquipSlot.EquipmentSlotType.Hook:
                    if (equipmentData.itemType == ItemType.Hook)
                    {
                        canEquip = true;
                        _hookData = (HookData)equipmentData;

                        // Update CharacterData
                        _player.characterData.hookDataID = GameDataManager.Instance.GetItemID(_hookData);
                    }                  
                    break;
                case UIEquipSlot.EquipmentSlotType.Helmet:
                    if (equipmentData.itemType == ItemType.Helmet)
                    {
                        canEquip = true;
                        _helmetData = (HelmetData)equipmentData;

                        // Update CharacterData
                        _player.characterData.helmetDataID = GameDataManager.Instance.GetItemID(_helmetData);
                    }
                    break;
                case UIEquipSlot.EquipmentSlotType.Chestplate:
                    if (equipmentData.itemType == ItemType.Chestplate)
                    {
                        canEquip = true;
                        _chestplateData = (ChestplateData)equipmentData;

                        // Update CharacterData
                        _player.characterData.chestplateDataID = GameDataManager.Instance.GetItemID(_chestplateData);
                    }
                    break;
                case UIEquipSlot.EquipmentSlotType.Legging:
                    if (equipmentData.itemType == ItemType.Legging)
                    {
                        canEquip = true;
                        _leggingData = (LeggingData)equipmentData;

                        // Update CharacterData
                        _player.characterData.leggingDataID = GameDataManager.Instance.GetItemID(_leggingData);
                    }
                    break;
                default:
                    break;
            }

            return canEquip;
        }


        public ItemData GetEquipmentData(UIEquipSlot.EquipmentSlotType equipmentSlotType)
        {
            switch (equipmentSlotType)
            {
                case UIEquipSlot.EquipmentSlotType.Boots:
                    return _bootsData;
                case UIEquipSlot.EquipmentSlotType.Hook:
                    return _hookData;
                case UIEquipSlot.EquipmentSlotType.Helmet:
                    return _helmetData;
                case UIEquipSlot.EquipmentSlotType.Chestplate:
                    return _chestplateData;
                case UIEquipSlot.EquipmentSlotType.Legging:
                    return _leggingData;
                case UIEquipSlot.EquipmentSlotType.Accessory:
                    return _accessoryData;
                default:
                    throw new System.Exception();

            }
        }

        public void ClearData(UIEquipSlot.EquipmentSlotType equipmentSlotType)
        {
            switch (equipmentSlotType)
            {
                case UIEquipSlot.EquipmentSlotType.Boots:                   
                    // Update CharacterData
                    _player.characterData._currentMoveSpeed -= _bootsData.additionMoveSpeed;
                    _player.characterData._currentJumpForce -= _bootsData.additionJumpForce;
                    _player.characterData.bootsDataID = -1;
                    // --------------------

                    
                    // Update Stats UI
                    CharacterStatsManager.Instance.UpdateStatUI(CharacterStats.MoveSpeed);
                    CharacterStatsManager.Instance.UpdateStatUI(CharacterStats.JumpForce);
                    // ---------------



                    // Show floating statistics popup UI
                    StartCoroutine(FloatingStatisticTextManager.Instance.ShowFloatingStatTextAfter("Move Speed", -_bootsData.additionMoveSpeed));
                    StartCoroutine(FloatingStatisticTextManager.Instance.ShowFloatingStatTextAfter("Jump Force", -_bootsData.additionJumpForce, 0.5f));
                    // ---------------------------------



                    // Set data to null.
                    _bootsData = null;
                    break;
                case UIEquipSlot.EquipmentSlotType.Hook:
                    _hookData = null;

                    // Update CharacterData
                    _player.characterData.hookDataID = -1;
                    break;
                case UIEquipSlot.EquipmentSlotType.Helmet:
                    _helmetData = null;

                    // Update CharacterData
                    _player.characterData.helmetDataID = -1;
                    break;
                case UIEquipSlot.EquipmentSlotType.Chestplate:
                    _chestplateData = null;

                    // Update CharacterData
                    _player.characterData.chestplateDataID = -1;
                    break;
                case UIEquipSlot.EquipmentSlotType.Legging:
                    _leggingData = null;

                    // Update CharacterData
                    _player.characterData.leggingDataID = -1;
                    break;
                case UIEquipSlot.EquipmentSlotType.Accessory:
                    _accessoryData = null;

                    // Update CharacterData
                    _player.characterData.accessoryDataID = -1;
                    break;
                default:
                    throw new System.Exception();

            }
        }
    }
}