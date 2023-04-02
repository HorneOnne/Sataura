using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

namespace Sataura
{
    public class PlayerUseItem : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private Player player;
        private PlayerInGameInventory playerInGameInv;
        private PlayerMovement playerMovement;
        
        [Header("Passive Items")]
        [SerializeField] List<Item> passiveItems = new List<Item>();


        [Header("Item Properties")]
        [SerializeField] private BootData bootData;


        public override void OnNetworkSpawn()
        {
            playerInGameInv = player.PlayerInGameInventory;
            playerMovement = player.PlayerMovement;

            Invoke(nameof(SettingsWhenSpawnPlayer), 1f);     
        }

        private void SetBootsEquipProperties(BootData _bootData = null)
        {
            bootData = _bootData;

            playerMovement.SetMovementSpeed(bootData);
            playerMovement.SetJumpForce(bootData);

        }

        private void SettingsWhenSpawnPlayer()
        {
            ClearAllPassiveItemObjectInInventory();
            CreateAllPassiveItemObjectInInventory();
        }



        #region Use Passive Item
        public void CreateAllPassiveItemObjectInInventory()
        {
            for(int i = 0; i < playerInGameInv.inGameInventory.Count; i++)
            {
                if (playerInGameInv.inGameInventory[i].HasItemData() == false)
                    continue;

                if (playerInGameInv.inGameInventory[i].ItemData is BootData)
                {
                    Debug.Log("Has boots data");
                    SetBootsEquipProperties((BootData)playerInGameInv.inGameInventory[i].ItemData);
                    
                }
                    

                var itemPrefab = GameDataManager.Instance.GetItemPrefab($"IP_{playerInGameInv.inGameInventory[i].ItemData.itemType}");
                if(itemPrefab != null)
                {
                    Debug.Log(itemPrefab.name);
                    var obj = Instantiate(itemPrefab, transform.position, Quaternion.identity);
                    var itemObj = obj.GetComponent<Item>();
                    itemObj.SetData(playerInGameInv.inGameInventory[i]);
                    itemObj.spriteRenderer.enabled = false;
                    itemObj.GetComponent<NetworkObject>().Spawn();
                    passiveItems.Add(itemObj);
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


        public float updateInterval = 0.05f; // the interval between updates in seconds
        private float lastUpdateTime; // the time the method was last called
        private void Update()
        { 
            if (Time.time - lastUpdateTime > updateInterval)
            {
                lastUpdateTime = Time.time;
                // run update logic here
                //......
                for (int i = 0; i < passiveItems.Count; i++)
                {
                    if(ItemEvolutionManager.Instance.IsEvoItem(passiveItems[i].ItemData))
                    {
                        passiveItems[i].UsePassive(player, Vector2.zero);
                    }                    
                }
            }

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

                if (playerInGameInv.inGameInventory[i].ItemData.Equals(ItemEvolutionManager.Instance.GetEvolutionItem(baseItem)))
                {
                    return true;
                }

            }
            return false;
        }
        // END Item level skill methods
        // ========================
    }

}