namespace Sataura
{
    public class BackToMainMenuButton : SatauraButton
    {
        public override void OnClick()
        {
            mainMenuUIManager.SetActivePlayerSelectionCanvas(false);

            mainMenuUIManager.SetActiveMainMenuCanvas(true);         
        }
    }
}

