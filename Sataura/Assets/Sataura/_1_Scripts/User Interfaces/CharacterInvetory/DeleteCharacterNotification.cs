using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

namespace Sataura
{
    public class DeleteCharacterNotification : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI _context;
        [SerializeField] private Button _noBtn;
        [SerializeField] private Button _yesBtn;

        [Header("Data")]
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
            this._characterData= characterData;
            _context.text = $"Delete {_characterData.characterName}";
        }

        private void YesBtnHandler()
        {
            SaveManager.Instance.charactersData.Remove(_characterData);           
            /*SaveManager.Instance.Save();
            SaveManager.Instance.Load();*/

            MainMenuUIManager.Instance.playerSelectionCanvas.SetActive(true);
            UIDisplayCharactersSelection.Instance.ClearAll();
            UIDisplayCharactersSelection.Instance.CreateList();         
        }

        private void NoBtnHandler()
        {
            NotificationManager.Instance.CloseCharacterNotification();
            MainMenuUIManager.Instance.playerSelectionCanvas.SetActive(true);
        }

    }
}
