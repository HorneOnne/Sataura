using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace Sataura
{
    [System.Serializable]
    public class SaveData
    {
        public string path = Application.dataPath + "/Sataura/Saves/PlayerData";

        //private List<CharacterData> charactersData;

        private CharacterData characterData;
        //private CharacterDataStruct charactersDataStruct;


        public SaveData(CharacterData characterData)
        {
            this.characterData = characterData;
        }



        public void SaveAllData()
        {
            Save(characterData.characterMovementData, "characterMovementData");
            Save(characterData.ingameInventoryData, "inGameInventoryData");
            Save(characterData.playerInventoryData, "playerInventoryData");
        }

        public void LoadAllData()
        {
            string jsonString = LoadJsonString("inGameInventoryData");
            JsonUtility.FromJsonOverwrite(jsonString, characterData.ingameInventoryData);

            jsonString = "";
            jsonString = LoadJsonString("playerInventoryData");
            JsonUtility.FromJsonOverwrite(jsonString, characterData.playerInventoryData);

            jsonString = "";
            jsonString = LoadJsonString("characterMovementData");
            JsonUtility.FromJsonOverwrite(jsonString, characterData.characterMovementData);
        }


        /*public CharacterData GetCharacterData()
        {
            characterData = new CharacterData();

            characterData.characterName = charactersDataStruct.characterName;
            characterData.characterMovementData = charactersDataStruct.characterMovementData;
            characterData.characterName = charactersDataStruct.characterName;

            return characterData;
        }*/



        #region Private Save/Load methods
        private void Save<T>(T objectToSave, string key)
        {
            Directory.CreateDirectory(path);
            string jsonString = JsonUtility.ToJson(objectToSave);
            using (StreamWriter sw = new StreamWriter($"{path}{key}.json"))
            {
                sw.Write(jsonString);
            }

            Debug.Log($"Saved at path: {path}{key}.json");
        }

        private T Load<T>(string key)
        {
            T returnValue = default(T);
            if (File.Exists($"{path}{key}.json"))
            {
                string jsonString = "";
                // LOAD DATA
                using (StreamReader sr = new StreamReader($"{path}{key}.json"))
                {
                    jsonString = sr.ReadToEnd();
                    returnValue = JsonUtility.FromJson<T>(jsonString);
                    Debug.Log("Loaded.");
                }
            }
            else
            {
                Debug.Log("NOT FOUND FILE.");
            }
            return returnValue;
        }

        private string LoadJsonString(string key)
        {
            string returnString = "";
            if (File.Exists($"{path}{key}.json"))
            {
                string jsonString = "";
                // LOAD DATA
                using (StreamReader sr = new StreamReader($"{path}{key}.json"))
                {
                    jsonString = sr.ReadToEnd();
                    Debug.Log("Loaded.");
                }
                returnString = jsonString;
            }
            else
            {
                Debug.Log("NOT FOUND FILE.");
            }
            return returnString;
        }
        #endregion

    }
}




