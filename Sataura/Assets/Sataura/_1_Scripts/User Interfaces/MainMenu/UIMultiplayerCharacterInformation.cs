using UnityEngine;

namespace Sataura
{
    public class UIMultiplayerCharacterInformation : UICharacterInformation
    {
        public override void RemoveCharacter()
        {
            MainMenuUIManager.Instance.CloseAll();
            MainMenuUIManager.Instance.DisplayDeleteMultiplayerCharacterNotification(true);

            MainMenuUIManager.Instance.uiDeleteMultiplayerCharacterNotification.OpenCharacterNotification(characterData);
        }


        public override void SelectCharacter()
        {
            SaveManager.Instance.selectionCharacterDataIndex = index;
            GameDataManager.Instance.mainMenuInformation.characterData = SaveManager.Instance.charactersData[index];

            MainMenuUIManager.Instance.CloseAll();
            MainMenuUIManager.Instance.DisplayMultiplayerSelection(true);
        }
    }
}

