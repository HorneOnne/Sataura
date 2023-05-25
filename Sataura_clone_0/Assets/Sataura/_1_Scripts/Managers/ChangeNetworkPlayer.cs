using Unity.Netcode;
using UnityEngine;

namespace Sataura
{
    public class ChangeNetworkPlayer : NetworkBehaviour
    {
        public GameObject networkPlayerPrefab;

        public override void OnNetworkSpawn()
        {
            Debug.Log("ChangeNetworkPlayer called!!!");

            CharacterData characterData = ScriptableObject.CreateInstance<CharacterData>();
            NetworkObject playerNetworkObject = NetworkManager.Singleton.ConnectedClients[NetworkManager.Singleton.LocalClientId].PlayerObject;

            if (playerNetworkObject.GetComponent<MainMenuPlayer>() != null)
                characterData = playerNetworkObject.GetComponent<MainMenuPlayer>().characterData;
            else if (playerNetworkObject.GetComponent<ItemSelectionPlayer>() != null)
                characterData = playerNetworkObject.GetComponent<ItemSelectionPlayer>().characterData;
            else if (playerNetworkObject.GetComponent<Player>() != null)
                characterData = playerNetworkObject.GetComponent<Player>().characterData;
            else
            {
                Debug.Log("characterData == null.");
            }

            // Destroy old player object
            playerNetworkObject.Despawn();


            // Create new player object
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

                if (IsOwner)
                {
                    if (UIPlayerInventory.Instance != null)
                    {
                        UIPlayerInventory.Instance.LoadReferences();
                    }


                    if (UIItemInHand.Instance != null)
                    {
                        UIItemInHand.Instance.LoadReferences();
                    }
                }
            }         
            else if (newPlayerNetworkObject.GetComponent<Player>() != null)
            {
                newPlayerNetworkObject.GetComponent<Player>().characterData = characterData;
                newPlayerNetworkObject.GetComponent<Player>().playerIngameSkills.UpdateCharacterData();
            }

            GameDataManager.Instance.singleModePlayer = newPlayerNetworkObject;
            
        }
    }
}