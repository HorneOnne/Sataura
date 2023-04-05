namespace Sataura
{
    public class BackToPlayerSelectionButton : SatauraButton
    {
        public override void OnClick()
        {
            mainMenuUIManager.SetActiveMainMenuCanvas(false);
            mainMenuUIManager.SetActiveCreateNewCharacterCanvas(false);

            mainMenuUIManager.SetActivePlayerSelectionCanvas(true);

           
        }
    }
}

