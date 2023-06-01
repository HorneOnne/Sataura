using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Sataura
{
    public class UIDeleteMultiplayerCharacterNotification : SatauraCanvas
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI _context;
        [SerializeField] private Button _noBtn;
        [SerializeField] private Button _yesBtn;

        [Header("Runtime References")]
        [SerializeField] private CharacterData _characterData;


        private void OnEnable()
        {
            _noBtn.onClick.AddListener(NoBtnHandler);
            _yesBtn.onClick.AddListener(YesBtnHandler);
        }



        private void OnDisable()
        {
            _noBtn.onClick.RemoveListener(NoBtnHandler);
            _yesBtn.onClick.RemoveListener(YesBtnHandler);
        }


        public void SetCharacterData(CharacterData characterData)
        {
            this._characterData = characterData;
            _context.text = $"Delete {_characterData.characterName}";
        }

        private void YesBtnHandler()
        {
            SaveManager.Instance.charactersData.Remove(_characterData);

            MainMenuUIManager.Instance.CloseAll();
            MainMenuUIManager.Instance.DisplayMultiplayerCharacterSelection(true);

            MainMenuUIManager.Instance.multiplayerCharacterSelection.ClearAll();
            MainMenuUIManager.Instance.multiplayerCharacterSelection.CreateList();
        }

        private void NoBtnHandler()
        {
            MainMenuUIManager.Instance.CloseAll();
            MainMenuUIManager.Instance.DisplayMultiplayerCharacterSelection(true);
        }

        public void OpenCharacterNotification(CharacterData characterData)
        {
            SetCharacterData(characterData);
        }
    }
}
