namespace Sataura
{
    public class SinglePlayerButton : SatauraButton
    {
        public override void OnClick()
        {
            mainMenuUIManager.SetActiveMainMenuCanvas(false);
            mainMenuUIManager.SetActivePlayerSelectionCanvas(true);
        }
    }
}

