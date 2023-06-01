using UnityEngine;

namespace Sataura
{
    public class BackToMultiplayerCharacterSelectionButton : SatauraButton
    {
        public override void OnClick()
        {
            MainMenuUIManager.Instance.CloseAll();
            MainMenuUIManager.Instance.DisplayMultiplayerCharacterSelection(true);
        }
    }
}

