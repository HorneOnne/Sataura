using Unity.Netcode;

namespace Sataura
{
    public class SinglePlayerButton : SatauraButton
    {
        public override void OnClick()
        {          
            NetworkManager.Singleton.StartHost();

            mainMenuUIManager.SetActiveMainMenuCanvas(false);
            mainMenuUIManager.SetActivePlayerSelectionCanvas(true);
        }
    }
}

