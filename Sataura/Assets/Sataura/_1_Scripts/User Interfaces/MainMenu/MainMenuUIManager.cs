using Unity.Netcode;
using UnityEngine;

namespace Sataura
{
    public class MainMenuUIManager : MonoBehaviour
    {
        public static MainMenuUIManager Instance { get; private set; }  
        public UIMainMenu mainMenu;

        public UISinglelayerCharactersSelection singleplayerCharacterSelection;
        public UIMultiplayerCharactersSelection multiplayerCharacterSelection;
        public UIMultiplayerSelection uiMultiplayerSelection;
        public UICreateNewSingleplayerCharacter createNewSingleplayerCharacter;
        public UICreateNewMultiplayerCharacter createNewMultiplayerCharacter;
        public UIDeleteSingleplayerCharacterNotification uiDeleteSingleplayerCharacterNotification;
        public UIDeleteMultiplayerCharacterNotification uiDeleteMultiplayerCharacterNotification;
        public UIUnityRelay uiUnityRelay;

        [Header("Multiplayer")]
        public UIEnterServerIP uiEnterServerIP;
        public UIEnterServerPort uiEnterServerPort;

        [Header("Loading")]
        public UILoadingToServer uiLoadingToServer;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            CloseAll();
            DisplayMainMenu(true);
        }

        public void CloseAll()
        {
            DisplaySingleplayerCharacterSelection(false);
            DisplayMultiplayerCharacterSelection(false);
            DisplayCreateNewSingleplayerCharacter(false);
            DisplayCreateNewMultiplayerCharacter(false);
            DisplayMultiplayerSelection(false);
            DisplayDeleteSingleplayerCharacterNotification(false);
            DisplayDeleteMultiplayerCharacterNotification(false);
            DisplayUIEnterServerIP(false);
            DisplayMainMenu(false);
            DisplayUIEnterServerPort(false);
            DisplayUILoadingToServer(false);
            DisplayUIUnityRelay(false);
        }

        public void DisplayMainMenu(bool isActive)
        {
            mainMenu.DisplayCanvas(isActive);
        }


        public void DisplayUIUnityRelay(bool isActive)
        {
            uiUnityRelay.DisplayCanvas(isActive);
        }

        public void DisplaySingleplayerCharacterSelection(bool isActive)
        {
            singleplayerCharacterSelection.DisplayCanvas(isActive);
        }

        public void DisplayMultiplayerCharacterSelection(bool isActive)
        {
            multiplayerCharacterSelection.DisplayCanvas(isActive);
        }


        public void DisplayCreateNewSingleplayerCharacter(bool isActive)
        {
            createNewSingleplayerCharacter.DisplayCanvas(isActive);
        }

        public void DisplayCreateNewMultiplayerCharacter(bool isActive)
        {
            createNewMultiplayerCharacter.DisplayCanvas(isActive);
        }


        public void DisplayMultiplayerSelection(bool isActive)
        {
            uiMultiplayerSelection.DisplayCanvas(isActive);
        }

        public void DisplayDeleteSingleplayerCharacterNotification(bool isActive)
        {
            uiDeleteSingleplayerCharacterNotification.DisplayCanvas(isActive);
        }

        public void DisplayDeleteMultiplayerCharacterNotification(bool isActive)
        {
            uiDeleteMultiplayerCharacterNotification.DisplayCanvas(isActive);
        }
        public void DisplayUIEnterServerIP(bool isActive)
        {
            uiEnterServerIP.DisplayCanvas(isActive);
        }

        public void DisplayUIEnterServerPort(bool isActive)
        {
            uiEnterServerPort.DisplayCanvas(isActive);
        }


        public void DisplayUILoadingToServer(bool isActive)
        {
            uiLoadingToServer.DisplayCanvas(isActive);
        }

        public void BackToMainMenu()
        {
            // Toggle UI
            CloseAll();
            DisplayMainMenu(true);
        }  
    }

}

