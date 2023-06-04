using TMPro;
using UnityEngine;

namespace Sataura
{
    public class UIEnterServerIP : SatauraCanvas
    {
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private SatauraButton acceptBtn;
        [SerializeField] private SatauraButton backBtn;

        private void Start()
        {
            acceptBtn.Button.onClick.AddListener(() =>
            {
                GameDataManager.Instance.mainMenuInformation.SetServerIPv4(_inputField.text);

                MainMenuUIManager.Instance.CloseAll();
                MainMenuUIManager.Instance.DisplayUIEnterServerPort(true);
            });

            backBtn.Button.onClick.AddListener(() =>
            {
                MainMenuUIManager.Instance.CloseAll();
                MainMenuUIManager.Instance.DisplayMultiplayerSelection(true);
            });
        }

        private void OnDestroy()
        {
            acceptBtn.Button.onClick.RemoveAllListeners();
            backBtn.Button.onClick.RemoveAllListeners();
        }


        public override void DisplayCanvas(bool isDisplay)
        {
            base.DisplayCanvas(isDisplay);

            if (isDisplay)
            {
                _inputField.Select();
                _inputField.text = "";
            }

        }
    }

}
