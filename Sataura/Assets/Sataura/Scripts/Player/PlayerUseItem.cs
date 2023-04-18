using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System.Collections;

namespace Sataura
{
    public class PlayerUseItem : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private Player player;
        [SerializeField] private PlayerLoot playerLoot;
        private PlayerInGameInventory playerInGameInv;
        private PlayerMovement playerMovement;
        private ItemEvolutionManager itemEvolutionManager;
        private IngameInformationManager ingameInformationManager;

        [Header("Properties")]
        public float updateInterval = 0.05f; // the interval between updates in seconds
        private float lastUpdateTime; // the time the method was last called

        [Header("Passive Items")]
        [SerializeField] List<Item> passiveItems = new List<Item>();


        [Header("Runtime References")]
        [SerializeField] private BootData bootData;
        [SerializeField] private Boots bootsGO;
        [SerializeField] private MagnetStoneData magnetStoneData;
        [SerializeField] private MagnetStone magnetStoneGO;

        private bool IsBootEvo;

        private PlayerInputAction playerInputAction;


        private bool[] canUseItems;
        [SerializeField] private LayerMask enemyLayer;

        public override void OnNetworkSpawn()
        {
            playerInGameInv = player.PlayerInGameInventory;
            playerMovement = player.PlayerMovement;
            itemEvolutionManager = ItemEvolutionManager.Instance;
            ingameInformationManager = IngameInformationManager.Instance;

            Invoke(nameof(SettingsWhenSpawnPlayer), 1f);

            playerInputAction = new PlayerInputAction();
            playerInputAction.Player.Enable();
            playerInputAction.Player.Jump.performed += DoubleJumpHandle;


            canUseItems = new bool[playerInGameInv.Capacity];
            Debug.Log(canUseItems.Length);
            // Set all canuseItems array to false at start.
            for (int i = 0; i < canUseItems.Length; i++)
            {
                canUseItems[i] = true;
            }
        }


        private void SetBootsEquipProperties(BootData _bootData = null)
        {
            bootData = _bootData;

            playerMovement.SetMovementSpeed(bootData);
            playerMovement.SetJumpForce(bootData);
        }

        private void SetMagnetStoneEquipProperties(MagnetStoneData _magnetStoneData = null)
        {
            this.magnetStoneData = _magnetStoneData;

            playerLoot.SetLootRadius(magnetStoneData.lootRadius);
        }

        private void SettingsWhenSpawnPlayer()
        {
            ClearAllPassiveItemObjectInInventory();
            CreateAllPassiveItemObjectInInventory();
        }



        #region Use Passive Item
        public void CreateAllPassiveItemObjectInInventory()
        {
            for (int i = 0; i < playerInGameInv.inGameInventory.Count; i++)
            {
                if (playerInGameInv.inGameInventory[i].HasItemData() == false)
                    continue;


                var itemPrefab = GameDataManager.Instance.GetItemPrefab($"IP_{playerInGameInv.inGameInventory[i].ItemData.itemType}");
                if (itemPrefab != null)
                {
                    var obj = Instantiate(itemPrefab, transform.position, Quaternion.identity);
                    var itemObj = obj.GetComponent<Item>();
                    itemObj.SetData(playerInGameInv.inGameInventory[i]);
                    itemObj.spriteRenderer.enabled = false;
                    itemObj.GetComponent<NetworkObject>().Spawn();               
                    passiveItems.Add(itemObj);

                    // Set parent
                    // =========
                    itemObj.transform.SetParent(player.transform);
                }

                if (playerInGameInv.inGameInventory[i].ItemData is BootData)
                {
                    Debug.Log($"Has boots data: {i}");
                    SetBootsEquipProperties((BootData)playerInGameInv.inGameInventory[i].ItemData);
                    IsBootEvo = itemEvolutionManager.IsEvoItem(bootData);

                    for (int j = 0; j < passiveItems.Count; j++)
                    {
                        if (passiveItems[j] is Boots)
                        {
                            bootsGO = passiveItems[j].GetComponent<Boots>();
                            break;
                        }
                    }
                }

                if (playerInGameInv.inGameInventory[i].ItemData is MagnetStoneData)
                {
                    Debug.Log($"Has MagnetStone data: {i}");
                    SetMagnetStoneEquipProperties((MagnetStoneData)playerInGameInv.inGameInventory[i].ItemData);

                    /*IsBootEvo = itemEvolutionManager.IsEvoItem(bootData);
                    for (int j = 0; j < passiveItems.Count; j++)
                    {
                        if (passiveItems[j] is Boots)
                        {
                            bootsGO = passiveItems[j].GetComponent<Boots>();
                            break;
                        }
                    }*/

                }

            }
        }

        public void ClearAllPassiveItemObjectInInventory()
        {
            for (int i = 0; i < passiveItems.Count; i++)
            {
                passiveItems[i].GetComponent<NetworkObject>().Despawn();
            }
            passiveItems.Clear();
        }



        [SerializeField] private float detectionRadius = 5f;
        private float detectionInterval = 1.0f;
        private float lastDetectionTime = 0f;
        [SerializeField] private Collider2D[] enemies = new Collider2D[5]; // Array to store results of the overlap check

        Vector2 nearestEnemy;
        Vector2 direction;

        private void Update()
        {
            if (ingameInformationManager.IsGameOver())
                return;

            // Check if it's time to detect enemies again      
            if (Time.time - lastDetectionTime > detectionInterval)
            {
                lastDetectionTime = Time.time;

                // Detect enemies within the specified radius
                nearestEnemy = DetectNearestEnemy();
            }


            if (passiveItems.Count == 0) return;


            for (int i = 0; i < passiveItems.Count; i++)
            {
                // Check if the item can be used

                if (canUseItems[i])
                {
                    // Use the item
                    // ...
                    passiveItems[i].Use(player, nearestEnemy);

                    // Set the canUseItem flag to false and start the coroutine to wait for the interval
                    canUseItems[i] = false;
                    float usageTime = 1.0f / passiveItems[i].ItemData.usageVelocity;
                    StartCoroutine(UseItemAfterInterval(i, usageTime));
                }
            }


            

        }


        /*private Transform DetectNearestEnemy(float detecionRadius)
        {
            Transform nearestEnemyTransform;

            // Array to store results of the overlap check
            int numEnemies = Physics2D.OverlapCircleNonAlloc(transform.position, detectionRadius, enemies);
            int index = -1;
            // Find the shortest squared distance to the player
            float shortestSquaredDistance = Mathf.Infinity;
            for (int i = 0; i < numEnemies; i++)
            {
                float squaredDistanceToPlayer = (enemies[i].transform.position - transform.position).sqrMagnitude;
                if (squaredDistanceToPlayer < shortestSquaredDistance)
                {
                    shortestSquaredDistance = squaredDistanceToPlayer;
                    index = i;
                }
            }

            nearestEnemyTransform = enemies[index].transform;
            return nearestEnemyTransform;
        }*/

        private Vector2 DetectNearestEnemy()
        {
            // Array to store results of the overlap check
            int numEnemies = Physics2D.OverlapCircleNonAlloc(transform.position, detectionRadius, enemies, enemyLayer);
            if(numEnemies > 0)
            {
                return enemies[Random.Range(0, numEnemies)].transform.position;
            }
            else
            {
                return GetRandomVector2() + (Vector2)player.transform.position;
            }        
        }


        private IEnumerator UseItemAfterInterval(int slotIndex, float useInterval)
        {
            // Wait for the interval before allowing the item to be used again
            yield return new WaitForSeconds(useInterval);
            canUseItems[slotIndex] = true;
        }

        private Vector2 GetRandomVector2()
        {
            float randomX = Random.Range(-10.0f, 10.0f);
            float randomY = Random.Range(-10.0f, 10.0f);
            return new Vector2(randomX, randomY);
        }

        private void DoubleJumpHandle(InputAction.CallbackContext obj)
        {
            if (IsBootEvo == false) return;
            bootsGO.DoubleJump(player);
        }

        #endregion Use Passive Item

        // START Item level skill methods
        // ========================
        public bool HasBaseItem(ItemData baseItem)
        {
            int inventorySize = playerInGameInv.inGameInventory.Count;
            for (int i = 0; i < inventorySize; i++)
            {
                if (ItemData.IsSameName(playerInGameInv.inGameInventory[i].ItemData, baseItem))
                {
                    return true;
                }

            }
            return false;
        }

        public int FindBaseItemIndex(ItemData baseItem)
        {
            int inventorySize = playerInGameInv.inGameInventory.Count;
            for (int i = 0; i < inventorySize; i++)
            {
                if (ItemData.IsSameName(playerInGameInv.inGameInventory[i].ItemData, baseItem))
                {
                    return i;
                }
            }

            throw new System.Exception($"Not found base item {baseItem} in PlayerplayerIngameInv.inGameInventory.cs.");
        }

        public ItemData GetUpgradeVersionOfItem(ItemData itemData)
        {
            if (itemData.currentLevel < itemData.maxLevel)
            {
                return itemData.upgradeRecipe.outputItemSlot.itemData;
            }
            else
            {
                return null;
            }
        }


        public bool HasEvoOfBaseItem(ItemData baseItem)
        {
            int inventorySize = playerInGameInv.inGameInventory.Count;
            for (int i = 0; i < inventorySize; i++)
            {
                if (playerInGameInv.inGameInventory[i].HasItemData() == false)
                    continue;

                if (playerInGameInv.inGameInventory[i].ItemData.Equals(itemEvolutionManager.GetEvolutionItem(baseItem)))
                {
                    return true;
                }

            }
            return false;
        }
        // END Item level skill methods
        // ========================


        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
    }

}