using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace Sataura
{
    [System.Serializable]
    public class SaveData
    {
        public string path = Application.dataPath + "/UltimateItemSystem/Saves/";
        public PlayerInventoryStruct playerInventoryData;
        public ItemOnGroundSaveData itemOnGroundData;
        public WorldChest worldChest;


        public SaveData(PlayerInventory playerInventory, List<Item> itemsOnGround, List<Chest> chestObjects)
        {
            playerInventoryData = new PlayerInventoryStruct(playerInventory);
            itemOnGroundData = new ItemOnGroundSaveData(itemsOnGround);
            worldChest = new WorldChest(chestObjects);            
        }

        public void SaveAllData()
        {
            Save(playerInventoryData, "playerInventory");
            Save(itemOnGroundData, "itemsOnGround");
            Save(worldChest, "chestInventory");
        }

        public void LoadAllData()
        {
            playerInventoryData = Load<PlayerInventoryStruct>("playerInventory");
            itemOnGroundData = Load<ItemOnGroundSaveData>("itemsOnGround");
            worldChest = Load<WorldChest>("chestInventory");
        }


        public void Save<T>(T objectToSave, string key)
        {
            Directory.CreateDirectory(path);
            string jsonString = JsonUtility.ToJson(objectToSave);
            using (StreamWriter sw = new StreamWriter($"{path}{key}.json"))
            {
                sw.Write(jsonString);
            }

            Debug.Log($"Saved at path: {path}{key}.json");
        }

        public T Load<T>(string key)
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

    }
}




