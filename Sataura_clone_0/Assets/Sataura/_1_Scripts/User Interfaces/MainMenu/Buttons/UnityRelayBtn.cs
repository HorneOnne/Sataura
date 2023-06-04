namespace Sataura
{
    public class UnityRelayBtn : SatauraButton
    {
        private void Awake()
        {
            
        }

        public override void OnClick()
        {
            MainMenuUIManager.Instance.CloseAll();
            MainMenuUIManager.Instance.DisplayUIUnityRelay(true);
        }
    }
}

