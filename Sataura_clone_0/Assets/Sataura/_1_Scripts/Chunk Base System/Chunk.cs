using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Sataura
{
    [System.Serializable]
    public class Chunk : NetworkBehaviour
    {
        public Vector2Int indexPosition;
        public Vector2Int position;
        //public Transform localPlayer;
        public List<Transform> players = new List<Transform>();
        public NetworkObject networkObject;
        private World world;



        [HideInInspector] public List<BaseEnemy> enemies = new List<BaseEnemy>();
        [HideInInspector] public List<Currency> currencies = new List<Currency>();


        float timeUpdateFrequency = 0.3f;
        float timeUpdateCount = 0;


        private int distanceToDisable;
        private float minDistance;


        public override void OnNetworkSpawn()
        {
 
        }

        public void SetChunkProperties(Vector2Int indexPosition, Vector2Int position, List<Transform> players, World world)
        {
            this.indexPosition = indexPosition;
            this.position = position;
            this.players = players;
            this.world = world;

            distanceToDisable = world.ChunkSize * 2 + 10;
        }

        private void Update()
        {
            if (!IsServer) return;
            if (players.Count == 0) return;

            if (Time.time - timeUpdateCount > timeUpdateFrequency)
            {
                timeUpdateCount = Time.time;
                minDistance = Vector2.Distance(transform.position, players[0].position);

                for (int i = 0; i < players.Count; i++)
                {
                    if (players[i] == null)
                        continue;

                    float distanceFromPlayer = Vector2.Distance(transform.position, players[i].position);
                    if (distanceFromPlayer < minDistance)
                    {
                        minDistance = distanceFromPlayer;
                    }        
                }


                if (minDistance >= distanceToDisable)
                {
                    SetDisable();
                }
            }
        }


        private void SetDisable()
        {
            if(networkObject.IsSpawned)
            {
                networkObject.Despawn(false);
                networkObject.gameObject.SetActive(false);
            }     
        }
    }


}


