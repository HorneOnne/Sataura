using Unity.Netcode;

namespace Sataura
{
    public class SinglePlayerButton : SatauraButton
    {
        public override void OnClick()
        {
            MainMenuUIManager.Instance.CloseAll();
            MainMenuUIManager.Instance.DisplaySingleplayerCharacterSelection(true);

            GameDataManager.Instance.mainMenuInformation.SetPlayMode(MainMenuInfomation.PlayMode.SinglePlayer);
        }
    }
}

