using Unity.Netcode.Transports.UTP;
using UnityEngine;

namespace Sataura
{
    public class MainMenuInfomation : MonoBehaviour
    {
        public static MainMenuInfomation Instance;
        public enum PlayMode
        {
            NotSelected,
            SinglePlayer,
            MultiPlayer,
        }

        [Header("References")]
        [SerializeField] private UnityTransport _unityTransport;

       
        [Header("Properties")]
        [SerializeField] private PlayMode _playMode = PlayMode.NotSelected;

        // Multiplayer information
        [SerializeField] private string _serverAddress = "127.0.0.1";
        [SerializeField] private string _serverPort = "7777";

        [Header("Runtime References")]
        public CharacterData characterData;


        #region Properties
        public string IPv4 { get => _serverAddress; }
        public string Port { get => _serverPort; }
        public UnityTransport UnityTransport { get => _unityTransport; }
        #endregion

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);

            if (Instance == null)
                Instance = this;
            else
                Destroy(this.gameObject);
        }

        private void Start()
        {
            _playMode = PlayMode.NotSelected;
        }

        public void SetPlayMode(PlayMode playMode)
        {
            this._playMode = playMode;  
        }

        public void SetServerIPv4(string serverAddress)
        {
            this._serverAddress = serverAddress;
        }

        public void SetServerPort(string port)
        {
            this._serverPort = port;
        }
    }
}