namespace Sataura
{
    public class GotoCreateNewMultiplayerCharacterBtn : SatauraButton
    {
        public override void OnClick()
        {
            MainMenuUIManager.Instance.CloseAll();
            MainMenuUIManager.Instance.DisplayCreateNewMultiplayerCharacter(true);

        }
    }
}

