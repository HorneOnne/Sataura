using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;


namespace Sataura
{
    [System.Serializable]
    public class SaveData
    {
        public string path = Application.dataPath + "/Sataura/_5_Saves/PlayerData";


        private AccountDataStruct accountData;

        public SaveData(List<CharacterDataStruct> charactersDataStruct)
        {
            this.accountData.charactersDataStruct = charactersDataStruct;
        }

        public void SaveAllData()
        {   
            Save(accountData, "accountData");
        }

        public void LoadAllData()
        {
            accountData = Load<AccountDataStruct>("accountData");
        }

        public AccountDataStruct GetSaveData()
        {
            return accountData;
        }




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

    [System.Serializable]
    public struct AccountDataStruct
    {
        public List<CharacterDataStruct> charactersDataStruct;
    }
}




