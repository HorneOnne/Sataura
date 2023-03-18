using Sataura;
using UnityEngine;
using Unity.Netcode;
using UnityEditor.PackageManager;

public class MaxEnemiesTest : NetworkBehaviour
{
    [SerializeField] Transform prefab;

    private void Start()
    {
        float xPos;
        float yPos;

        for (int i = 0; i < 5000; i++)
        {
            xPos = Random.Range(-400, 400);
            yPos = Random.Range(-400, 400);

            var e = Instantiate(prefab, new Vector2(xPos, yPos), Quaternion.identity);
            WorldGrid.Instance.AddCurrency(new Vector2(xPos, yPos), e.GetComponent<Currency>());
        }

        /*for(int i = 0; i < WorldGrid.Instance.chunkList.Count; i++)
        {
            WorldGrid.Instance.HideChunk(WorldGrid.Instance.chunkList[i].IndexPosition);
        }*/
    }

    /*public Transform player;
    public override void OnNetworkSpawn()
    {
        var clientId = NetworkManager.Singleton.LocalClientId;
        player = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.transform;
    }*/

    Vector2 mousePos;
    Vector2 direction;
    float distance;

    float timeElapse = 0.0f;
    float getEnemiesAroundTimeElapse = 0.0f;
    public LayerMask enemyLayer;
    Enemy001 enemy;
    Collider2D[] enemies;


    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Vector2Int chunkIndex = WorldGrid.Instance.WorldToIndexPosition(mousePos);

            for(int y = chunkIndex.y - 1; y <= chunkIndex.y + 1; y++)
            {
                for (int x = chunkIndex.x - 1; x <= chunkIndex.y + 1; x++)
                {
                    Vector2Int index = new Vector2Int(x, y);
                    WorldGrid.Instance.ShowChunk(index);
                }
            }

        }
    }

    /*private void FixedUpdate()
    {
        //if (player == null) return;

        if(Time.time - getEnemiesAroundTimeElapse >= 2.0f)
        {
            getEnemiesAroundTimeElapse = Time.time;
            enemies = Physics2D.OverlapCircleAll(mousePos, 150, enemyLayer);
        }

        if (Time.time - timeElapse >= 0.035f)
        {
            timeElapse = Time.time;

            if (enemies == null) return;

            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //mousePos = player.position;
            int size = enemies.Length;

            for (int i = 0; i < size; i++)
            {
                enemy = enemies[i].GetComponent<Enemy001>();
                distance = Vector2.Distance(mousePos, enemy.Rb2D.position);

                if (distance < 1f)
                {
  
                    continue;
                }
                else if (distance < 400)
                {           
                    direction = mousePos - enemy.Rb2D.position;
                    direction.Normalize();
                    enemy.Rb2D.MovePosition(enemy.Rb2D.position + direction * 10 * Time.fixedDeltaTime);
                }
            }     
        }           
    }*/


    /*List<Enemy001> enemiesList = new List<Enemy001>(); 
    private void FixedUpdate()
    {

        if (Time.time - getEnemiesAroundTimeElapse >= 2.0f)
        {
            *//*getEnemiesAroundTimeElapse = Time.time;
            enemies = Physics2D.OverlapCircleAll(mousePos, 200, enemyLayer);*//*

            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            enemiesList.Clear();

            Vector2Int worldIndex = WorldGrid.Instance.WorldToIndexPosition(mousePos);

            for (int y = worldIndex.y - 1; y <= worldIndex.y + 1; y++)
            {
                for (int x = worldIndex.x - 1; x <= worldIndex.x + 1; x++)
                {
                    Vector2Int key = new Vector2Int(x, y);
                    WorldGrid.Instance.EnablePhysicsSimulationAtGrid(key);
                    enemiesList.AddRange(WorldGrid.Instance.worldGrid[key].enemies);
                }
            }

            getEnemiesAroundTimeElapse = Time.time;
            //enemies = Physics2D.OverlapCircleAll(mousePos, 200, enemyLayer);
            
        }

        if (Time.time - timeElapse >= 0.035f)
        {
            timeElapse = Time.time;

            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int size = enemiesList.Count;

            for (int i = 0; i < size; i++)
            {
                enemy = enemiesList[i].GetComponent<Enemy001>();
                distance = Vector2.Distance(mousePos, enemy.rb2D.position);

                if (distance < 1f)
                {

                    continue;
                }
                else if (distance < 400)
                {


                    direction = mousePos - enemy.rb2D.position;
                    direction.Normalize();
                    enemy.rb2D.MovePosition(enemy.rb2D.position + direction * 10 * Time.fixedDeltaTime);
                }
            }
        }
    }*/





}
