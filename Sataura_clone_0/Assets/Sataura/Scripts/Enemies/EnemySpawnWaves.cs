using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

namespace Sataura
{
    public class EnemySpawnWaves : NetworkBehaviour
    {
        [SerializeField] private GameObject enemyPrefab;
        private NetworkObjectPool networkObjectPool;





        [SerializeField] private List<Transform> players = new List<Transform>();

        [SerializeField] private List<Enemy001> currentEnemy001Wave = new List<Enemy001>();

        public override void OnNetworkSpawn()
        {
            networkObjectPool = NetworkObjectPool.Singleton;

            NetworkManager.OnClientConnectedCallback += AddClient;
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
                currentEnemy001Wave.Clear();

                //SpawnEnemiesInCircle();

                GenerateSquareWaveEnemies();

                //GenerateCircleWaveEnemies(players[0].position);
            }
        }

        float getEnemiesAroundTimeElapse = 0.0f;
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

                if (currentEnemy001Wave == null) return;
                if (currentEnemy001Wave.Count == 0) return;

                int size = currentEnemy001Wave.Count;

                for (int i = 0; i < size; i++)
                {
                    var enemy = currentEnemy001Wave[i].GetComponent<Enemy001>();
                    //var distance = Vector2.Distance(players[0].position, enemy.Rb2D.position);

                    if (enemy.isBeingKnockback == false)
                    {
                        var direction = (Vector2)players[0].position - enemy.Rb2D.position;
                        direction.Normalize();
                        enemy.Rb2D.MovePosition(enemy.Rb2D.position + direction * 10 * Time.fixedDeltaTime);
                    }

                }
            }
        }


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
                currentEnemy001Wave.Add(e.GetComponent<Enemy001>());
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
                currentEnemy001Wave.Add(e.GetComponent<Enemy001>());
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
                currentEnemy001Wave.Add(e.GetComponent<Enemy001>());

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
