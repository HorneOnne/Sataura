using UnityEngine;
using System.Numerics;
using System;

namespace Sataura
{
    [CreateAssetMenu(fileName = "CharacterData", menuName = "Sataura/Player/CharacterData")]
    [System.Serializable]
    public class CharacterData : ScriptableObject
    {
        [Header("Character Data")]
        public string characterName;
        public CharacterMovementData characterMovementData;

        [Header("Inventory Data")]
        public InventoryData playerInventoryData;

        [Header("Skills")]
        public InventoryData weaponsData;
        public InventoryData accessoriesData;

        [Header("Default Character Statistics")]
        public int _defaultMaxHealth = 100;
        public float _defaultRecovery = 0;
        public int _defaultArmor = 0;
        public int _defaultMoveSpeed = 20;
        public int _defaultJumpForce = 30;
        public float _defaultAttackSpeed = 1;
        public int _defaultDuration = 0;
        public int _defaultArea = 0;
        public int _defaultCooldown = 0;
        public float _defaultMagnet = 0;
        public int _defaultRevival = 0;

        [Header("Current Character Statistics")]
        public int _currentMaxHealth;
        public float _currentRecovery;
        public int _currentArmor;
        public int _currentMoveSpeed;
        public int _currentJumpForce;
        public float _currentAttackSpeed;
        public int _currentDuration;
        public int _currentArea;
        public int _currentCooldown;
        public float _currentMagnet;
        public int _currentRevival;


        [Header("Equipment")]
        public int helmetDataID = -1;
        public int chestplateDataID = -1;
        public int leggingDataID = -1;
        public int bootsDataID = -1;
        public int hookDataID = -1;
        public int accessoryDataID = -1;

        [Header("Currency")]
        public string currencyString;
        public BigInteger currency;


        private void Awake()
        {
            _currentMaxHealth = _defaultMaxHealth;
            _currentRecovery = _defaultRecovery;
            _currentArmor = _defaultArmor;
            _currentMoveSpeed = _defaultMoveSpeed;
            _currentJumpForce = _defaultJumpForce;
            _currentAttackSpeed = _defaultAttackSpeed;
            _currentDuration = _defaultDuration;
            _currentArea = _defaultArea;
            _currentCooldown = _defaultCooldown;
            _currentMagnet = _defaultMagnet;
            _currentRevival = _defaultRevival;
        }

        public override bool Equals(object other)
        {
            if (other == null || !(other is CharacterData))
            {
                return false;
            }

            CharacterData otherCharacterData = (CharacterData)other;

            // Compare all relevant fields for equality
            return characterName == otherCharacterData.characterName &&
                   characterMovementData == otherCharacterData.characterMovementData &&
                   playerInventoryData == otherCharacterData.playerInventoryData &&
                   weaponsData == otherCharacterData.weaponsData &&
                   accessoriesData == otherCharacterData.accessoriesData &&
                   currencyString == otherCharacterData.currencyString &&
                   currency == otherCharacterData.currency;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(characterName, characterMovementData, playerInventoryData, weaponsData, accessoriesData, currencyString);
        }
    }
}