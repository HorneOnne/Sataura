using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
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

        [Header("Spawner points")]
        [SerializeField] private List<Transform> spawnerPointsLeft = new List<Transform>();
        [SerializeField] private List<Transform> spawnerPointsRight = new List<Transform>();
        [SerializeField] private List<Transform> spawnerPointsUp = new List<Transform>();
        


        public override void OnNetworkSpawn()
        {
            networkObjectPool = NetworkObjectPool.Singleton;
            NetworkManager.OnClientConnectedCallback += AddClient;

            if(IsServer)
            {
                if (players.Contains(NetworkManager.Singleton.LocalClient.PlayerObject.transform) == false)
                    players.Add(NetworkManager.Singleton.LocalClient.PlayerObject.transform);

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

            //Debug.Log($"Client id: {clientId} is connected.");
            
            if(players.Contains(NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.transform) == false)
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

                //GenerateSquareWaveEnemies(players[0]);

                GenerateLeftWaveEnemies(players[0].position);
                GenerateRightWaveEnemies(players[0].position);
            }

            if(Time.time < 2.0f)
            {
                return;
            }
            if (players.Count == 0) return;


            return;
            if(CountdownTimer.Instance.TimeLeft > 15 * 60)
            {
                if (IngameInformationManager.Instance.currentTotalEnemiesIngame < 10)
                {
                    var enemyObject = Instantiate(pinkSlimePrefab, (Vector2)players[0].position + new Vector2(Random.Range(-30, 30), 40), Quaternion.identity);
                    enemyObject.GetComponent<BaseEnemy>().SetFollowTarget(players[0]);

                    var enemyNetworkObject = enemyObject.GetComponent<NetworkObject>();
                    enemyNetworkObject.Spawn();
                    IngameInformationManager.Instance.currentTotalEnemiesIngame++;
                }
            }
            else if (CountdownTimer.Instance.TimeLeft > 14 * 60)
            {
                if (IngameInformationManager.Instance.currentTotalEnemiesIngame < 20)
                {
                    var enemyObject = Instantiate(pinkSlimePrefab, (Vector2)players[0].position + new Vector2(Random.Range(-30, 30), 40), Quaternion.identity);
                    enemyObject.GetComponent<BaseEnemy>().SetFollowTarget(players[0]);

                    var enemyNetworkObject = enemyObject.GetComponent<NetworkObject>();
                    enemyNetworkObject.Spawn();
                    IngameInformationManager.Instance.currentTotalEnemiesIngame++;
                }
            }
            else
            {
                if (IngameInformationManager.Instance.currentTotalEnemiesIngame < 20)
                {
                    var enemyObject = Instantiate(batPrefab, (Vector2)players[0].position + new Vector2(Random.Range(-30, 30), 60), Quaternion.identity);
                    enemyObject.GetComponent<BaseEnemy>().SetFollowTarget(players[0]);

                    var enemyNetworkObject = enemyObject.GetComponent<NetworkObject>();
                    enemyNetworkObject.Spawn();
                    IngameInformationManager.Instance.currentTotalEnemiesIngame++;
                }
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
            var enemyObject = Instantiate(pinkSlimePrefab, position, rotation);
            return enemyObject;
        }


        public void GenerateLeftWaveEnemies(Vector2 playerPosition)
        {
            for (int i = 0; i < spawnerPointsLeft.Count; i++)
            {
                float x = playerPosition.x + spawnerPointsLeft[i].position.x;
                float y = spawnerPointsLeft[i].position.y;

                var e = Spawn(new Vector2(x, y), Quaternion.identity);
                currentEnemyWave.Add(e.GetComponent<Slime>());
                e.GetComponent<BaseEnemy>().SetFollowTarget(players[0]);

                var enemyNetworkObject = e.GetComponent<NetworkObject>();
                enemyNetworkObject.Spawn();
                IngameInformationManager.Instance.currentTotalEnemiesIngame++;
            }
        }

        public void GenerateRightWaveEnemies(Vector2 playerPosition)
        {
            for (int i = 0; i < spawnerPointsLeft.Count; i++)
            {
                float x = playerPosition.x + spawnerPointsRight[i].position.x;
                float y = spawnerPointsLeft[i].position.y;

                var e = Spawn(new Vector2(x, y), Quaternion.identity);
                currentEnemyWave.Add(e.GetComponent<Slime>());
                e.GetComponent<BaseEnemy>().SetFollowTarget(players[0]);

                var enemyNetworkObject = e.GetComponent<NetworkObject>();
                enemyNetworkObject.Spawn();
                IngameInformationManager.Instance.currentTotalEnemiesIngame++;
            }
        }


        private float radius = 10.0f;
        private int numPoints = 10;
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
        public void GenerateSquareWaveEnemies(Transform playerTransform)
        {
            float x = -spacing * (numPoints / 2);
            float y = spacing * (numPoints / 2);

            for (int i = 0; i < numPoints; i++)
            {
                var e = Spawn(playerTransform.position + new Vector3(x, y, 0), Quaternion.identity);
                currentEnemyWave.Add(e.GetComponent<Slime>());
                e.GetComponent<BaseEnemy>().SetFollowTarget(players[0]);

                var enemyNetworkObject = e.GetComponent<NetworkObject>();
                enemyNetworkObject.Spawn();
                IngameInformationManager.Instance.currentTotalEnemiesIngame++;

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
