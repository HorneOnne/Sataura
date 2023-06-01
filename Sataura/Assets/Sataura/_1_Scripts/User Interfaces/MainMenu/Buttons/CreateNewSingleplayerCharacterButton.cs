using System;
using UnityEditor;
using UnityEngine;

namespace Sataura
{
    public class CreateNewSingleplayerCharacterButton : SatauraButton
    {
        public override void OnClick()
        {        
            CreateNewCharacter();

            MainMenuUIManager.Instance.CloseAll();
            MainMenuUIManager.Instance.DisplaySingleplayerCharacterSelection(true);
        }


        private void CreateNewCharacter()
        {           
            CharacterData characterData = ScriptableObject.CreateInstance<CharacterData>();
            string characterName = MainMenuUIManager.Instance.createNewSingleplayerCharacter.CharacterName;
            characterData.name = $"{characterName}_characterdata";
            characterData.dateCreated = DateTime.Now.ToString("dd-MM-yyyy");
            characterData.characterName = characterName;
   
            characterData.characterMovementData = CreateCharacterMovementData(characterData.characterName);
            characterData.playerInventoryData = CreateInventoryData(characterData.characterName);
            characterData.weaponsData = CreateWeaponsData(characterData.characterName);
            characterData.accessoriesData = CreateAccessoriesData(characterData.characterName);
            

            SaveManager.Instance.charactersData.Add(characterData);
        }

        private CharacterMovementData CreateCharacterMovementData(string characterName)
        {
            CharacterMovementData characterMovementData = Instantiate(SaveManager.Instance.defaultCharacterData.characterMovementData);
            characterMovementData.name = $"{characterName}_movementData";

            return characterMovementData;
        }

        private InventoryData CreateWeaponsData(string characterName)
        {
            InventoryData weaponData = Instantiate(SaveManager.Instance.defaultCharacterData.weaponsData);
            weaponData.name = $"{characterName}_weaponsData";

            return weaponData;
        }

        private InventoryData CreateAccessoriesData(string characterName)
        {
            InventoryData accessoriesData = Instantiate(SaveManager.Instance.defaultCharacterData.accessoriesData);
            accessoriesData.name = $"{characterName}_accessoriesData";

            return accessoriesData;
        }

        private InventoryData CreateInventoryData(string characterName)
        {
            InventoryData inventoryData = Instantiate(SaveManager.Instance.defaultCharacterData.playerInventoryData);
            inventoryData.name = $"{characterName}_inventoryData";

            return inventoryData;
        }
    }
}

