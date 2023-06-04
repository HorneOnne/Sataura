using UnityEngine;

namespace Sataura
{
    public class UIMainMenu : SatauraCanvas
    {
        [SerializeField] private SatauraButton singleplayerBtn;
        [SerializeField] private SatauraButton multiplayerBtn;
        [SerializeField] private SatauraButton exitBtn;


        private void Start()
        {
            singleplayerBtn.Button.onClick.AddListener(() =>
            {
                MainMenuUIManager.Instance.CloseAll();
                MainMenuUIManager.Instance.DisplaySingleplayerCharacterSelection(true);

                GameDataManager.Instance.mainMenuInformation.SetPlayMode(MainMenuInfomation.PlayMode.SinglePlayer);
            });

            multiplayerBtn.Button.onClick.AddListener(() =>
            {
                MainMenuUIManager.Instance.CloseAll();
                MainMenuUIManager.Instance.DisplayMultiplayerCharacterSelection(true);

                GameDataManager.Instance.mainMenuInformation.SetPlayMode(MainMenuInfomation.PlayMode.MultiPlayer);
            });

            exitBtn.Button.onClick.AddListener(() =>
            {
                ExitGame();
            });
        }

        private void OnDestroy()
        {
            singleplayerBtn.Button.onClick.RemoveAllListeners();
            multiplayerBtn.Button.onClick.RemoveAllListeners();
            exitBtn.Button.onClick.RemoveAllListeners();
        }

        private void ExitGame()
        {
            // Check if the game is running in the Unity editor
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            // Quit the application in standalone builds
            Application.Quit();
#endif
        }
    }
}

