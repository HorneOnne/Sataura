namespace Sataura
{
    [System.Serializable]
    public struct CharacterDataStruct
    {
        public string characterName;

        public CharacterMovementDataStruct characterMovementData;
        public InventoryStruct playerInventoryData;
        public InventoryStruct ingameInventoryData;

        public string currencyString;
    }
}
