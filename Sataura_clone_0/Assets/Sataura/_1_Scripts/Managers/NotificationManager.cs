using UnityEngine;

namespace Sataura
{
    public class NotificationManager : Singleton<NotificationManager>
    {
        [SerializeField] private DeleteCharacterNotification _deleteCharacterNotification;

        public void OpenCharacterNotification(CharacterData characterData)
        {
            _deleteCharacterNotification.gameObject.SetActive(true);
            _deleteCharacterNotification.SetCharacterData(characterData);
        }

        public void CloseCharacterNotification()
        {
            _deleteCharacterNotification.gameObject.SetActive(false);
        }



    }
}
