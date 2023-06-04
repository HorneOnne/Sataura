using UnityEngine;
using TMPro;
using System;

namespace Sataura
{
    public class UICreateNewSingleplayerCharacter : SatauraCanvas
    {
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] SatauraButton createBtn;
        [SerializeField] SatauraButton cancelBtn;


        #region Properties
        public string CharacterName { get => nameText.text; }
        #endregion


        private void OnEnable()
        {
            inputField.Select();
            inputField.text = "";
        }

        public override void DisplayCanvas(bool isDisplay)
        {
            base.DisplayCanvas(isDisplay);

            if(isDisplay)
            {
                inputField.Select();
                inputField.text = "";
            }            
        }

        private void Start()
        {
            createBtn.Button.onClick.AddListener(() =>
            {
                CreateNewCharacter();

                MainMenuUIManager.Instance.CloseAll();
                MainMenuUIManager.Instance.DisplaySingleplayerCharacterSelection(true);
            });

            cancelBtn.Button.onClick.AddListener(() =>
            {
                MainMenuUIManager.Instance.CloseAll();
                MainMenuUIManager.Instance.DisplaySingleplayerCharacterSelection(true);
            });
        }

        private void OnDestroy()
        {
            createBtn.Button.onClick.RemoveAllListeners();
            cancelBtn.Button.onClick.RemoveAllListeners();
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
