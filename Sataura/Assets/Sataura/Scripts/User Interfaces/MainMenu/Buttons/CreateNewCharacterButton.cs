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
            characterData.ingameInventoryData = CreateInGameInventoryData();
            characterData.playerInventoryData = CreateInventoryData();


            //AssetDatabase.CreateAsset(characterData, $"Assets/Sataura/Saves/PlayerData/{characterData.name}.asset");
            //AssetDatabase.SaveAssets();

            SaveManager.charactersData.Add(characterData);
        }

        private CharacterMovementData CreateCharacterMovementData()
        {
            CharacterMovementData characterMovementData = Instantiate(SaveManager.Instance.defaultCharacterMovementData);
            characterMovementData.name = $"{UICreateNewCharacterManager.Instance.CharacterName}_movementData";

            //AssetDatabase.CreateAsset(characterMovementData, $"Assets/Sataura/Saves/PlayerData/{characterMovementData.name}.asset");
            //AssetDatabase.SaveAssets();

            return characterMovementData;
        }

        private InventoryData CreateInGameInventoryData()
        {
            InventoryData ingameInventoryData = Instantiate(SaveManager.Instance.defaultInGameInventoryData);
            ingameInventoryData.name = $"{UICreateNewCharacterManager.Instance.CharacterName}_ingameInventoryData";

            //AssetDatabase.CreateAsset(ingameInventoryData, $"Assets/Sataura/Saves/PlayerData/{ingameInventoryData.name}.asset");
            //AssetDatabase.SaveAssets();

            return ingameInventoryData;
        }

        private InventoryData CreateInventoryData()
        {
            InventoryData inventoryData = Instantiate(SaveManager.Instance.defaultInventoryData);
            inventoryData.name = $"{UICreateNewCharacterManager.Instance.CharacterName}_inventoryData";

            //AssetDatabase.CreateAsset(inventoryData, $"Assets/Sataura/Saves/PlayerData/{inventoryData.name}.asset");
            //AssetDatabase.SaveAssets();

            return inventoryData;
        }
    }
}

