using Unity.Netcode;
using UnityEngine.SceneManagement;  

namespace Sataura
{
    public class SinglePlayerButton : SatauraButton
    {
        /*public override void OnClick()
        {          
            NetworkManager.Singleton.StartHost();

            mainMenuUIManager.SetActiveMainMenuCanvas(false);
            mainMenuUIManager.SetActivePlayerSelectionCanvas(true);
        }*/

        public override void OnClick()
        {
            SceneManager.LoadScene(Loader.Scene.GameScene.ToString());
        
            /*mainMenuUIManager.SetActiveMainMenuCanvas(false);
            mainMenuUIManager.SetActivePlayerSelectionCanvas(true);*/
        }
    }
}

