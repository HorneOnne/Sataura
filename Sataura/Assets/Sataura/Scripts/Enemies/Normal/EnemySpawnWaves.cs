using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Sataura
{
    public class EnemySpawnWaves : NetworkBehaviour
    {
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private GameObject pinkSlimePrefab;
        [SerializeField] private GameObject batPrefab;

        private NetworkObjectPool networkObjectPool;


        [SerializeField] private List<Transform> players = new List<Transform>();

        [SerializeField] private List<BaseEnemy> currentEnemyWave = new List<BaseEnemy>();

        


        public override void OnNetworkSpawn()
        {
            networkObjectPool = NetworkObjectPool.Singleton;
            NetworkManager.OnClientConnectedCallback += AddClient;

            if(IsServer)
            {
                IngameInformationManager.Instance.currentTotalEnemiesIngame = 0;
            }
        }

        public override void OnNetworkDespawn()
        {
            NetworkManager.OnClientConnectedCallback -= AddClient;
        }

        private void AddClient(ulong clientId)
        {
            if (!IsServer) return;

            Debug.Log($"Client id: {clientId} is connected.");
            players.Add(NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.transform);
        }

        private void Update()
        {
            if (!IsServer) return;

            if (Input.GetKeyDown(KeyCode.J))
            {
                //currentEnemyWave.Clear();

                //SpawnEnemiesInCircle();

                //GenerateSquareWaveEnemies();

                //GenerateCircleWaveEnemies(players[0].position);

                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                for (int i = 0; i < 1; i++)
                {              
                    var enemyObject = Instantiate(pinkSlimePrefab, mousePos, Quaternion.identity);
                    enemyObject.GetComponent<BaseEnemy>().SetFollowTarget(players[0]);

                    var enemyNetworkObject = enemyObject.GetComponent<NetworkObject>();
                    enemyNetworkObject.Spawn();
                    IngameInformationManager.Instance.currentTotalEnemiesIngame++;
                }
               
            }

            if(Time.time < 2.0f)
            {
                return;
            }
            if(IngameInformationManager.Instance.currentTotalEnemiesIngame < 10)
            {
                var enemyObject = Instantiate(pinkSlimePrefab, (Vector2)players[0].position + new Vector2(Random.Range(-30, 30),40), Quaternion.identity);
                enemyObject.GetComponent<BaseEnemy>().SetFollowTarget(players[0]);

                var enemyNetworkObject = enemyObject.GetComponent<NetworkObject>();
                enemyNetworkObject.Spawn();
                IngameInformationManager.Instance.currentTotalEnemiesIngame++;
            }

            if (IngameInformationManager.Instance.currentTotalEnemiesIngame < 20)
            {
                var enemyObject = Instantiate(batPrefab, (Vector2)players[0].position + new Vector2(Random.Range(-30, 30), 60), Quaternion.identity);
                enemyObject.GetComponent<BaseEnemy>().SetFollowTarget(players[0]);

                var enemyNetworkObject = enemyObject.GetComponent<NetworkObject>();
                enemyNetworkObject.Spawn();
                IngameInformationManager.Instance.currentTotalEnemiesIngame++;
            }
        }

        /*float getEnemiesAroundTimeElapse = 0.0f;
        float timeElapse = 0.0f;
        private void FixedUpdate()
        {
            if (!IsServer) return;

            if (Time.time - getEnemiesAroundTimeElapse >= 2.0f)
            {
                getEnemiesAroundTimeElapse = Time.time;
            }

            if (Time.time - timeElapse >= 0.035f)
            {
                timeElapse = Time.time;

                if (currentEnemyWave == null) return;
                if (currentEnemyWave.Count == 0) return;

                int size = currentEnemyWave.Count;
                List<int> enemiesDestroyedIndex = new List<int>();

                for (int i = 0; i < size; i++)
                {
                    if(currentEnemyWave[i] == null)
                    {
                        enemiesDestroyedIndex.Add(i);
                        continue;
                    }

                    var enemy = currentEnemyWave[i].GetComponent<BaseEnemy>();
                    enemy.MoveAI(players[0].position);
                }


                // Destroy enemies in currentEnemyWave when it has been dead.
                for (int i = 0; i < enemiesDestroyedIndex.Count; i++)
                {
                    enemiesDestroyedIndex.RemoveAt(i);
                }
            }
        }*/


        private GameObject Spawn(Vector2 position, Quaternion rotation)
        {
            var enemyNetObject = networkObjectPool.GetNetworkObject(enemyPrefab, position, rotation);
            enemyNetObject.Spawn();

            return enemyNetObject.gameObject;
        }



        public float radius = 10.0f;
        public int numPoints = 10;
        public void SpawnEnemiesInCircle()
        {
            float angle = 0.0f;
            float angleIncrement = (Mathf.PI * 2.0f) / numPoints;

            for (int i = 0; i < numPoints; i++)
            {
                float x = Mathf.Cos(angle) * radius;
                float y = Mathf.Sin(angle) * radius;
                angle += angleIncrement;


                var e = Spawn(new Vector2(x, y), Quaternion.identity);
                currentEnemyWave.Add(e.GetComponent<Enemy001>());
            }
        }


        public void GenerateCircleWaveEnemies(Vector2 centerPoint)
        {
            float angle = 0.0f;
            float angleIncrement = (Mathf.PI * 2.0f) / numPoints;

            for (int i = 0; i < numPoints; i++)
            {
                float x = centerPoint.x + Mathf.Cos(angle) * radius;
                float y = centerPoint.y + Mathf.Sin(angle) * radius;
                angle += angleIncrement;

                var e = Spawn(new Vector2(x, y), Quaternion.identity);
                currentEnemyWave.Add(e.GetComponent<Enemy001>());
            }
        }


        private float spacing = 10;
        private int pointsPerSide = 3;
        public void GenerateSquareWaveEnemies()
        {
            float x = -spacing * (numPoints / 2);
            float y = spacing * (numPoints / 2);

            for (int i = 0; i < numPoints; i++)
            {
                var e = Spawn(new Vector2(x, y), Quaternion.identity);
                currentEnemyWave.Add(e.GetComponent<Enemy001>());

                if ((i + 1) % pointsPerSide == 0)
                {
                    x = -spacing * (numPoints / 2);
                    y -= spacing;
                }
                else
                {
                    x += spacing;
                }
            }
        }
    }
}
