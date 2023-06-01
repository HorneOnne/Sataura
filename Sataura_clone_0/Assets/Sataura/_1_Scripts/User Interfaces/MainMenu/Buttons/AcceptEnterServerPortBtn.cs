using UnityEngine;
using TMPro;
using System;

namespace Sataura
{
    public class AcceptEnterServerPortBtn : SatauraButton
    {
        [SerializeField] private TMP_InputField _inputField;


        public override void OnClick()
        {
            GameDataManager.Instance.mainMenuInformation.SetServerPort(_inputField.text);

            MainMenuInfomation mainmenuInformation = GameDataManager.Instance.mainMenuInformation;
            string serverIPv4 = mainmenuInformation.IPv4;
            ushort serverPort = Convert.ToUInt16(mainmenuInformation.Port);

            HandleSetNetworkTransport(serverIPv4, serverPort);

            MainMenuUIManager.Instance.CloseAll();
            MainMenuUIManager.Instance.DisplayUILoadingToServer(true);        
        }


        private void HandleSetNetworkTransport(string ipv4, ushort port)
        {
            MainMenuInfomation mainmenuInformation = GameDataManager.Instance.mainMenuInformation;
            mainmenuInformation.UnityTransport.SetConnectionData(ipv4, port);
        }       
    }
}

