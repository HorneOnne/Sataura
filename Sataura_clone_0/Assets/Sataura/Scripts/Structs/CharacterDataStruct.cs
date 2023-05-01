namespace Sataura
{
    [System.Serializable]
    public struct CharacterDataStruct
    {
        public string characterName;

        public CharacterMovementDataStruct characterMovementData;
        public InventoryStruct playerInventoryData;

        public InventoryStruct weaponsData;
        public InventoryStruct accessoriesData;

        public string currencyString;
    }
}
