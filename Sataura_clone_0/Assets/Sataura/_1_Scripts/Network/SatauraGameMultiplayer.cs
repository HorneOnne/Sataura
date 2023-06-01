using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Sataura
{
    public class SatauraGameMultiplayer : NetworkBehaviour
    {
        public const int MAX_PLAYER_AMOUNT = 4;
        private const string PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER = "PlayerNameMultiplayer";

        public static SatauraGameMultiplayer Instance { get; private set; }


        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(this.gameObject);

            DontDestroyOnLoad(this.gameObject);
        }
    }
}

