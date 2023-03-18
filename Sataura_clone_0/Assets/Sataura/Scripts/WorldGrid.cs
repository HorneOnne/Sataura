using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sataura
{
    public class WorldGrid : Singleton<WorldGrid>
    {
        public Dictionary<Vector2Int, Chunk> world = new Dictionary<Vector2Int, Chunk>();
        public List<Chunk> chunkList;

        public void Add(Vector2 worldPosition, Enemy001 enemy)
        {
            Vector2Int indexPos = WorldToIndexPosition(worldPosition);

            if(world.ContainsKey(indexPos) == false)
            {
                world.Add(indexPos, new Chunk());
                world[indexPos].IndexPosition = indexPos;
                world[indexPos].enemies.Add(enemy);

                chunkList.Add(world[indexPos]);
            }
            else
            {
                world[indexPos].enemies.Add(enemy);
            }
        }


        public void AddCurrency(Vector2 worldPosition, Currency currency)
        {
            Vector2Int indexPos = WorldToIndexPosition(worldPosition);

            if (world.ContainsKey(indexPos) == false)
            {
                world.Add(indexPos, new Chunk());
                world[indexPos].IndexPosition = indexPos;
                world[indexPos].currencies.Add(currency);

                chunkList.Add(world[indexPos]);
            }
            else
            {
                world[indexPos].currencies.Add(currency);
            }
        }

        public Vector2Int WorldToIndexPosition(Vector2 worldPosition)
        {
            // Convert to grid position
            int x = Mathf.FloorToInt(worldPosition.x / 100);
            int y = Mathf.FloorToInt(worldPosition.y / 100);

            return new Vector2Int(x, y);
        }


        public void EnablePhysicsSimulationAtGrid(Vector2Int indexPosition)
        {
            if (world.ContainsKey(indexPosition) == false) return;

            int size = world[indexPosition].enemies.Count;
            for(int i =0; i < size; i++)
            {
                world[indexPosition].enemies[i].Rb2D.simulated = true;
            }
        }

        public void SaveAndDisablePhysicsSimulateEnemiesAtGrid(Vector2Int indexPosition)
        {
            /*enemies = Physics2D.OverlapCircleAll(mousePos, 200, enemyLayer);
            if (worldGrid.ContainsKey(indexPosition) == false) return;

            int size = worldGrid[indexPosition].enemies.Count;
            for (int i = 0; i < size; i++)
            {
                worldGrid[indexPosition].enemies[i].rb2D.simulated = true;
            }*/
        }


        public void HideChunk(Vector2Int chunkIndex)
        {
            if (world.ContainsKey(chunkIndex) == false) return;

            int size = world[chunkIndex].currencies.Count;
           
            for (int i = 0; i < size; i++)
            {
                world[chunkIndex].currencies[i].gameObject.SetActive(false);
            }
        }

        public void ShowChunk(Vector2Int chunkIndex)
        {
            if (world.ContainsKey(chunkIndex) == false) return;

            int size = world[chunkIndex].currencies.Count;

            for (int i = 0; i < size; i++)
            {
                world[chunkIndex].currencies[i].gameObject.SetActive(true);
            }
        }

        /*private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                int count = 0;
                for(int i = 0; i < worldList.Count; i++)
                {
                    count += worldList[i].enemies.Count;
                }

                Debug.Log($"total: {count}");
            }
        }*/

    }

    [System.Serializable]
    public class Chunk
    {
        public Vector2Int IndexPosition;
        public List<Enemy001> enemies = new List<Enemy001>();
        public List<Currency> currencies = new List<Currency>();

    }


}


