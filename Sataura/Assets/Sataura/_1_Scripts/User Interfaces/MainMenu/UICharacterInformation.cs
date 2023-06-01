using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Sataura
{
    public abstract class UICharacterInformation : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] protected TextMeshProUGUI characterNameText;
        [SerializeField] protected TextMeshProUGUI dateCreatedText;
        [SerializeField] protected Button selectCharacterBtn;
        [SerializeField] protected Button removeCharacterBtn;


        [Header("Runtime References")]
        [SerializeField] protected CharacterData characterData;

        public int index;

        public void SetCharacterData(CharacterData characterData)
        {
            this.characterData = characterData;
            removeCharacterBtn.onClick.AddListener(RemoveCharacter);
            selectCharacterBtn.onClick.AddListener(SelectCharacter);
            UpdateUI();
        }

        public abstract void RemoveCharacter();


        public abstract void SelectCharacter();

        private void UpdateUI()
        {
            if (characterData == null) return;
            characterNameText.text = characterData.characterName;
            dateCreatedText.text = characterData.dateCreated;
        }

    }
}

