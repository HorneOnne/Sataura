using Unity.Netcode;
using UnityEngine;

namespace Sataura
{
    public class MainMenuPlayer : NetworkBehaviour
    {
        public CharacterData characterData;

        public override void OnNetworkSpawn()
        {
            GameDataManager.Instance.singleModePlayer = this.gameObject;
        }
    }
}