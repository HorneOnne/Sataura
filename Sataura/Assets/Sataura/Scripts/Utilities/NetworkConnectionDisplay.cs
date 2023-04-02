using Unity.Netcode;
using UnityEngine;

namespace Sataura
{
    public class NetworkConnectionDisplay : MonoBehaviour
    {
        private GUIStyle buttonStyle;

        private void OnGUI()
        {
            if (this == null) return;

            buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.fontSize = 12;
            buttonStyle.alignment = TextAnchor.MiddleCenter;

            const int buttonWidth = 80;
            const int buttonHeight = 20;
            const int buttonSpacing = 10;
            const int topMargin = 10;
            const int rightMargin = 10;

            Rect hostButtonRect = new Rect(Screen.width - buttonWidth - rightMargin, topMargin, buttonWidth, buttonHeight);
            if (GUI.Button(hostButtonRect, "Host", buttonStyle))
            {
                StartHost();
            }

            /*Rect serverButtonRect = new Rect(Screen.width - buttonWidth - rightMargin, hostButtonRect.yMax + buttonSpacing, buttonWidth, buttonHeight);
            if (GUI.Button(serverButtonRect, "Server", buttonStyle))
            {
                StartServer();
            }

            Rect clientButtonRect = new Rect(Screen.width - buttonWidth - rightMargin, serverButtonRect.yMax + buttonSpacing, buttonWidth, buttonHeight);
            if (GUI.Button(clientButtonRect, "Client", buttonStyle))
            {
                StartClient();
            }*/
        }

        private void StartHost()
        {
            NetworkManager.Singleton.StartHost();
            Destroy(this);
        }

        private void StartServer()
        {
            NetworkManager.Singleton.StartServer();
            Destroy(this);
        }

        private void StartClient()
        {
            NetworkManager.Singleton.StartClient();
            Destroy(this);
        }
    }

}
