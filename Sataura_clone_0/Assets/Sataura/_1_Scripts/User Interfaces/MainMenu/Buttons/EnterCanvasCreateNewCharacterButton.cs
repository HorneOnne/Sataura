using UnityEngine;

namespace Sataura
{
    public class EnterCanvasCreateNewCharacterButton : SatauraButton
    {
        public override void OnClick()
        {
            mainMenuUIManager.SetActiveMainMenuCanvas(false);
            mainMenuUIManager.SetActivePlayerSelectionCanvas(false);
            mainMenuUIManager.SetActiveNotificationCanvas(false);
            mainMenuUIManager.SetActiveCreateNewCharacterCanvas(true);
            
        }
    }
}

