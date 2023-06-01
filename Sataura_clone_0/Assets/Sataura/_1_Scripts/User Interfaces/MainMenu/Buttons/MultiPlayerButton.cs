namespace Sataura
{
    public class MultiPlayerButton : SatauraButton
    {
        public override void OnClick()
        {
            MainMenuUIManager.Instance.CloseAll();
            MainMenuUIManager.Instance.DisplayMultiplayerCharacterSelection(true);

            GameDataManager.Instance.mainMenuInformation.SetPlayMode(MainMenuInfomation.PlayMode.MultiPlayer);
        }
    }
}

