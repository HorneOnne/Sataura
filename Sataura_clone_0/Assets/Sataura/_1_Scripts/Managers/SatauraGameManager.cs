using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Sataura
{
    public class SatauraGameManager : NetworkBehaviour
    {
        public static SatauraGameManager Instance { get; private set; }
        [SerializeField] private Transform playerPrefab;


        public event Action OnStateChanged;
        public event Action OnLocalGamePaused;
        public event Action OnLocalGameUnPaused;
        public event Action OnMultiplayerGamePaused;
        public event Action OnMultiplayerGameUnpaused;
        public event Action OnLocalPlayerReadyChanged;


        public enum State
        {
            WaitingToStart,
            CountdownToStart,
            GamePlaying,
            GameOver,
        }

        public NetworkVariable<State> state = new NetworkVariable<State>(State.WaitingToStart);
        private bool isLocalPlayerReady;
        private NetworkVariable<float> countdownToStartTimer = new NetworkVariable<float>(3.0f);
        private NetworkVariable<float> gamePlayingTimer = new NetworkVariable<float>(0.0f);
        private float gamePlayingTimermax = 90.0f;
        private bool isLocalGamePaused = false;
        private NetworkVariable<bool> isGamePaused = new NetworkVariable<bool>(false);
        private Dictionary<ulong, bool> playerReadyDictionary;
        private Dictionary<ulong, bool> playerPauseDictionary;
        private bool autoTestGamePausedState;



        public List<Transform> players = new List<Transform>();





        private void Awake()
        {
            Instance = this;

            playerReadyDictionary = new Dictionary<ulong, bool>();
            playerPauseDictionary = new Dictionary<ulong, bool>();
        }

        private void Start()
        {

        }

        public override void OnNetworkSpawn()
        {
            state.OnValueChanged += State_OnValueChanged;
            isGamePaused.OnValueChanged += IsGamePaused_OnValueChanged;

            if(IsServer)
            {
                NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
                NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
            }
        }

       
        private void SceneManager_OnLoadEventCompleted(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode,
            List<ulong> clientsComplete, List<ulong> clientsTimeOut)
        {
            foreach(ulong clientID in NetworkManager.Singleton.ConnectedClientsIds)
            {
                Vector2 spawnPosition = new Vector2(30, 30);
                Transform playerTransform = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
                playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientID, true);
                players.Add(playerTransform);
            }

            SetPlayerReadyServerRpc();
        }

        private void NetworkManager_OnClientDisconnectCallback(ulong obj)
        {
            autoTestGamePausedState = false;
        }


        private void IsGamePaused_OnValueChanged(bool previousValue, bool newValue)
        {
            if (isGamePaused.Value)
            {
                Time.timeScale = 0.0f;

                OnMultiplayerGamePaused?.Invoke();
            }
            else
            {
                Time.timeScale = 1.0f;

                OnMultiplayerGameUnpaused?.Invoke();
            }
        }

        private void State_OnValueChanged(State previousValue, State newValue)
        {
            OnStateChanged?.Invoke();
        }



        private void GameInput_OnInteractAction()
        {
            if (state.Value == State.WaitingToStart)
            {
                isLocalPlayerReady = true;
                OnLocalPlayerReadyChanged?.Invoke();

                SetPlayerReadyServerRpc();
            }
        }


        [ServerRpc(RequireOwnership =false)]
        private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
        {
            playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

            bool allClientReady = true;
            foreach(ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                if (!playerReadyDictionary.ContainsKey(clientId) || !playerReadyDictionary[clientId])
                {
                    // This player is NOT ready.
                    allClientReady = false;
                    break;
                }
            }

            if(allClientReady)
            {
                state.Value = State.CountdownToStart;
            }
        }


        private void Update()
        {
            if (!IsServer) return;

            switch(state.Value)
            {
                default:
                case State.WaitingToStart:
                    break;
                case State.CountdownToStart:
                    countdownToStartTimer.Value -= Time.deltaTime;
                    if(countdownToStartTimer.Value < 0.0f)
                    {
                        state.Value = State.GamePlaying;
                        gamePlayingTimer.Value = gamePlayingTimermax;
                    }
                    break;
                case State.GamePlaying:
                    gamePlayingTimer.Value -= Time.deltaTime;
                    if (gamePlayingTimer.Value < 0f)
                    {
                        state.Value = State.GameOver;
                    }
                    break;
                case State.GameOver:
                    break;
            }
        }

        private void LateUpdate()
        {
            if(autoTestGamePausedState)
            {
                autoTestGamePausedState = false;
                //
            }
        }


        /*public bool IsGamePlaying()
        {

        }

        public bool IsCountdownToStartActive()
        {

        }

        public float GetCountdownToStartTimer()
        {

        }

        public bool IsGameOver()
        {

        }

        public bool IsWaitingToStart()
        {

        }

        public bool IsLocalPlayerReady()
        {

        }

        public float GetGamePlayingTimerNormalized()
        {

        }*/

        public void TogglePauseGame()
        {
            isLocalGamePaused = !isLocalGamePaused;
            if(isLocalGamePaused)
            {
                PauseGameServerRpc();

                OnLocalGamePaused?.Invoke();
            }
            else
            {
                UnpauseGameServerRpc();
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void PauseGameServerRpc(ServerRpcParams serverRpcParams = default)
        {

        }

        [ServerRpc(RequireOwnership = false)]
        private void UnpauseGameServerRpc(ServerRpcParams serverRpcParams = default)
        {

        }

        private void TestGamePausedState()
        {
            foreach (ulong clientID in NetworkManager.Singleton.ConnectedClientsIds)
            {
                if (playerPauseDictionary.ContainsKey(clientID) && playerPauseDictionary[clientID])
                {
                    // This player is paused
                    isGamePaused.Value = true;
                    return;
                }
            }

            // All players are unpaused
            isGamePaused.Value = false;
        }


    }
}