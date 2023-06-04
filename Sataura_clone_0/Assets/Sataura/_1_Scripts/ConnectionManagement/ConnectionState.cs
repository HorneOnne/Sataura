using System;
using Unity.Netcode;

namespace Sataura.ConnectionManagement
{
    public abstract class ConnectionState
    {
        protected ConnectionManager m_ConnectionManager;

        public abstract void Enter();

        public abstract void Exit();

        public virtual void OnClientConnected(ulong clientID) { }
        public virtual void OnClientDisconnected(ulong clientID) { }
        public virtual void OnServerStarted() { }
        public virtual void StartClientIP(string playerName, string ipAddress, int port) { }
        public virtual void StartClientLobby(string playerName) { }
        public virtual void StartHostIP(string playerName, string ipAddress, int port) { }
        public virtual void StartHostLobby(string playerName) { }
        public virtual void OnUserRequestedShutdown() { }
        public virtual void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response) { }

        public virtual void OnTransportFailure() { }
        public virtual void OnsServerStopped() { }
    }
}



