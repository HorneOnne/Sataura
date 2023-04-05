using System.Collections.Generic;
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
        public CharacterData characterData;

        [Space(20)]
        private SaveData saveData;


        public void Save()
        {
            saveData = new SaveData(characterData);
            saveData.SaveAllData();
        }

        public void Load()
        {
            saveData = new SaveData(characterData);
            saveData.LoadAllData();   
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.S))
            {
                Save();
            }

            if(Input.GetKeyDown(KeyCode.L))
            {
                Load();
            }
        }

 
    }
}
