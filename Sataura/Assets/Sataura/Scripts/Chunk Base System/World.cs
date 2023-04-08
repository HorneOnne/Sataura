using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Sataura
{
    public class World : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject chunkPrefab;
        [SerializeField] private int chunkSize;


        [Header("Containers")]
        public Dictionary<Vector2Int, Chunk> chunks = new Dictionary<Vector2Int, Chunk>();  // [indexPosition, chunk]
        //public HashSet<Chunk> activeChunks = new HashSet<Chunk>();


        [Header("References To Player")]
        public List<Transform> players = new List<Transform>();
        public List<Vector2> lastPlayerPosition = new List<Vector2>();



        #region Properties
        public int ChunkSize { get { return chunkSize; } }

        #endregion

        public override void OnNetworkSpawn()
        {
            if (!IsServer) return;

            NetworkManager.OnClientConnectedCallback += SetClient;


            SetLocalClient();
            CreateChunk(new Vector2Int(0, 0));
            UpdateWorld();
        }

        public void SetClient(ulong clientId)
        {
            players.Add(NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.transform);
            lastPlayerPosition.Add(new Vector2(0, 0));
        }

        public void SetLocalClient()
        {
            //players.Add(GameDataManager.Instance.singleModePlayer.transform);
            players.Add(NetworkManager.Singleton.LocalClient.PlayerObject.transform);
            lastPlayerPosition.Add(new Vector2(0, 0));
        }




        private void Update()
        {
            if (!IsServer) return;

            if (players.Count == 0)
                return;

            for (int i = 0; i < players.Count; i++)
            {
                if (players[i] == null)
                    continue;

                if (Mathf.Abs(players[i].position.x - lastPlayerPosition[i].x) > chunkSize / 3)
                {
                    lastPlayerPosition[i] = new Vector2(players[i].position.x, lastPlayerPosition[i].y);
                    UpdateWorld();
                }
            }

        }


        public void UpdateWorld()
        {
            if (players.Count == 0)
                return;

            for (int i = 0; i < players.Count; i++)
            {
                int posX = Mathf.FloorToInt(players[i].position.x / chunkSize);
                Vector2Int currentPos;

                for (int x = posX - 1; x <= posX + 1; x++)
                {
                    currentPos = new Vector2Int(x, 0);

                    if (chunks.ContainsKey(currentPos))
                    {
                        if (!chunks[currentPos].gameObject.activeInHierarchy)
                        {
                            SetChunkActive(chunks[currentPos]);
                        }
                    }
                    else
                    {
                        CreateChunk(currentPos);
                    }
                }
            }       
        }

        private void SetChunkActive(Chunk chunk)
        {
            if (chunks.ContainsKey(chunk.indexPosition))
            {               
                chunks[chunk.indexPosition].gameObject.SetActive(true);
                chunk.networkObject.Spawn();
            }
        }


        void CreateChunk(Vector2Int chunkIndex)
        {
            Vector2Int chunkPosition = new Vector2Int(chunkIndex.x * chunkSize, chunkIndex.y * chunkSize);
            GameObject chunkObject = Instantiate(chunkPrefab, (Vector2)chunkPosition, Quaternion.identity);        
            Chunk chunk = chunkObject.GetComponent<Chunk>();
            chunk.networkObject.Spawn();

            /*chunk.indexPosition = chunkIndex;
            chunk.position = chunkPosition;
            chunk.players = players;*/
            chunk.SetChunkProperties(chunkIndex, chunkPosition, players, this);

            chunks.Add(chunkIndex, chunk);          
        }
    }
}

