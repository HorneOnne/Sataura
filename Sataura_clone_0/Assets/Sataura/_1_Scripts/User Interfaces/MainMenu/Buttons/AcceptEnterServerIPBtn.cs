using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using Unity.Netcode;

namespace Sataura
{
    public class AcceptEnterServerIPBtn : SatauraButton
    {
        [SerializeField] private TMP_InputField _inputField;

        public override void OnClick()
        {
            GameDataManager.Instance.mainMenuInformation.SetServerIPv4(_inputField.text);

            MainMenuUIManager.Instance.CloseAll();
            MainMenuUIManager.Instance.DisplayUIEnterServerPort(true);
        }

    }
}

