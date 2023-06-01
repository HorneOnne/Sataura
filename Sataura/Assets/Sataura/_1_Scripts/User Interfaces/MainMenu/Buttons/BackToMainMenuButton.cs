namespace Sataura
{
    public class BackToMainMenuButton : SatauraButton
    {
        public override void OnClick()
        {
            MainMenuUIManager.Instance.CloseAll();
            MainMenuUIManager.Instance.BackToMainMenu();
        }
    }

}

