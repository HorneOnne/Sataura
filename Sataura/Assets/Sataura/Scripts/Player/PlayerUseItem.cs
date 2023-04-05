using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System;

namespace Sataura
{
    public class PlayerUseItem : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private Player player;
        private PlayerInGameInventory playerInGameInv;
        private PlayerMovement playerMovement;
        private ItemEvolutionManager itemEvolutionManager;
        
        [Header("Passive Items")]
        [SerializeField] List<Item> passiveItems = new List<Item>();


        [Header("Item Properties")]
        [SerializeField] private BootData bootData;
        [SerializeField] private Boots bootsGO;
        private bool IsBootEvo;



        private PlayerInputAction playerInputAction;

        public override void OnNetworkSpawn()
        {
            playerInGameInv = player.PlayerInGameInventory;
            playerMovement = player.PlayerMovement;
            itemEvolutionManager = ItemEvolutionManager.Instance;

            Invoke(nameof(SettingsWhenSpawnPlayer), 1f);

            playerInputAction = new PlayerInputAction();
            playerInputAction.Player.Enable();
            playerInputAction.Player.Jump.performed += DoubleJumpHandle;
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
        
                   
                var itemPrefab = GameDataManager.Instance.GetItemPrefab($"IP_{playerInGameInv.inGameInventory[i].ItemData.itemType}");
                if(itemPrefab != null)
                {
                    var obj = Instantiate(itemPrefab, transform.position, Quaternion.identity);
                    var itemObj = obj.GetComponent<Item>();
                    itemObj.SetData(playerInGameInv.inGameInventory[i]);
                    itemObj.spriteRenderer.enabled = false;
                    itemObj.GetComponent<NetworkObject>().Spawn();
                    passiveItems.Add(itemObj);
                }

                if (playerInGameInv.inGameInventory[i].ItemData is BootData)
                {
                    Debug.Log($"Has boots data: {i}");
                    SetBootsEquipProperties((BootData)playerInGameInv.inGameInventory[i].ItemData);
                    IsBootEvo = itemEvolutionManager.IsEvoItem(bootData);

                    for(int j = 0; j < passiveItems.Count; j++)
                    {
                        if (passiveItems[j] is Boots)
                        {
                            bootsGO = passiveItems[j].GetComponent<Boots>();
                            break;
                        }
                    }
                    
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
                    if(itemEvolutionManager.IsEvoItem(passiveItems[i].ItemData))
                    {
                        passiveItems[i].UsePassive(player, Vector2.zero);
                    }                    
                }
            }
      
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
    }

}