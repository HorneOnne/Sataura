using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using System;
using TMPro.EditorUtilities;
using System.Threading.Tasks;
using Unity.Services;
using Unity.Services.Authentication;

namespace Sataura.ConnectionManagement
{
    public enum ConnectStatus
    {
        Undefined,
        Success,                    //client successfully connected. This may also be a successful reconnect.
        ServerFull,                 //can't join, server is already at capacity.
        LoggedInAgain,              //logged in on a separate client, causing this one to be kicked out.
        UserRequestedDisconnect,    //intentional Disconnect triggered by the user.
        GenericDisconnect,          //server disconnected, but no specific reason given.
        Reconnecting,               //client lost connection and it attempting to reconnect.
        IncompatibleBuildType,      // client build type is imcompatible with server.
        HostEndedSession,           //host intentionally ended the session.
        StartHostFailed,            //server failed to bind.
        StartClientFailed,          //failed to connect to server and/or invalid network endpoint.
    }


    public struct ReconnectMessage
    {
        public int CurrentAttempt;
        public int MaxAttempt;

        public ReconnectMessage(int currentAttempt, int maxAttempt)
        {
            this.CurrentAttempt = currentAttempt;
            this.MaxAttempt = maxAttempt;
        }
    }


    public struct ConnectionEventMessage : INetworkSerializeByMemcpy
    {
        public ConnectStatus ConnectStatus;
    }

    [System.Serializable]
    public class ConnectionPayload
    {
        public string playerID;
        public string playerName;
        public bool isDebug;
    }

    /// <summary>
    /// This state machine handles connection through the NetworkManager. It is responsible for listening
    /// to NetworkManager callbacks and other outside calls and redirecting them to the current 
    /// ConnectionState object
    /// </summary>
    public class ConnectionManager : MonoBehaviour
    {
        private ConnectionState _currentState;

        private NetworkManager _networkManager;

        public NetworkManager NetworkManager => _networkManager;

        [SerializeField] private int _nbReconnectAttempts = 2;
        public int NbReconnectAttempts => _nbReconnectAttempts;

        public int maxConnectedPlayers = 8;

        
    }

    class OfflineState : ConnectionState
    {
        private const string _mainMenuSceneName = "MainMenu";

        public override void Enter()
        {
            
        }

        public override void Exit() { }

        public override void StartClientIP(string playerName, string ipAddress, int port)
        {
           
        }

        public override void StartClientLobby(string playerName)
        {
            
        }

        public override void StartHostIP(string playerName, string ipAddress, int port)
        {
          
        }

        public override void StartHostLobby(string playerName)
        {
           
        }
    }


    /// <summary>
    /// ConectionMethod contains all setup needed to setup NGO to be ready to start a connection, either
    /// host or client side.
    /// Please override this abstract class to add a new transport or way of connecting.
    /// </summary>
    public abstract class ConnectionMethodBase
    {
        protected ConnectionManager _connectionManager;

        protected readonly string _playerName;
        protected const string _dtlsConnType = "dtls";

        /// <summary>
        /// Setup the host connection prior to starting the NetworkManager
        /// </summary>
        /// <returns></returns>
        public abstract Task SetupHostConnectionAsync();


        /// <summary>
        /// Setup the client connection prior to starting the NetworkManager
        /// </summary>
        /// <returns></returns>
        public abstract Task SetupClientConnectionAysnc();


        /// <summary>
        /// Setup the client for reconnection prior to reconnecting
        /// </summary>
        /// <returns>
        /// sucess = true if succeeded in setting up reconnection, false if failed.
        /// shouldTryAgain = true if we should try again after failing, false if not.
        /// </returns>
        public abstract Task<(bool success, bool shouldTryAgain)> SetupClientReconnectionAsync();
   
    
        public ConnectionMethodBase(ConnectionManager connectionManager, string playerName)
        {
            this._connectionManager = connectionManager;
            this._playerName = playerName;
        }


        /// <summary>
        /// Using authentication, this makes sure your session is associated with your account and not your 
        /// device. This means you could reconnect from a different device for example. A playerID is also a bit more
        /// permanent thatn player prefs. In a browser for example, player prefs can be cleared as easily as cookies.
        /// 
        /// The forked flow here is for debug purposes and to make UGS optional in Boss Room. This way you can study
        /// the sample without setting up a UGS account. It's recommended to investigate you own initialization and
        /// IsSigned flows to see if you need...
        /// </summary>
        /// <param name="playerID"></param>
        /// <param name="playerName"></param>
        protected void SetConnectionPlayload(string playerID, string playerName)
        {
            var payload = JsonUtility.ToJson(new ConnectionPayload()
            {
                playerID = playerID,
                playerName = playerName,
                isDebug = Debug.isDebugBuild
            });

            byte[] playLoadBytes = System.Text.Encoding.UTF8.GetBytes(payload);
            _connectionManager.NetworkManager.NetworkConfig.ConnectionData = playLoadBytes;
        }



        protected string GetPlayerID()
        {
            if(Unity.Services.Core.UnityServices.State != Unity.Services.Core.ServicesInitializationState.Initialized)
            {
                return "";
            }

            return AuthenticationService.Instance.IsSignedIn ? AuthenticationService.Instance.PlayerId : "";
        }
    }


    /// <summary>
    /// Simple IP connection setup with UTP
    /// </summary>
    public class ConenctionMethodIP : ConnectionMethodBase
    {
        private string _ipAddress;
        private ushort _port;

        public ConenctionMethodIP(string ip, ushort port, ConnectionManager connectionManager, string playerName)
            : base(connectionManager, playerName)
        {
            this._ipAddress = ip;
            this._port = port;
            base._connectionManager = connectionManager;
        }

        public override Task SetupClientConnectionAysnc()
        {
            throw new NotImplementedException();
        }

        public override Task<(bool success, bool shouldTryAgain)> SetupClientReconnectionAsync()
        {
            throw new NotImplementedException();
        }

        public override Task SetupHostConnectionAsync()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// UTP's Relay connection setup using the Lobby integration.
    /// </summary>
    class ConenctionMethodRelay : ConnectionMethodBase
    {
        public ConenctionMethodRelay(ConnectionManager connectionManager, string playerName) 
            : base(connectionManager, playerName)
        {
        }

        public override Task SetupClientConnectionAysnc()
        {
            throw new NotImplementedException();
        }

        public override Task<(bool success, bool shouldTryAgain)> SetupClientReconnectionAsync()
        {
            throw new NotImplementedException();
        }

        public override Task SetupHostConnectionAsync()
        {
            throw new NotImplementedException();
        }
    }
}

