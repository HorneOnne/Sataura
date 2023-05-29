using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Sataura
{
    /// <summary>
    /// Manages saving and loading game data, including the player inventory and items and chests on the ground.
    /// </summary>
    public class SaveManager : Singleton<SaveManager>
    {
        [Header("Default Character Data")]
        public CharacterData defaultCharacterData;


        [Space(20)]
        private SaveData saveData;

        [Space(20)]
        public List<CharacterData> charactersData = new List<CharacterData>();
        public int selectionCharacterDataIndex;

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }

        private void Start()
        {
            LoadCharacterData();
        }

        public void SaveCharacterData()
        {
            UpdateCurrentCharacterData();

            List<CharacterDataStruct> characterDataStructs = new List<CharacterDataStruct>();
            for (int i = 0; i < charactersData.Count; i++)
            {
                characterDataStructs.Add(new CharacterDataStruct
                {
                    characterName = charactersData[i].characterName,
                    characterMovementData = new CharacterMovementDataStruct(charactersData[i].characterMovementData),
                    weaponsData = new InventoryStruct(charactersData[i].weaponsData),
                    accessoriesData = new InventoryStruct(charactersData[i].accessoriesData),
                    playerInventoryData = new InventoryStruct(charactersData[i].playerInventoryData),
               
                    helmetDataID = GameDataManager.Instance.GetItemID(charactersData[i].helmetData),
                    chestplateDataID = GameDataManager.Instance.GetItemID(charactersData[i].chestplateData),
                    leggingDataID = GameDataManager.Instance.GetItemID(charactersData[i].leggingData),
                    bootsDataID = GameDataManager.Instance.GetItemID(charactersData[i].bootsData),
                    hookDataID = GameDataManager.Instance.GetItemID(charactersData[i].hookData),
                });
            }

            saveData = new SaveData(characterDataStructs);
            saveData.SaveAllData();
        }

        public void LoadCharacterData()
        {          
            List<CharacterDataStruct> characterDataStructs = new List<CharacterDataStruct>();
            saveData = new SaveData(characterDataStructs);
            saveData.LoadAllData();

            var accountData = saveData.GetSaveData();
            if (accountData.charactersDataStruct == null)
                return;

            charactersData = new List<CharacterData>();
            for (int i = 0; i < accountData.charactersDataStruct.Count; i++)
            {
                var characterData = ScriptableObject.CreateInstance<CharacterData>();
                characterData.name = accountData.charactersDataStruct[i].characterName;
                characterData.characterName = accountData.charactersDataStruct[i].characterName;
                characterData.characterMovementData = Utilities.ConvertStructToCharacterMovementData(accountData.charactersDataStruct[i].characterMovementData);
                characterData.weaponsData = Utilities.ConvertInventoryDataStructToInventoryData(accountData.charactersDataStruct[i].weaponsData);
                characterData.accessoriesData = Utilities.ConvertInventoryDataStructToInventoryData(accountData.charactersDataStruct[i].accessoriesData);
                characterData.playerInventoryData = Utilities.ConvertInventoryDataStructToInventoryData(accountData.charactersDataStruct[i].playerInventoryData);

                characterData.helmetData = (HelmetData)GameDataManager.Instance.GetItemData(accountData.charactersDataStruct[i].helmetDataID);
                characterData.chestplateData = (ChestplateData)GameDataManager.Instance.GetItemData(accountData.charactersDataStruct[i].chestplateDataID);
                characterData.leggingData = (LeggingData)GameDataManager.Instance.GetItemData(accountData.charactersDataStruct[i].leggingDataID);
                characterData.bootsData = (BootData)GameDataManager.Instance.GetItemData(accountData.charactersDataStruct[i].bootsDataID);
                characterData.hookData = (HookData)GameDataManager.Instance.GetItemData(accountData.charactersDataStruct[i].hookDataID);

                characterData.currencyString = accountData.charactersDataStruct[i].currencyString;
               
                charactersData.Add(characterData);
            }

        }



        private void OnApplicationQuit()
        {
            Debug.Log("OnApplicationQuit");
            SaveCharacterData();
        }

        private void UpdateCurrentCharacterData()
        {
            if (GameDataManager.Instance.currentPlayer == null)
                return;
            if (GameDataManager.Instance.currentPlayer.characterData == null)
                return;

            charactersData[selectionCharacterDataIndex] = Instantiate(GameDataManager.Instance.currentPlayer.characterData);
        }

    }
}
