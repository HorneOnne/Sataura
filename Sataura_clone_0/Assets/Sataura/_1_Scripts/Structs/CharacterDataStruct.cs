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

        public int helmetDataID;
        public int chestplateDataID;
        public int leggingDataID;
        public int bootsDataID;
        public int hookDataID;
        public int accessoryDataID;

        public string currencyString;

        public string dateCreated;
    }
}
