using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Sataura
{
    public class UICharacterInformation : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TextMeshProUGUI characterNameText;
        [SerializeField] private Button selectCharacterBtn;
        [SerializeField] private Button removeCharacterBtn;
        

        [Header("Runtime References")]
        [SerializeField] private CharacterData characterData;

        public int index;

        public void SetCharacterData(CharacterData characterData)
        {
            this.characterData = characterData;
            removeCharacterBtn.onClick.AddListener(RemoveCharacter);
            selectCharacterBtn.onClick.AddListener(SelectCharacter);
            UpdateUI();
        }

        private void RemoveCharacter()
        {
            MainMenuUIManager.Instance.playerSelectionCanvas.SetActive(false);
            MainMenuUIManager.Instance.SetActiveNotificationCanvas(true);
            NotificationManager.Instance.OpenCharacterNotification(characterData);

            /*SaveManager.charactersData.Remove(characterData);           
            SaveManager.Instance.Save();
            SaveManager.Instance.Load();

            UIDisplayCharactersSelection.Instance.ClearAll();
            UIDisplayCharactersSelection.Instance.CreateList();
            Destroy(this.gameObject);*/
        }


        private void SelectCharacter()
        {
            SaveManager.selectionCharacterDataIndex = index;
            GameDataManager.Instance.singleModePlayer.GetComponent<MainMenuPlayer>().characterData = SaveManager.charactersData[index];
            Loader.LoadNetwork(Loader.Scene._1_CharacterInventoryScene);
        }

        private void UpdateUI()
        {
            if (characterData == null) return;

            characterNameText.text = characterData.characterName;
        }

    }
}

