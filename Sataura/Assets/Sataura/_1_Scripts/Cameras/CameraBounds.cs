using UnityEngine;
using Unity.Netcode;

namespace Sataura
{
    public class CameraBounds : MonoBehaviour
    {
        public static CameraBounds Instance { get; private set; }

        public Transform localPlayer;


        private void Awake()
        {
            Instance = this;
        }

        public void SetHostLocalClient(ulong clientId)
        {
            localPlayer = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.transform;
        }

        public void SetLocalClient()
        {
            localPlayer = NetworkManager.Singleton.LocalClient.PlayerObject.transform;
        }

        private void LateUpdate()
        {
            if (localPlayer == null) return;
            transform.position = new Vector2(localPlayer.position.x, transform.position.y);
        }
    }
}

