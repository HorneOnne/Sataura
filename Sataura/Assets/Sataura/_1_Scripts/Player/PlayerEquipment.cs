using System.Collections;
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


        // Load CharacterData variables
        private bool _isCharacterDataReady = false;
        private float _timeoutDuration = 10.0f;
        private float _timer;

        private void Start()
        {
            StartCoroutine(LoadEquipment());
        }

        private IEnumerator LoadEquipment()
        {
            _timer = 0f;
            yield return new WaitUntil(() => _player.characterData != null);
            _isCharacterDataReady = true;

            TryEquip(_player.characterData.helmetData, UIEquipSlot.EquipmentSlotType.Helmet, loadStatsText: false);
            TryEquip(_player.characterData.chestplateData, UIEquipSlot.EquipmentSlotType.Chestplate, loadStatsText: false);
            TryEquip(_player.characterData.leggingData, UIEquipSlot.EquipmentSlotType.Legging, loadStatsText: false);
            TryEquip(_player.characterData.bootsData, UIEquipSlot.EquipmentSlotType.Boots, loadStatsText: false);
            TryEquip(_player.characterData.hookData, UIEquipSlot.EquipmentSlotType.Hook, loadStatsText: false);

            UIEquipmentManager.Instance.UpdateUI();
        }


        private void Update()
        {
            if (_isCharacterDataReady == false)
            {
                _timer += Time.deltaTime;
                if (_timer >= _timeoutDuration)
                {
                    Debug.LogError("Cannot load character data.");
                    StopCoroutine(LoadEquipment());
                }
            }
        }

        public bool TryEquip(ItemData equipmentData, UIEquipSlot.EquipmentSlotType equipmentSlotType, bool loadStatsText = true)
        {
            if (equipmentData == null)
                return false;

            bool canEquip = false;
            switch (equipmentSlotType)
            {
                case UIEquipSlot.EquipmentSlotType.Boots:
                    if (equipmentData.itemType == ItemType.Boots)
                    {
                        canEquip = true;
                        _bootsData = (BootData)equipmentData;


                        // Update CharacterData
                        _player.characterData.bootsData = _bootsData;
                        /*_player.characterData._currentMoveSpeed += _bootsData.additionMoveSpeed;
                        _player.characterData._currentJumpForce += _bootsData.additionJumpForce;*/
                        // --------------------

                        /*if(loadStatsText)
                        {
                            // Show floating statistics popup UI
                            StartCoroutine(FloatingStatisticTextManager.Instance.ShowFloatingStatTextAfter("Move Speed", _bootsData.additionMoveSpeed));
                            StartCoroutine(FloatingStatisticTextManager.Instance.ShowFloatingStatTextAfter("Jump Force", _bootsData.additionJumpForce, 0.5f));
                            // ---------------------------------
                        }


                        // Update Stats UI
                        CharacterStatsManager.Instance.UpdateStatUI(CharacterStats.MoveSpeed);
                        CharacterStatsManager.Instance.UpdateStatUI(CharacterStats.JumpForce);
                        // ---------------*/
                    }
                    break;
                case UIEquipSlot.EquipmentSlotType.Hook:
                    if (equipmentData.itemType == ItemType.Hook)
                    {
                        canEquip = true;
                        _hookData = (HookData)equipmentData;

                        // Update CharacterData
                        _player.characterData.hookData = _hookData;
                        // --------------------
                    }
                    break;
                case UIEquipSlot.EquipmentSlotType.Helmet:
                    if (equipmentData.itemType == ItemType.Helmet)
                    {
                        canEquip = true;
                        _helmetData = (HelmetData)equipmentData;

                        // Update CharacterData
                        _player.characterData.helmetData = _helmetData;
                        _player.characterData._currentArmor += _helmetData.armor;
                        // --------------------

                        if (loadStatsText)
                        {
                            FloatingStatisticTextManager.Instance.ShowFloatingStatText("Armor", _helmetData.armor);
                        }

                        CharacterStatsManager.Instance.UpdateStatUI(CharacterStats.Armor);
                    }
                    break;
                case UIEquipSlot.EquipmentSlotType.Chestplate:
                    if (equipmentData.itemType == ItemType.Chestplate)
                    {
                        canEquip = true;
                        _chestplateData = (ChestplateData)equipmentData;

                        // Update CharacterData
                        _player.characterData.chestplateData = _chestplateData;
                        _player.characterData._currentArmor += _chestplateData.armor;
                        // --------------------

                        if (loadStatsText)
                        {
                            FloatingStatisticTextManager.Instance.ShowFloatingStatText("Armor", _chestplateData.armor);
                        }

                        CharacterStatsManager.Instance.UpdateStatUI(CharacterStats.Armor);
                    }
                    break;
                case UIEquipSlot.EquipmentSlotType.Legging:
                    if (equipmentData.itemType == ItemType.Legging)
                    {
                        canEquip = true;
                        _leggingData = (LeggingData)equipmentData;

                        // Update CharacterData
                        _player.characterData.leggingData = _leggingData;
                        _player.characterData._currentArmor += _leggingData.armor;
                        // --------------------

                        if (loadStatsText)
                        {
                            FloatingStatisticTextManager.Instance.ShowFloatingStatText("Armor", _leggingData.armor);
                        }

                        CharacterStatsManager.Instance.UpdateStatUI(CharacterStats.Armor);
                    }
                    break;
                default:
                    break;
            }

            return canEquip;
        }



        public void ClearData(UIEquipSlot.EquipmentSlotType equipmentSlotType)
        {
            switch (equipmentSlotType)
            {
                case UIEquipSlot.EquipmentSlotType.Boots:
                    // Update CharacterData
                    /*_player.characterData._currentMoveSpeed -= _bootsData.additionMoveSpeed;
                    _player.characterData._currentJumpForce -= _bootsData.additionJumpForce;*/
                    _player.characterData.bootsData = null;
                    // --------------------

                    /*
                    // Update Stats UI
                    CharacterStatsManager.Instance.UpdateStatUI(CharacterStats.MoveSpeed);
                    CharacterStatsManager.Instance.UpdateStatUI(CharacterStats.JumpForce);
                    // ---------------



                    // Show floating statistics popup UI
                    StartCoroutine(FloatingStatisticTextManager.Instance.ShowFloatingStatTextAfter("Move Speed", -_bootsData.additionMoveSpeed));
                    StartCoroutine(FloatingStatisticTextManager.Instance.ShowFloatingStatTextAfter("Jump Force", -_bootsData.additionJumpForce, 0.5f));
                    // ---------------------------------
                    */

                    // Set data to null.
                    _bootsData = null;
                    break;
                case UIEquipSlot.EquipmentSlotType.Hook:
                    // Update CharacterData
                    _player.characterData.hookData = null;


                    _hookData = null;
                    break;
                case UIEquipSlot.EquipmentSlotType.Helmet:
                    // Update CharacterData
                    _player.characterData._currentArmor -= _helmetData.armor;
                    _player.characterData.helmetData = null;
                    // --------------------

                    FloatingStatisticTextManager.Instance.ShowFloatingStatText("Armor", -_helmetData.armor);
                    CharacterStatsManager.Instance.UpdateStatUI(CharacterStats.Armor);

                    _helmetData = null;
                    break;
                case UIEquipSlot.EquipmentSlotType.Chestplate:
                    // Update CharacterData
                    _player.characterData._currentArmor -= _chestplateData.armor;
                    _player.characterData.chestplateData = null;

                    FloatingStatisticTextManager.Instance.ShowFloatingStatText("Armor", -_chestplateData.armor);
                    CharacterStatsManager.Instance.UpdateStatUI(CharacterStats.Armor);

                    _chestplateData = null;
                    break;
                case UIEquipSlot.EquipmentSlotType.Legging:
                    // Update CharacterData
                    _player.characterData._currentArmor -= _leggingData.armor;
                    _player.characterData.leggingData = null;

                    FloatingStatisticTextManager.Instance.ShowFloatingStatText("Armor", -_leggingData.armor);
                    CharacterStatsManager.Instance.UpdateStatUI(CharacterStats.Armor);

                    _leggingData = null;
                    break;
                case UIEquipSlot.EquipmentSlotType.Accessory:
                    // Update CharacterData

                    break;
                default:
                    throw new System.Exception();

            }
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
    }
}