namespace Sataura
{
    public class UISingleplayerCharacterInformation : UICharacterInformation
    {          
        public override void RemoveCharacter()
        {
            MainMenuUIManager.Instance.CloseAll();
            MainMenuUIManager.Instance.DisplayDeleteSingleplayerCharacterNotification(true);

            MainMenuUIManager.Instance.uiDeleteSingleplayerCharacterNotification.OpenCharacterNotification(characterData);
        }


        public override void SelectCharacter()
        {
            SaveManager.Instance.selectionCharacterDataIndex = index;
            GameDataManager.Instance.mainMenuInformation.characterData = SaveManager.Instance.charactersData[index];

            Loader.Load(Loader.Scene._1_CharacterInventoryScene);
        }
    }
}

