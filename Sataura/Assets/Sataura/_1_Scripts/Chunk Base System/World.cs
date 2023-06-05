using System.Collections.Generic;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using Unity.VisualScripting;

namespace Sataura
{
    public class World : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject chunkPrefab;
        [SerializeField] private int chunkSize;
        private SatauraGameManager satauraGameManager;


        [Header("Containers")]
        public Dictionary<Vector2Int, Chunk> chunks = new Dictionary<Vector2Int, Chunk>();  // [indexPosition, chunk]
        //public HashSet<Chunk> activeChunks = new HashSet<Chunk>();


        [Header("References To Player")]
        public List<Vector2> lastPlayerPosition = new List<Vector2>();



        #region Properties
        public int ChunkSize { get { return chunkSize; } }

        #endregion

        public override void OnNetworkSpawn()
        {
            if (!IsServer) return;
            satauraGameManager = SatauraGameManager.Instance;

            StartCoroutine(LoadPlayerPosition());           
        }

        private IEnumerator LoadPlayerPosition()
        {
            yield return new WaitUntil(() => satauraGameManager.state.Value != SatauraGameManager.State.CountdownToStart);
            for (int i = 0; i < satauraGameManager.players.Count; i++)
            {
                lastPlayerPosition.Add(satauraGameManager.players[i].transform.position);
            }

            CreateChunk(new Vector2Int(0, 0));
            UpdateWorld();
        }



        private void Update()
        {
            if (!IsServer) return;       
            if (satauraGameManager.state.Value != SatauraGameManager.State.GamePlaying)
                return;


            if (satauraGameManager.players.Count == 0)
                return;

            for (int i = 0; i < satauraGameManager.players.Count; i++)
            {
                if (satauraGameManager.players[i] == null)
                    continue;

                if (Mathf.Abs(satauraGameManager.players[i].position.x - lastPlayerPosition[i].x) > chunkSize / 3)
                {
                    lastPlayerPosition[i] = new Vector2(satauraGameManager.players[i].position.x, lastPlayerPosition[i].y);
                    UpdateWorld();
                }
            }

        }


        public void UpdateWorld()
        {
            if (satauraGameManager.players.Count == 0)
                return;

            for (int i = 0; i < satauraGameManager.players.Count; i++)
            {
                int posX = Mathf.FloorToInt(satauraGameManager.players[i].position.x / chunkSize);
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
            chunk.SetChunkProperties(chunkIndex, chunkPosition, satauraGameManager.players, this);

            chunks.Add(chunkIndex, chunk);          
        }
    }
}

