using UnityEngine;
using System.Numerics;

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
        public InventoryData ingameInventoryData;

        [Header("Currency")]
        public string currencyString;
        public BigInteger currency;       
    }
}