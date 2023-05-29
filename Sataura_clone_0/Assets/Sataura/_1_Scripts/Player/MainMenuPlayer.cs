using Unity.Netcode;
using UnityEngine;

namespace Sataura
{
    public class MainMenuPlayer : Player
    {       
        public override void OnNetworkSpawn()
        {
            GameDataManager.Instance.mainMenuPlayer = this;
        }
    }
}