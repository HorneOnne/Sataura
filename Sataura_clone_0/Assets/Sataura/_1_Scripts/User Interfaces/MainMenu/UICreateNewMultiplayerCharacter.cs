using UnityEngine;
using TMPro;

namespace Sataura
{
    public class UICreateNewMultiplayerCharacter : SatauraCanvas
    {
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private TextMeshProUGUI nameText;

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

            if (isDisplay)
            {
                inputField.Select();
                inputField.text = "";
            }

        }
    }

}
