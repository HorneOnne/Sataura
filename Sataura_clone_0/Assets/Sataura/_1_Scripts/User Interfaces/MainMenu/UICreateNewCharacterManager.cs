using UnityEngine;
using TMPro;

namespace Sataura
{
    public class UICreateNewCharacterManager : Singleton<UICreateNewCharacterManager>
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

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.J))
            {
                Debug.Log(nameText.text);
            }
        }
    }

}
