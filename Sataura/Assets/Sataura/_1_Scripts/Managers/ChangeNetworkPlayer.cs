using Unity.Netcode;
using UnityEngine;

namespace Sataura
{
    public class ChangeNetworkPlayer : MonoBehaviour
    {
        [SerializeField] private PlayerType _playerType;

        
        public void Start()
        {
            CharacterData characterData = SaveManager.Instance.charactersData[SaveManager.Instance.selectionCharacterDataIndex];

            Player _currentPlayer = GameDataManager.Instance.currentPlayer;
            if (_currentPlayer != null)
            {
                characterData = Instantiate(_currentPlayer.characterData);
                GameDataManager.Instance.currentPlayer.NetObject.Despawn();
            }
            else
            {
                //Debug.LogWarning("_currentPlayer == null.");
            }

            

            switch (_playerType)
            { 
                case PlayerType.InventoryPlayer:
                    var inventoryPlayer = Instantiate(GameDataManager.Instance.inventoryPlayerPrefab);
                    inventoryPlayer.NetObject.SpawnAsPlayerObject(NetworkManager.Singleton.LocalClientId);
                    GameDataManager.Instance.inventoryPlayer = inventoryPlayer;
                    GameDataManager.Instance.currentPlayer = GameDataManager.Instance.inventoryPlayer;
                    GameDataManager.Instance.currentPlayer.characterData = Instantiate(characterData);
                    break;
                case PlayerType.IngamePlayer:
                    var ingamePlayer = Instantiate(GameDataManager.Instance.ingamePlayerPrefab);
                    ingamePlayer.NetObject.SpawnAsPlayerObject(NetworkManager.Singleton.LocalClientId);
                    GameDataManager.Instance.ingamePlayer = ingamePlayer;
                    GameDataManager.Instance.currentPlayer = GameDataManager.Instance.ingamePlayer;
                    GameDataManager.Instance.currentPlayer.characterData = Instantiate(characterData);
                    break;
            }


           
        }
    }
}