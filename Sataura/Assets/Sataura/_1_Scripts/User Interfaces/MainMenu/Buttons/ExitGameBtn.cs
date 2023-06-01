namespace Sataura
{
    public class ExitGameBtn : SatauraButton
    {

        public override void OnClick()
        {
            base.OnClick();
            ExitGame();
        }
        public void ExitGame()
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

