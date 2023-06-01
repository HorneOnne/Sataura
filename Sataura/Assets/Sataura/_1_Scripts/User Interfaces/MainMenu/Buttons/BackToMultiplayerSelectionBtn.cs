namespace Sataura
{
    public class BackToMultiplayerSelectionBtn : SatauraButton
    {
        public override void OnClick()
        {
            MainMenuUIManager.Instance.CloseAll();
            MainMenuUIManager.Instance.DisplayMultiplayerSelection(true);
        }
    }
}

