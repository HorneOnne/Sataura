using TMPro;
using UnityEngine;
using System;

namespace Sataura
{
    public class UIEnterServerPort : SatauraCanvas
    {
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private SatauraButton acceptBtn;
        [SerializeField] private SatauraButton backBtn;

        private void Start()
        {
            acceptBtn.Button.onClick.AddListener(() =>
            {
                GameDataManager.Instance.mainMenuInformation.SetServerPort(_inputField.text);

                MainMenuInfomation mainmenuInformation = GameDataManager.Instance.mainMenuInformation;
                string serverIPv4 = mainmenuInformation.IPv4;
                ushort serverPort = Convert.ToUInt16(mainmenuInformation.Port);

                HandleSetNetworkTransport(serverIPv4, serverPort);

                MainMenuUIManager.Instance.CloseAll();
                MainMenuUIManager.Instance.DisplayUILoadingToServer(true);
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

        private void HandleSetNetworkTransport(string ipv4, ushort port)
        {
            MainMenuInfomation mainmenuInformation = GameDataManager.Instance.mainMenuInformation;
            mainmenuInformation.UnityTransport.SetConnectionData(ipv4, port);
        }
    }

}
