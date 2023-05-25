using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System.Collections;
using System.Runtime.CompilerServices;

namespace Sataura
{
    public class PlayerUseItem : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private Player _player;
        private CharacterData _characterData;
        [SerializeField] private PlayerLoot _playerLoot;
        private PlayerInGameSkills _playerInGameSkills;
        private PlayerMovement _playerMovement;
        private ItemEvolutionManager _itemEvolutionManager;
        private IngameInformationManager _ingameInformationManager;

        [Header("Properties")]
        public float updateInterval = 0.05f; // the interval between updates in seconds
        private float lastUpdateTime; // the time the method was last called
        private bool IsBootEvo;
        [SerializeField] private LayerMask enemyLayer;
        private bool[] canUseItems;

        // Enemy detection
        private float detectionInterval = 0.5f;
        private float lastDetectionTime = 0f;
        [SerializeField] private Collider2D[] enemies = new Collider2D[5]; // Array to store results of the overlap check
        Vector2 nearestEnemy;
        Vector2 direction;


        [Header("Passive Items")]
        [SerializeField] List<Item> passiveItems = new List<Item>();



        #region Properties
        public Collider2D[] Enemies { get { return enemies; } }
        #endregion

        public override void OnNetworkSpawn()
        {
            _characterData = _player.characterData;
            _playerInGameSkills = _player.playerIngameSkills;
            _playerMovement = _player.playerMovement;
            _itemEvolutionManager = ItemEvolutionManager.Instance;
            _ingameInformationManager = IngameInformationManager.Instance;

            Invoke(nameof(SettingsWhenSpawnPlayer), 1f);

            canUseItems = new bool[_playerInGameSkills.weaponsData.itemSlots.Count];
            // Set all canuseItems array to false at start.
            for (int i = 0; i < canUseItems.Length; i++)
            {
                canUseItems[i] = true;
            }
        }





        private void SettingsWhenSpawnPlayer()
        {
            ClearAllPassiveItemObjectInInventory();
            CreateAllPassiveItemObjectInInventory();
        }



        #region Use Passive Item
        public void CreateAllPassiveItemObjectInInventory()
        {
            for (int i = 0; i < _playerInGameSkills.weaponsData.itemSlots.Count; i++)
            {
                if (_playerInGameSkills.weaponsData.itemSlots[i].HasItemData() == false)
                    continue;


                var itemPrefab = GameDataManager.Instance.GetItemPrefab(_playerInGameSkills.weaponsData.itemSlots[i].ItemData.itemType);
                if (itemPrefab != null)
                {
                    var obj = Instantiate(itemPrefab, transform.position, Quaternion.identity);
                    var itemObj = obj.GetComponent<Item>();
                    itemObj.SetData(_playerInGameSkills.weaponsData.itemSlots[i]);
                    itemObj.spriteRenderer.enabled = false;
                    itemObj.GetComponent<NetworkObject>().Spawn();
                    passiveItems.Add(itemObj);

                    // Set parent
                    // =========
                    itemObj.transform.SetParent(_player.transform);
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





        private void Update()
        {
            if (_ingameInformationManager.IsGameOver())
                return;


            // Check if it's time to detect enemies again      
            if (Time.time - lastDetectionTime > detectionInterval)
            {
                lastDetectionTime = Time.time;

                // Detect enemies within the specified radius
                nearestEnemy = DetectNearestEnemyPosition();
            }


            if (passiveItems.Count == 0) return;


            for (int i = 0; i < passiveItems.Count; i++)
            {
                // Check if the item can be used

                if (canUseItems[i])
                {
                    // Use the item
                    // ...
                    passiveItems[i].Use(_player, nearestEnemy);

                    // Set the canUseItem flag to false and start the coroutine to wait for the interval
                    canUseItems[i] = false;
                    float usageTime = 1.0f / passiveItems[i].ItemData.rateOfFire;
                    StartCoroutine(UseItemAfterInterval(i, usageTime));
                }
            }
        }




        public Vector2 DetectNearestEnemyPosition()
        {
            // Array to store results of the overlap check
            int numEnemies = Physics2D.OverlapCircleNonAlloc(transform.position, (_characterData._currentAware * 30 / 100), enemies, enemyLayer);

            if (numEnemies == 0)
            {
                numEnemies = Physics2D.OverlapCircleNonAlloc(transform.position, _characterData._currentAware, enemies, enemyLayer);

                if (numEnemies > 0)
                {
                    return enemies[Random.Range(0, numEnemies)].transform.position;
                }
                else
                {
                    return GetRandomVector2() + (Vector2)_player.transform.position;
                }

            }
            else
            {
                return enemies[Random.Range(0, numEnemies)].transform.position;
            }
        }

        public Transform DetectNearestEnemy()
        {
            // Array to store results of the overlap check
            int numEnemies = Physics2D.OverlapCircleNonAlloc(transform.position, (_characterData._currentAware * 30 / 100), enemies, enemyLayer);

            if (numEnemies == 0)
            {
                numEnemies = Physics2D.OverlapCircleNonAlloc(transform.position, _characterData._currentAware, enemies, enemyLayer);

                if (numEnemies > 0)
                {
                    return enemies[Random.Range(0, numEnemies)].transform;
                }
                else
                {
                    return null;
                }

            }
            else
            {
                return enemies[Random.Range(0, numEnemies)].transform;
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



        #endregion Use Passive Item




        private void OnDrawGizmos()
        {
            if (_characterData == null) return;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _characterData._currentAware * 30 / 100);

            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, _characterData._currentAware);
        }
    }

}