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
        public CharacterMovementData defaultCharacterMovementData;
        public InventoryData defaultInGameInventoryData;
        public InventoryData defaultInventoryData;


        [Space(20)]
        private SaveData saveData;

        [Space(20)]
        public static List<CharacterData> charactersData = new List<CharacterData>();
        public static int selectionCharacterDataIndex;

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }

        private void Start()
        {
            Load();
        }

        public void Save()
        {
            List<CharacterDataStruct> characterDataStructs = new List<CharacterDataStruct>();
            for (int i = 0; i < charactersData.Count; i++)
            {
                characterDataStructs.Add(new CharacterDataStruct
                {
                    characterName = charactersData[i].characterName,
                    characterMovementData = new CharacterMovementDataStruct(charactersData[i].characterMovementData),
                    ingameInventoryData = new InventoryStruct(charactersData[i].ingameInventoryData),
                    playerInventoryData = new InventoryStruct(charactersData[i].playerInventoryData),
                });
            }

            saveData = new SaveData(characterDataStructs);
            saveData.SaveAllData();
        }

        public void Load()
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
                characterData.ingameInventoryData = Utilities.ConvertInventoryDataStructToInventoryData(accountData.charactersDataStruct[i].ingameInventoryData);
                characterData.playerInventoryData = Utilities.ConvertInventoryDataStructToInventoryData(accountData.charactersDataStruct[i].playerInventoryData);
                characterData.currencyString = accountData.charactersDataStruct[i].currencyString;

                charactersData.Add(characterData);
            }

        }



        private void OnApplicationQuit()
        {
            Debug.Log("OnApplicationQuit");
            if (GameDataManager.Instance.singleModePlayer == null)
                return;


            if (GameDataManager.Instance.singleModePlayer.GetComponent<MainMenuPlayer>() != null)
                if (GameDataManager.Instance.singleModePlayer.GetComponent<MainMenuPlayer>().characterData != null)
                    charactersData[selectionCharacterDataIndex] = Instantiate(GameDataManager.Instance.singleModePlayer.GetComponent<MainMenuPlayer>().characterData);

            if (GameDataManager.Instance.singleModePlayer.GetComponent<Player>() != null)
                if (GameDataManager.Instance.singleModePlayer.GetComponent<Player>().characterData != null)
                    charactersData[selectionCharacterDataIndex] = Instantiate(GameDataManager.Instance.singleModePlayer.GetComponent<Player>().characterData);

            //Debug.Log($"OnApplicationQuit: {charactersData.Count} \t {charactersData[0] == null}");

            Save();
        }

    }
}
