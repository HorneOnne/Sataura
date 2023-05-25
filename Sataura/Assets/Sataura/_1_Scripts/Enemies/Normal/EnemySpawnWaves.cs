using System.Collections.Generic;
using System.Collections;
using Unity.Netcode;
using UnityEngine;


namespace Sataura
{
    public class EnemySpawnWaves : NetworkBehaviour
    {
        public static event System.Action OnNewEnemyWaveSpawned;
        public static int waveIndex = 1;
        public static int currentWaveIndex = 1;

        private NetworkObjectPool networkObjectPool;
        [SerializeField] private List<Transform> players = new List<Transform>();
        [Header("Spawner points")]
        [SerializeField] private List<Transform> spawnerPointsLeft = new List<Transform>();
        [SerializeField] private List<Transform> spawnerPointsRight = new List<Transform>();
        [SerializeField] private List<Transform> spawnerPointsUp = new List<Transform>();

        private float _timeElapse = 0.0f;

        // Cached
        private PinkSlime _pinkSlimePrefab;
        private BlueSlime _blueSlimeePrefab;
        private MotherSlime _motherSlimePrefab;
        private BabySlime _babySlimePrefab;
        private Bat _batPrefab;
        private BlackBat _blackBatPrefab;
        private WhiteSkull _whiteSkullPrefab;
        private PurpleSkull _purpleSkullPrefab;
        private Golem _golemPrefab;



        // Boss
        private bool _isSummonKingSlime = false;
        private KingSlime _kingSlime;

        public override void OnNetworkSpawn()
        {
            networkObjectPool = NetworkObjectPool.Singleton;
            NetworkManager.OnClientConnectedCallback += AddClient;

            if (IsServer)
            {
                if (players.Contains(NetworkManager.Singleton.LocalClient.PlayerObject.transform) == false)
                    players.Add(NetworkManager.Singleton.LocalClient.PlayerObject.transform);

                IngameInformationManager.Instance.currentTotalEnemiesIngame = 0;
            }
        }

        private GameObject GetEnemyPrefabByType(EnemyType enemyType)
        {
            switch (enemyType)
            {
                case EnemyType.PinkSlime:
                    return _pinkSlimePrefab == null ? GameDataManager.Instance.GetEnemyPrefab(enemyType) : _pinkSlimePrefab.gameObject;
                case EnemyType.BlueSlime:
                    return _blueSlimeePrefab == null ? GameDataManager.Instance.GetEnemyPrefab(enemyType) : _blueSlimeePrefab.gameObject;
                case EnemyType.MotherSlime:
                    return _motherSlimePrefab == null ? GameDataManager.Instance.GetEnemyPrefab(enemyType) : _motherSlimePrefab.gameObject;
                case EnemyType.BabySlime:
                    return _babySlimePrefab == null ? GameDataManager.Instance.GetEnemyPrefab(enemyType) : _babySlimePrefab.gameObject;
                case EnemyType.Bat:
                    return _batPrefab == null ? GameDataManager.Instance.GetEnemyPrefab(enemyType) : _batPrefab.gameObject;
                case EnemyType.BlackBat:
                    return _blackBatPrefab == null ? GameDataManager.Instance.GetEnemyPrefab(enemyType) : _blackBatPrefab.gameObject;
                case EnemyType.Bonehead:
                    return _whiteSkullPrefab == null ? GameDataManager.Instance.GetEnemyPrefab(enemyType) : _whiteSkullPrefab.gameObject;
                case EnemyType.Cursedwraith:
                    return _purpleSkullPrefab == null ? GameDataManager.Instance.GetEnemyPrefab(enemyType) : _purpleSkullPrefab.gameObject;
                case EnemyType.ObsidianMaw:
                    return _golemPrefab == null ? GameDataManager.Instance.GetEnemyPrefab(enemyType) : _blueSlimeePrefab.gameObject;
                default:
                    return null;
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

            if (players.Contains(NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.transform) == false)
                players.Add(NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.transform);
        }


        private void Update()
        {
            if (!IsServer) return;
            if (IngameInformationManager.Instance.CurrentGameState == IngameInformationManager.GameState.Play)
            {
                /*if (Input.GetKeyDown(KeyCode.J))
                {
                    //GenerateLeftWaveEnemies(players[0].position + new Vector3(-3, 20, 0), EnemyType.ObsidianMaw, Random.Range(1, 2));
                    //SpawnBoss(BossType.KingSlime, Camera.main.ScreenToWorldPoint(Input.mousePosition), Quaternion.identity);
                }*/


                if (_isSummonKingSlime == false)
                {
                    Map001WaveGeneration();
                }

            }


            if (currentWaveIndex != waveIndex)
            {
                currentWaveIndex = waveIndex;
                OnNewEnemyWaveSpawned?.Invoke();
            }
        }


       
        private void Map001WaveGeneration()
        {
            if (CountdownTimer.Instance.TimeLeft > CountdownTimer.Instance.MinutesToSeconds(14) + 30)
            {
                waveIndex = 1;

                if (Wait(10.0f))
                {
                    Debug.Log("15 - 14.30");
                    if (IngameInformationManager.Instance.currentTotalEnemiesIngame < 30)
                    {
                        Wave01();
                    }
                }
            }
            else if (CountdownTimer.Instance.TimeLeft > CountdownTimer.Instance.MinutesToSeconds(14))
            {
                waveIndex = 2;

                if (Wait(8.0f))
                {
                    if (IngameInformationManager.Instance.currentTotalEnemiesIngame < 60)
                    {
                        Wave01();
                    }
                }
            }
            else if (CountdownTimer.Instance.TimeLeft > CountdownTimer.Instance.MinutesToSeconds(13))
            {
                waveIndex = 3;

                if (Wait(8.0f))
                {
                    Debug.Log("14 - 13.00");
                    if (IngameInformationManager.Instance.currentTotalEnemiesIngame < 70)
                    {
                        Wave02();
                    }

                    if (Random.Range(0f, 1f) < 0.15f)
                    {
                        SpawnFilledCircleEnemy((Vector2)players[0].position + Random.insideUnitCircle.normalized * 50f, EnemyType.BlackBat);
                    }
                }
            }
            else if (CountdownTimer.Instance.TimeLeft > CountdownTimer.Instance.MinutesToSeconds(12))
            {
                waveIndex = 4;
                if (Wait(7.0f))
                {
                    if (IngameInformationManager.Instance.currentTotalEnemiesIngame < 70)
                    {
                        Wave03();
                    }

                    if (Random.Range(0f, 1f) < 0.3f)
                    {
                        SpawnFilledCircleEnemy((Vector2)players[0].position + Random.insideUnitCircle.normalized * 50f, EnemyType.BlackBat, 9, 9);
                    }
                }
            }
            else if (CountdownTimer.Instance.TimeLeft > CountdownTimer.Instance.MinutesToSeconds(10))
            {
                waveIndex = 5;

                if (Wait(7.0f))
                {
                    if (IngameInformationManager.Instance.currentTotalEnemiesIngame < 70)
                    {
                        Wave03();
                    }

                    if (Random.Range(0f, 1f) < 0.3f)
                    {
                        SpawnFilledCircleEnemy((Vector2)players[0].position + Random.insideUnitCircle.normalized * 50f, EnemyType.BlackBat, 9, 9);
                    }
                }
            }
            else if (CountdownTimer.Instance.TimeLeft > CountdownTimer.Instance.MinutesToSeconds(8))
            {
                waveIndex = 6;

                if (Wait(6.0f))
                {
                    if (IngameInformationManager.Instance.currentTotalEnemiesIngame < 100)
                    {
                        Wave04();
                    }

                    if (Random.Range(0f, 1f) < 0.5f)
                    {
                        SpawnFilledCircleEnemy((Vector2)players[0].position + Random.insideUnitCircle.normalized * 50f, EnemyType.BlackBat, 5, 5);

                        StartCoroutine(WaitAfter(0.3f, () =>
                        {
                            SpawnFilledCircleEnemy((Vector2)players[0].position + Random.insideUnitCircle.normalized * 50f, EnemyType.BlackBat, 5, 5);
                        }));
                    }
                }
            }
            else if (CountdownTimer.Instance.TimeLeft > CountdownTimer.Instance.MinutesToSeconds(6))
            {
                waveIndex = 7;

                if (Wait(5.0f))
                {
                    if (IngameInformationManager.Instance.currentTotalEnemiesIngame < 130)
                    {
                        Wave05();
                    }

                    if (Random.Range(0f, 1f) < 0.5f)
                    {
                        SpawnFilledCircleEnemy((Vector2)players[0].position + Random.insideUnitCircle.normalized * 50f, EnemyType.BlackBat, 10, 10);

                        StartCoroutine(WaitAfter(0.3f, () =>
                        {
                            SpawnFilledCircleEnemy((Vector2)players[0].position + Random.insideUnitCircle.normalized * 50f, EnemyType.BlackBat, 10, 10);
                        }));
                    }

                    if (Random.Range(0f, 1f) < 0.3f)
                    {
                        SpawnEnemiesInCircle(players[0].position, EnemyType.Bat, 30, 30);
                    }
                }
            }
            else if (CountdownTimer.Instance.TimeLeft > CountdownTimer.Instance.MinutesToSeconds(4))
            {
                waveIndex = 8;

                if (Wait(5.0f))
                {
                    if (IngameInformationManager.Instance.currentTotalEnemiesIngame < 150)
                    {
                        Wave06();
                    }

                    if (Random.Range(0f, 1f) < 0.5f)
                    {
                        SpawnFilledCircleEnemy((Vector2)players[0].position + Random.insideUnitCircle.normalized * 50f, EnemyType.BlackBat, 10, 10);

                        StartCoroutine(WaitAfter(0.3f, () =>
                        {
                            SpawnFilledCircleEnemy((Vector2)players[0].position + Random.insideUnitCircle.normalized * 50f, EnemyType.BlackBat, 10, 10);
                        }));

                        StartCoroutine(WaitAfter(0.6f, () =>
                        {
                            SpawnFilledCircleEnemy((Vector2)players[0].position + Random.insideUnitCircle.normalized * 50f, EnemyType.BlackBat, 10, 10);
                        }));

                    }

                    if (Random.Range(0f, 1f) < 0.3f)
                    {
                        SpawnEnemiesInCircle(players[0].position, EnemyType.Bat, 30, 30);
                    }
                }
            }
            else if (CountdownTimer.Instance.TimeLeft > CountdownTimer.Instance.MinutesToSeconds(2))
            {
                waveIndex = 9;

                if (Wait(5.0f))
                {
                    if (IngameInformationManager.Instance.currentTotalEnemiesIngame < 150)
                    {
                        Wave07();
                    }

                    if (Random.Range(0f, 1f) < 0.75f)
                    {
                        SpawnFilledCircleEnemy((Vector2)players[0].position + Random.insideUnitCircle.normalized * 50f, EnemyType.BlackBat, 10, 10);

                        StartCoroutine(WaitAfter(0.3f, () =>
                        {
                            SpawnFilledCircleEnemy((Vector2)players[0].position + Random.insideUnitCircle.normalized * 50f, EnemyType.BlackBat, 10, 10);
                        }));
                    }

                    if (Random.Range(0f, 1f) < 0.6f)
                    {
                        SpawnEnemiesInCircle(players[0].position, EnemyType.Bat, 30, 30);
                    }
                }
            }
            else if (CountdownTimer.Instance.TimeLeft > 0.001f)
            {
                waveIndex = 10;

                if (Wait(4.0f))
                {
                    if (IngameInformationManager.Instance.currentTotalEnemiesIngame < 150)
                    {
                        Wave08();
                    }

                    if (Random.Range(0f, 1f) < 0.5f)
                    {
                        SpawnEnemiesInCircle(players[0].position, EnemyType.Bat, 20, 30);
                        StartCoroutine(WaitAfter(0.5f, () =>
                        {
                            SpawnEnemiesInCircle(players[0].position, EnemyType.Bat, 30, 50);
                        }));
                    }
                }
            }
            else
            {
                _isSummonKingSlime = true;
                Vector2 aroundPlayerPosition = (Vector2)players[0].position + (Random.insideUnitCircle.normalized * 50f);
                if (aroundPlayerPosition.y < 15)
                {
                    aroundPlayerPosition = new Vector2(aroundPlayerPosition.x, 15);
                }
                var bossEnemy = SpawnBoss(BossType.KingSlime, aroundPlayerPosition, Quaternion.identity);
                UIBossHealthBar.Instance.SetBossHealthValue(bossEnemy.GetComponent<KingSlime>());

                UIManager.Instance.bossHealthBarCanvas.enabled = true;
            }
        }

        private void Wave01(int quantityLeft = 7, int quantityRight = 7)
        {
            GenerateLeftWaveEnemies(players[0].position, EnemyType.PinkSlime, quantityLeft);
            GenerateRightWaveEnemies(players[0].position, EnemyType.PinkSlime, quantityRight);
        }

        private void Wave02()
        {
            GenerateLeftWaveEnemies(players[0].position, EnemyType.PinkSlime);
            GenerateRightWaveEnemies(players[0].position, EnemyType.PinkSlime);

            if (Random.Range(0f, 1f) < 0.33f)
            {
                GenerateLeftWaveEnemies(players[0].position + new Vector3(-3, 0, 0), EnemyType.BlueSlime);
                GenerateRightWaveEnemies(players[0].position + new Vector3(3, 0, 0), EnemyType.BlueSlime);
            }
        }

        private void Wave03()
        {
            GenerateLeftWaveEnemies(players[0].position, EnemyType.BlueSlime);
            GenerateRightWaveEnemies(players[0].position, EnemyType.BlueSlime);

            if (Random.Range(0f, 1f) < 0.33f)
            {
                GenerateLeftWaveEnemies(players[0].position + new Vector3(-3, 0, 0), EnemyType.MotherSlime, Random.Range(0, 2));
                GenerateRightWaveEnemies(players[0].position + new Vector3(3, 0, 0), EnemyType.MotherSlime, Random.Range(0, 2));
            }
        }

        private void Wave04()
        {
            GenerateLeftWaveEnemies(players[0].position, EnemyType.BlueSlime);
            GenerateRightWaveEnemies(players[0].position, EnemyType.BlueSlime);

            if (Random.Range(0f, 1f) < 0.33f)
            {
                GenerateLeftWaveEnemies(players[0].position + new Vector3(-3, 0, 0), EnemyType.MotherSlime, Random.Range(3, 5));
                GenerateRightWaveEnemies(players[0].position + new Vector3(3, 0, 0), EnemyType.MotherSlime, Random.Range(3, 5));
            }
        }

        private void Wave05()
        {
            GenerateLeftWaveEnemies(players[0].position, EnemyType.Bonehead, Random.Range(2, 5));
            GenerateRightWaveEnemies(players[0].position, EnemyType.Bonehead, Random.Range(2, 5));

            if (Random.Range(0f, 1f) < 0.33f)
            {
                GenerateLeftWaveEnemies(players[0].position + new Vector3(-3, 0, 0), EnemyType.Cursedwraith, Random.Range(0, 2));
                GenerateRightWaveEnemies(players[0].position + new Vector3(3, 0, 0), EnemyType.Cursedwraith, Random.Range(0, 2));
            }
        }

        private void Wave06()
        {
            GenerateLeftWaveEnemies(players[0].position, EnemyType.Bonehead, Random.Range(4, 7));
            GenerateRightWaveEnemies(players[0].position, EnemyType.Bonehead, Random.Range(4, 7));

            if (Random.Range(0f, 1f) < 0.5f)
            {
                GenerateLeftWaveEnemies(players[0].position + new Vector3(-3, 0, 0), EnemyType.Cursedwraith, Random.Range(1, 3));
                GenerateRightWaveEnemies(players[0].position + new Vector3(3, 0, 0), EnemyType.Cursedwraith, Random.Range(1, 3));
            }

            if (Random.Range(0f, 1f) < 0.33f)
            {
                GenerateLeftWaveEnemies(players[0].position + new Vector3(-3, 0, 0), EnemyType.ObsidianMaw, Random.Range(2, 3));
                GenerateRightWaveEnemies(players[0].position + new Vector3(3, 0, 0), EnemyType.ObsidianMaw, Random.Range(2, 3));
            }
        }

        private void Wave07()
        {
            GenerateLeftWaveEnemies(players[0].position, EnemyType.Bonehead, Random.Range(4, 7));
            GenerateRightWaveEnemies(players[0].position, EnemyType.Bonehead, Random.Range(4, 7));

            if (Random.Range(0f, 1f) < 0.5f)
            {
                GenerateLeftWaveEnemies(players[0].position + new Vector3(-3, 0, 0), EnemyType.Cursedwraith, Random.Range(1, 3));
                GenerateRightWaveEnemies(players[0].position + new Vector3(3, 0, 0), EnemyType.Cursedwraith, Random.Range(1, 3));
            }

            if (Random.Range(0f, 1f) < 0.5f)
            {
                GenerateLeftWaveEnemies(players[0].position + new Vector3(-3, 0, 0), EnemyType.ObsidianMaw, Random.Range(4, 7));
                GenerateRightWaveEnemies(players[0].position + new Vector3(3, 0, 0), EnemyType.ObsidianMaw, Random.Range(4, 7));
            }
        }

        private void Wave08()
        {
            GenerateLeftWaveEnemies(players[0].position, EnemyType.ObsidianMaw);
            GenerateRightWaveEnemies(players[0].position, EnemyType.ObsidianMaw);

            GenerateLeftWaveEnemies(players[0].position, EnemyType.Bonehead);
            GenerateRightWaveEnemies(players[0].position, EnemyType.Bonehead);

            if (Random.Range(0f, 1f) < 0.7f)
            {
                GenerateLeftWaveEnemies(players[0].position + new Vector3(-3, 0, 0), EnemyType.Cursedwraith, Random.Range(2, 3));
                GenerateRightWaveEnemies(players[0].position + new Vector3(3, 0, 0), EnemyType.Cursedwraith, Random.Range(2, 3));
            }
        }


        private IEnumerator WaitAfter(float time, System.Action callback)
        {
            yield return new WaitForSeconds(time);
            callback?.Invoke();
        }


        private void Spawn(EnemyType enemyType, Vector2 position, Quaternion rotation)
        {
            var enemyPrefab = GetEnemyPrefabByType(enemyType);
            var enemyObject = Instantiate(enemyPrefab, position, rotation);

            enemyObject.GetComponent<BaseEnemy>().SetFollowTarget(players[0]);

            var enemyNetworkObject = enemyObject.GetComponent<NetworkObject>();
            enemyNetworkObject.Spawn();
            IngameInformationManager.Instance.currentTotalEnemiesIngame++;
        }

        private BaseEnemy SpawnBoss(BossType bossType, Vector2 position, Quaternion rotation)
        {
            var bossPrefab = GameDataManager.Instance.GetBossPrefab(bossType);
            var bossObject = Instantiate(bossPrefab, position, rotation);

            bossObject.GetComponent<BaseEnemy>().SetFollowTarget(players[0]);

            var enemyNetworkObject = bossObject.GetComponent<NetworkObject>();
            enemyNetworkObject.Spawn();
            IngameInformationManager.Instance.currentTotalEnemiesIngame++;

            return bossObject.GetComponent<BaseEnemy>();
        }


        public void GenerateLeftWaveEnemies(Vector2 playerPosition, EnemyType enemyType, int quantity = 7)
        {
            if (quantity >= 7)
            {
                for (int i = 0; i < spawnerPointsLeft.Count; i++)
                {
                    float x = playerPosition.x + spawnerPointsLeft[i].position.x;
                    float y = spawnerPointsLeft[i].position.y;

                    Spawn(enemyType, new Vector2(x, y), Quaternion.identity);

                }
            }
            else if (quantity < 7 && quantity > 0)
            {
                for (int i = 0; i < quantity; i++)
                {
                    float x = playerPosition.x + spawnerPointsLeft[Random.Range(0, quantity)].position.x;
                    float y = spawnerPointsLeft[Random.Range(0, quantity)].position.y;

                    Spawn(enemyType, new Vector2(x, y), Quaternion.identity);
                }
            }
            else
            {
                return;
            }
        }

        public void GenerateRightWaveEnemies(Vector2 playerPosition, EnemyType enemyType, int quantity = 7)
        {
            if (quantity >= 7)
            {
                for (int i = 0; i < spawnerPointsLeft.Count; i++)
                {
                    float x = playerPosition.x + spawnerPointsRight[i].position.x;
                    float y = spawnerPointsLeft[i].position.y;

                    Spawn(enemyType, new Vector2(x, y), Quaternion.identity);

                }
            }
            else if (quantity < 7 || quantity > 0)
            {
                for (int i = 0; i < quantity; i++)
                {
                    float x = playerPosition.x + spawnerPointsRight[Random.Range(0, quantity)].position.x;
                    float y = spawnerPointsLeft[Random.Range(0, quantity)].position.y;

                    Spawn(enemyType, new Vector2(x, y), Quaternion.identity);

                }
            }
            else
            {
                return;
            }

        }

        private void SpawnFilledCircleEnemy(Vector2 spawnPosition, EnemyType enemyType, int width = 7, int height = 7)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Spawn(enemyType, spawnPosition + new Vector2(x, y), Quaternion.identity);

                }
            }
        }


        public void SpawnEnemiesInCircle(Vector2 playerPosition, EnemyType enemyType, int numOfEnemy, float radius)
        {
            float angle = 0.0f;
            float angleIncrement = (Mathf.PI * 2.0f) / numOfEnemy;

            for (int i = 0; i < numOfEnemy; i++)
            {
                float x = Mathf.Cos(angle) * radius;
                float y = Mathf.Sin(angle) * radius;
                angle += angleIncrement;


                Spawn(enemyType, playerPosition + new Vector2(x, y), Quaternion.identity);
            }
        }


        public void GenerateCircleWaveEnemies(EnemyType enemyType, Vector2 centerPoint, int numOfEnemy, float radius)
        {
            float angle = 0.0f;
            float angleIncrement = (Mathf.PI * 2.0f) / numOfEnemy;

            for (int i = 0; i < numOfEnemy; i++)
            {
                float x = centerPoint.x + Mathf.Cos(angle) * radius;
                float y = centerPoint.y + Mathf.Sin(angle) * radius;
                angle += angleIncrement;

                Spawn(enemyType, new Vector2(x, y), Quaternion.identity);
            }
        }


        private float spacing = 10;
        private int pointsPerSide = 3;
        public void GenerateSquareWaveEnemies(EnemyType enemyType, Transform playerTransform, int numOfEnemy, float radius)
        {
            float x = -spacing * (numOfEnemy / 2);
            float y = spacing * (numOfEnemy / 2);

            for (int i = 0; i < numOfEnemy; i++)
            {
                Spawn(enemyType, playerTransform.position + new Vector3(x, y, 0), Quaternion.identity);

                if ((i + 1) % pointsPerSide == 0)
                {
                    x = -spacing * (numOfEnemy / 2);
                    y -= spacing;
                }
                else
                {
                    x += spacing;
                }
            }
        }

        private bool Wait(float waitTime)
        {
            if (Time.time - _timeElapse > waitTime)
            {
                _timeElapse = Time.time;
                return true;
            }
            return false;
        }
    }
}
