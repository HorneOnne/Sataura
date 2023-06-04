using UnityEngine;
using UnityEngine.UI;

namespace Sataura
{
    public class UIMultiplayerSelection : SatauraCanvas
    {
        [SerializeField] private SatauraButton unityRelayBtn;
        [SerializeField] private SatauraButton joinViaIPBtn;
        [SerializeField] private SatauraButton hostAndPlayBtn;
        [SerializeField] private SatauraButton backBtn;


        private void Start()
        {
            unityRelayBtn.Button.onClick.AddListener(() =>
            {
                MainMenuUIManager.Instance.CloseAll();
                MainMenuUIManager.Instance.DisplayUIUnityRelay(true);
            });

            joinViaIPBtn.Button.onClick.AddListener(() =>
            {
                MainMenuUIManager.Instance.CloseAll();
                MainMenuUIManager.Instance.DisplayUIEnterServerIP(true);
            });

            /*hostAndPlayBtn.Button.onClick.AddListener(() =>
            {
                
            });*/

            backBtn.Button.onClick.AddListener(() =>
            {
                MainMenuUIManager.Instance.CloseAll();
                MainMenuUIManager.Instance.DisplayMultiplayerCharacterSelection(true);
            });
        }

        private void OnDestroy()
        {
            unityRelayBtn.Button.onClick.RemoveAllListeners();

            joinViaIPBtn.Button.onClick.RemoveAllListeners();

            //hostAndPlayBtn.Button.onClick.RemoveAllListeners();

            backBtn.Button.onClick.RemoveAllListeners();
        }
    }

}
