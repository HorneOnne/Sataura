using UnityEngine;

namespace Sataura
{
    public class GotoCreateNewSingleplayerCharacterBtn : SatauraButton
    {
        public override void OnClick()
        {
            MainMenuUIManager.Instance.CloseAll();
            MainMenuUIManager.Instance.DisplayCreateNewSingleplayerCharacter(true);
            
        }
    }
}

