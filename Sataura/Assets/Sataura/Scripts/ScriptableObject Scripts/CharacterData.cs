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

        [Header("InGame")]
        public InventoryData weaponsData;
        public InventoryData accessoriesData;

        [Header("Currency")]
        public string currencyString;
        public BigInteger currency;

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