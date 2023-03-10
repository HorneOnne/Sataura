using UnityEngine;
using Unity.Netcode;

namespace Sataura
{
    public class GetLocalPlayer : NetworkBehaviour
    {
        public Player Player { get; private set; }

        public override void OnNetworkSpawn()
        {
            Debug.Log("Network spawn");
        }


    }

}

