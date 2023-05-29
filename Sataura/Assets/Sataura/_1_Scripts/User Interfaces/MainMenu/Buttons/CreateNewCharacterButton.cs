using UnityEditor;
using UnityEngine;

namespace Sataura
{
    public class CreateNewCharacterButton : SatauraButton
    {

        public override void OnClick()
        {
            CreateNewCharacter();


            mainMenuUIManager.SetActiveMainMenuCanvas(false);
            mainMenuUIManager.SetActiveCreateNewCharacterCanvas(false);
            mainMenuUIManager.SetActivePlayerSelectionCanvas(true);
        }


        private void CreateNewCharacter()
        {
            CharacterData characterData = ScriptableObject.CreateInstance<CharacterData>();
            characterData.name = $"{UICreateNewCharacterManager.Instance.CharacterName}_characterdata";
            characterData.characterName = UICreateNewCharacterManager.Instance.CharacterName;

        
            characterData.characterMovementData = CreateCharacterMovementData();
            characterData.playerInventoryData = CreateInventoryData();
            characterData.weaponsData = CreateWeaponsData();
            characterData.accessoriesData = CreateAccessoriesData();
            

            SaveManager.Instance.charactersData.Add(characterData);
        }

        private CharacterMovementData CreateCharacterMovementData()
        {
            CharacterMovementData characterMovementData = Instantiate(SaveManager.Instance.defaultCharacterData.characterMovementData);
            characterMovementData.name = $"{UICreateNewCharacterManager.Instance.CharacterName}_movementData";

            return characterMovementData;
        }

        private InventoryData CreateWeaponsData()
        {
            InventoryData weaponData = Instantiate(SaveManager.Instance.defaultCharacterData.weaponsData);
            weaponData.name = $"{UICreateNewCharacterManager.Instance.CharacterName}_weaponsData";

            return weaponData;
        }

        private InventoryData CreateAccessoriesData()
        {
            InventoryData accessoriesData = Instantiate(SaveManager.Instance.defaultCharacterData.accessoriesData);
            accessoriesData.name = $"{UICreateNewCharacterManager.Instance.CharacterName}_accessoriesData";

            return accessoriesData;
        }

        private InventoryData CreateInventoryData()
        {
            InventoryData inventoryData = Instantiate(SaveManager.Instance.defaultCharacterData.playerInventoryData);
            inventoryData.name = $"{UICreateNewCharacterManager.Instance.CharacterName}_inventoryData";

            return inventoryData;
        }
    }
}

