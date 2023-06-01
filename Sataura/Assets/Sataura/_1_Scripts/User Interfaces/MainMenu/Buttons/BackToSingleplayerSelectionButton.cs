namespace Sataura
{
    public class BackToSingleplayerSelectionButton : SatauraButton
    {
        public override void OnClick()
        {
            MainMenuUIManager.Instance.CloseAll();
            MainMenuUIManager.Instance.DisplaySingleplayerCharacterSelection(true);       
        }
    }
}

