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
        [SerializeField] private TextMeshProUGUI _characterNameText;
        [SerializeField] private Button _selectCharacterBtn;
        [SerializeField] private Button _removeCharacterBtn;
        

        [Header("Runtime References")]
        [SerializeField] private CharacterData _characterData;

        public int index;

        public void SetCharacterData(CharacterData characterData)
        {
            this._characterData = characterData;
            _removeCharacterBtn.onClick.AddListener(RemoveCharacter);
            _selectCharacterBtn.onClick.AddListener(SelectCharacter);
            UpdateUI();
        }

        private void RemoveCharacter()
        {
            MainMenuUIManager.Instance.playerSelectionCanvas.SetActive(false);
            MainMenuUIManager.Instance.SetActiveNotificationCanvas(true);
            NotificationManager.Instance.OpenCharacterNotification(_characterData);
        }


        private void SelectCharacter()
        {
            SaveManager.Instance.selectionCharacterDataIndex = index;
            GameDataManager.Instance.currentPlayer.characterData = SaveManager.Instance.charactersData[index];
            Loader.LoadNetwork(Loader.Scene._1_CharacterInventoryScene);
        }

        private void UpdateUI()
        {
            if (_characterData == null) return;
            _characterNameText.text = _characterData.characterName;
        }

    }
}

