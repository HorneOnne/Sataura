using Unity.Netcode;
using UnityEngine;

namespace Sataura
{
    public class ChangeNetworkPlayer : NetworkBehaviour
    {
        [SerializeField] private PlayerType _playerType;

        /*public override void OnNetworkSpawn()
        {
            Debug.Log("ChangeNetworkPlayer called!!!");

            CharacterData characterData = ScriptableObject.CreateInstance<CharacterData>();
            NetworkObject playerNetworkObject = NetworkManager.Singleton.ConnectedClients[NetworkManager.Singleton.LocalClientId].PlayerObject;

            Debug.Log($"playerNetworkObject == null: {playerNetworkObject == null}");
            Player playerObject;
            if(playerNetworkObject.TryGetComponent(out playerObject))
            {
                characterData = playerObject.characterData;        
            }
            else
            {
                Debug.LogError("characterData == null.");
            }

            // Destroy old player object
            playerNetworkObject.Despawn();


            *//*// Create new player object
            var newPlayerNetworkObject = Instantiate(networkPlayerPrefab);
            newPlayerNetworkObject.GetComponent<NetworkObject>().SpawnAsPlayerObject(NetworkManager.Singleton.LocalClientId, false);
            newPlayerNetworkObject.SetActive(true);

            if (newPlayerNetworkObject.GetComponent<MainMenuPlayer>() != null)
            {
                newPlayerNetworkObject.GetComponent<MainMenuPlayer>().characterData = characterData;
            }             
            else if (newPlayerNetworkObject.GetComponent<ItemSelectionPlayer>() != null)
            {
                newPlayerNetworkObject.GetComponent<ItemSelectionPlayer>().characterData = characterData;
                newPlayerNetworkObject.GetComponent<ItemSelectionPlayer>().playerInventory.UpdateCharacterData();   
            }         
            else if (newPlayerNetworkObject.GetComponent<IngamePlayer>() != null)
            {
                newPlayerNetworkObject.GetComponent<IngamePlayer>().characterData = characterData;
                newPlayerNetworkObject.GetComponent<IngamePlayer>().playerIngameSkills.UpdateCharacterData();
            }

            GameDataManager.Instance.currentPlayer = newPlayerNetworkObject;*//*

            Debug.Log($"PlayerType: {_playerType}");
            switch(_playerType)
            {
                case PlayerType.MainMenuPlayer:
                    var mainMenuPlayer = Instantiate(GameDataManager.Instance.mainMenuPlayerPrefab);                  
                    GameDataManager.Instance.mainMenuPlayer = mainMenuPlayer;
                    GameDataManager.Instance.currentPlayer = GameDataManager.Instance.mainMenuPlayer;
                    break;
                case PlayerType.InventoryPlayer:
                    var inventoryPlayer = Instantiate(GameDataManager.Instance.inventoryPlayerPrefab);
                    GameDataManager.Instance.inventoryPlayer = inventoryPlayer;
                    GameDataManager.Instance.currentPlayer = GameDataManager.Instance.inventoryPlayer;
                    inventoryPlayer.playerInventory.UpdateCharacterData();
                    break;
                case PlayerType.IngamePlayer:
                    var ingamePlayer = Instantiate(GameDataManager.Instance.ingamePlayerPrefab);
                    GameDataManager.Instance.ingamePlayer = ingamePlayer;
                    GameDataManager.Instance.currentPlayer = GameDataManager.Instance.ingamePlayer;
                    ingamePlayer.playerIngameSkills.UpdateCharacterData();
                    break;
            }
        }*/

        public override void OnNetworkSpawn()
        {
            CharacterData characterData = null;
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
                case PlayerType.MainMenuPlayer:
                    var mainMenuPlayer = Instantiate(GameDataManager.Instance.mainMenuPlayerPrefab);
                    mainMenuPlayer.NetObject.SpawnAsPlayerObject(NetworkManager.LocalClientId);
                    GameDataManager.Instance.mainMenuPlayer = mainMenuPlayer;
                    GameDataManager.Instance.currentPlayer = GameDataManager.Instance.mainMenuPlayer;
                    break;
                case PlayerType.InventoryPlayer:
                    var inventoryPlayer = Instantiate(GameDataManager.Instance.inventoryPlayerPrefab);
                    inventoryPlayer.NetObject.SpawnAsPlayerObject(NetworkManager.LocalClientId);
                    GameDataManager.Instance.inventoryPlayer = inventoryPlayer;
                    GameDataManager.Instance.currentPlayer = GameDataManager.Instance.inventoryPlayer;
                    GameDataManager.Instance.currentPlayer.characterData = Instantiate(characterData);
                    break;
                case PlayerType.IngamePlayer:
                    var ingamePlayer = Instantiate(GameDataManager.Instance.ingamePlayerPrefab);
                    ingamePlayer.NetObject.SpawnAsPlayerObject(NetworkManager.LocalClientId);
                    GameDataManager.Instance.ingamePlayer = ingamePlayer;
                    GameDataManager.Instance.currentPlayer = GameDataManager.Instance.ingamePlayer;
                    GameDataManager.Instance.currentPlayer.characterData = Instantiate(characterData);
                    break;
            }


           
        }
    }
}