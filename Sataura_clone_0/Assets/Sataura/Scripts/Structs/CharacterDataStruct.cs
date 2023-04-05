namespace Sataura
{
    [System.Serializable]
    public struct CharacterDataStruct
    {
        public string characterName;
        public CharacterMovementData characterMovementData;

        public InventoryStruct playerInventoryData;
        public InventoryStruct ingameInventoryData;

        public string currencyString;
    }
}
