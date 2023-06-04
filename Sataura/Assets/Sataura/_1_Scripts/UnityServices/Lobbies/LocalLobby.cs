using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using System;

namespace Sataura.UnityServices.Lobbies
{
    /// <summary>
    /// A local wrapper around a lobby's remote data, with additional functionality for providing that data
    /// to UI elements and tracking local player objects.
    /// </summary>
    public sealed class LocalLobby
    {
        public event Action<LocalLobby> onChanged;


        public static List<LocalLobby> CreateLocalLobbies(QueryResponse response)
        {
            var retLst = new List<LocalLobby>();
            foreach(var lobby in response.Results)
            {
                retLst.Add(Create(lobby));
            }
            return retLst;
        }

        public static LocalLobby Create(Lobby lobby)
        {
            var data = new LocalLobby();
            return data;
        }
    }

}
