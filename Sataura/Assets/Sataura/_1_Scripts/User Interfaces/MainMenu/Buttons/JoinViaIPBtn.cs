namespace Sataura
{
    public class JoinViaIPBtn : SatauraButton
    {
        public override void OnClick()
        {
            MainMenuUIManager.Instance.CloseAll();
            MainMenuUIManager.Instance.DisplayUIEnterServerIP(true);
        }
    }
}

