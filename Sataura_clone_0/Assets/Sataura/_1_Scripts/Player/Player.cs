using Unity.Netcode;
using UnityEngine;

namespace Sataura
{
    public abstract class Player : NetworkBehaviour
    {
        [Header("References")]
        private PlayerType _playerType;       
        [SerializeField] private NetworkObject _networkObject;


        [Header("Runtime References")]
        public CharacterData characterData;


        #region
        public PlayerType PlayerType { get { return _playerType; } }
        public NetworkObject NetObject { get { return _networkObject; } }
        #endregion


    }
}