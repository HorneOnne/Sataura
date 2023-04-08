using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace Sataura
{
    /// <summary>
    /// Represents an item that is currently held in the player's hand.
    /// </summary>
    public class ItemInHand : NetworkBehaviour
    {
        public PlayerType playerType;

        [Header("References")]
        [SerializeField] private Player player;
        [SerializeField] private ItemSelectionPlayer itemSelectionPlayer;

        private PlayerInputHandler playerInputHandler;


        /// <summary>
        /// The item slot that contains the item data and item quantity in this hand.
        /// </summary>
        [SerializeField] private ItemSlot itemSlot;


        /// <summary>
        /// The UI element that displays the item in this hand.
        /// </summary>
        private UIItemInHand uiItemInHand;

        /// <summary>
        /// The current item that is held in this hand.
        /// </summary>
        [SerializeField] private Item currentItemObject;
        public NetworkVariable<int> currentHoldHandItemID = new NetworkVariable<int>(-1);


        /// <summary>
        /// Indicates whether this is the first time that the item has been used.
        /// </summary>
        [Header("Settings")]
        [Tooltip("Prevent using a selected item for the first time.")]
        private bool isFirstTimeUsingItem = true;


        /// <summary>
        /// The time elapsed since the last mouse press.
        /// </summary>
        private float lastUsedTime = 0.0f;

        /// <summary>
        /// Whether the current item being used is being used for the first time.
        /// </summary>
        private bool firstUseItem = true;

        #region Properties
        /// <summary>
        /// The item slot data that the item was obtained from.
        /// </summary>
        public ItemSlotData ItemGetFrom { get; private set; }

        #endregion


        private void OnEnable()
        {
            if (playerType == PlayerType.IngamePlayer)
            {
                if (player.canUseItem)
                {
                    EventManager.OnItemInHandChanged += ReInstantiateItem;
                    currentHoldHandItemID.OnValueChanged += OnInHandItemIDChanged;
                    player.PlayerInputHandler.currentUseItemIndex.OnValueChanged += OnCurrentUseIndexItemIDChanged;


                    // Optimize
                    UIUpgradeSkill.OnUpgradeButtonClicked += UpdateCurrentItemObjectData;
                }
            }

            if (playerType == PlayerType.ItemSelectionPlayer)
            {

            }

        }


        private void OnDisable()
        {
            if (playerType == PlayerType.IngamePlayer)
            {
                if (player.canUseItem)
                {
                    EventManager.OnItemInHandChanged -= ReInstantiateItem;
                    currentHoldHandItemID.OnValueChanged -= OnInHandItemIDChanged;
                    player.PlayerInputHandler.currentUseItemIndex.OnValueChanged -= OnCurrentUseIndexItemIDChanged;


                    // Optimize
                    UIUpgradeSkill.OnUpgradeButtonClicked -= UpdateCurrentItemObjectData;
                }
            }
            
        }



        public override void OnNetworkSpawn()
        {
            uiItemInHand = UIItemInHand.Instance;

            if (playerType == PlayerType.IngamePlayer)
                playerInputHandler = player.PlayerInputHandler;

            if (playerType == PlayerType.ItemSelectionPlayer)
                playerInputHandler = itemSelectionPlayer.PlayerInputHandler;
        }


        public void UpdateCurrentItemObjectData()
        {
            Debug.LogWarning("Optimize here.");
            if (currentItemObject != null)
            {
                var playerIngameInventory = player.PlayerInGameInventory;
                currentItemObject.SetData(playerIngameInventory.inGameInventory[playerInputHandler.currentUseItemIndex.Value]);
            }
        }

        public bool HasItemObjectInServer(InventoryData inventoryData)
        {
            if (currentHoldHandItemID.Value != -1)
            {
                return true;
            }


            if (playerInputHandler.currentUseItemIndex.Value != -1)
            {
                if (inventoryData.itemSlots[playerInputHandler.currentUseItemIndex.Value].HasItemData())
                {
                    return true;
                }
            }


            return false;
        }

        public bool HasHandHoldItemInServer()
        {
            return currentHoldHandItemID.Value != -1;
        }

        public bool HasUseItemIndexInServer()
        {
            return playerInputHandler.currentUseItemIndex.Value != -1;
        }

        private void Update()
        {
            if (!IsOwner) return;

            if (playerType != PlayerType.IngamePlayer)
                return;

            if (player.handleItem)
            {
                /*if (currentItemObject != null)
                {
                    if (currentItemObject.showIconWhenHoldByHand == true)
                    {
                        // Mouse
                        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        RotateHoldItemServerRpc(mousePosition);

                        // Controller
                        //RotateHoldItemServerRpc(player.PlayerInputHandler.RotateWeaponInput);
                    }
                }*/

                if (HasItemObjectInServer(player.PlayerInGameInventory.inGameInventoryData))
                {
                    //if (currentItemObject.showIconWhenHoldByHand == true)
                    //{
                    // Mouse
                    Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    RotateHoldItemServerRpc(mousePosition);

                    // Controller
                    //RotateHoldItemServerRpc(player.PlayerInputHandler.RotateWeaponInput);
                    //}
                }

            }
        }

        /// <summary>
        /// Rotate hold item if it can be shown
        /// </summary>
        [ServerRpc]
        private void RotateHoldItemServerRpc(Vector3 mousePosition)
        {
            // Mouse
            Utilities.RotateObjectTowardMouse2D(mousePosition, player.HandHoldItem, 0);

            // Controller
            //Utilities.RotateObjectTowardDirection2D(mousePosition, player.HandHoldItem, 0);
        }

        /// <summary>
        /// Gets the item data for the item in this hand.
        /// </summary>
        /// <returns>The item data for the item in this hand.</returns>
        public ItemData GetItemData()
        {
            return itemSlot.ItemData;
        }


        /// <summary>
        /// Sets the item in this hand to the specified item slot.
        /// </summary>
        /// <param name="takenSlot">The item slot containing the item to set.</param>
        /// <param name="slotIndex">The index of the item slot.</param>
        /// <param name="storageType">The storage type of the item slot.</param>
        /// <param name="forceUpdateUI">Whether to force an update of the UI.</param>
        /// <param name="parent">The parent transform of the item.</param>
        public void SetItem(ItemSlot takenSlot, int slotIndex = -1, StoredType storageType = StoredType.Another, bool forceUpdateUI = false, Transform parent = null)
        {
            itemSlot = new ItemSlot(takenSlot);
            ItemGetFrom = new ItemSlotData
            {
                slotIndex = slotIndex,
                slotStoredType = storageType
            };

            isFirstTimeUsingItem = true;
            EventManager.TriggerItemInHandChangedEvent();

            if (forceUpdateUI)
            {
                uiItemInHand.UpdateItemInHandUI(parent);
            }
        }

        [ServerRpc]
        public void SetItemServerRpc(ulong clientId, ItemSlotStruct itemSlotStruct, int slotIndex = -1, StoredType storageType = StoredType.Another, bool forceUpdateUI = false)
        {
            ItemData itemData = GameDataManager.Instance.GetItemData(itemSlotStruct.itemID);
            int itemQuantity = itemSlotStruct.itemQuantity;
            ItemSlot itemSlot = new ItemSlot(itemData, itemQuantity);
            SetItem(itemSlot, slotIndex, storageType, false);

            ClientRpcParams clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { clientId }
                }
            };

            SetItemClientRpc(itemSlotStruct, slotIndex, storageType, forceUpdateUI, clientRpcParams);
        }

        [ClientRpc]
        public void SetItemClientRpc(ItemSlotStruct itemSlotStruct, int slotIndex = -1, StoredType storageType = StoredType.Another, bool forceUpdateUI = false, ClientRpcParams clientRpcParams = default)
        {
            ItemData itemData = GameDataManager.Instance.GetItemData(itemSlotStruct.itemID);
            int itemQuantity = itemSlotStruct.itemQuantity;
            ItemSlot itemSlot = new ItemSlot(itemData, itemQuantity);
            SetItem(itemSlot, slotIndex, storageType, forceUpdateUI);
        }


        #region MOUSE EVENT METHODS HANDLER
        /// <summary>
        /// Swaps the item in this hand with an item in the specified inventory.
        /// </summary>
        /// <param name="inventory">The inventory to swap with.</param>
        /// <param name="slotIndex">The index of the item slot to swap with.</param>
        /// <param name="storageType">The storage type of the item slot.</param>
        /// <param name="forceUpdateUI">Whether to force an update of the UI.</param>
        /// <param name="parent">The parent transform of the item.</param>
        public void Swap(ref List<ItemSlot> inventory, int slotIndex, StoredType storageType = StoredType.Another, bool forceUpdateUI = false, Transform parent = null)
        {
            var inHandSlotTemp = new ItemSlot(this.itemSlot);
            var invSlotChoosen = inventory[slotIndex];

            this.itemSlot = new ItemSlot(invSlotChoosen);
            inventory[slotIndex] = new ItemSlot(inHandSlotTemp);

            ItemGetFrom = new ItemSlotData
            {
                slotIndex = slotIndex,
                slotStoredType = storageType
            };


            isFirstTimeUsingItem = true;
            EventManager.TriggerItemInHandChangedEvent();

            if (forceUpdateUI)
            {
                uiItemInHand.UpdateItemInHandUI(parent);
            }
        }

        /// <summary>
        /// Swaps the current item with the item at the given slot index in the specified inventory.
        /// </summary>
        /// <param name="inventory">The inventory array to swap with.</param>
        /// <param name="slotIndex">The index of the item slot to swap with.</param>
        /// <param name="storageType">The type of storage where the swap occurs. Default is StoredType.Another.</param>
        /// <param name="forceUpdateUI">Whether to force update the UI after the swap. Default is false.</param>
        /// <param name="parent">The parent Transform for the UI. Default is null.</param>
        public void Swap(ref ItemSlot[] inventory, int slotIndex, StoredType storageType = StoredType.Another, bool forceUpdateUI = false, Transform parent = null)
        {
            var inHandSlotTemp = new ItemSlot(this.itemSlot);
            var invSlotChoosen = inventory[slotIndex];

            this.itemSlot = new ItemSlot(invSlotChoosen);
            inventory[slotIndex] = new ItemSlot(inHandSlotTemp);

            ItemGetFrom = new ItemSlotData
            {
                slotIndex = slotIndex,
                slotStoredType = storageType
            };


            isFirstTimeUsingItem = true;
            EventManager.TriggerItemInHandChangedEvent();

            if (forceUpdateUI)
            {
                uiItemInHand.UpdateItemInHandUI(parent);
            }
        }


        [ServerRpc]
        public void SwapServerRpc(ulong clientId, int slotIndex, StoredType storageType = StoredType.Another, bool forceUpdateUI = false)
        {
            if(playerType == PlayerType.IngamePlayer)
            {
                Swap(ref player.PlayerInGameInventory.inGameInventory, slotIndex, storageType, forceUpdateUI);

                // NOTE! In case you know a list of ClientId's ahead of time, that does not need change,
                // Then please consider caching this (as a member variable), to avoid Allocating Memory every time you run this function
                ClientRpcParams clientRpcParams = new ClientRpcParams
                {
                    Send = new ClientRpcSendParams
                    {
                        TargetClientIds = new ulong[] { clientId }
                    }
                };
                SwapClientRpc(slotIndex, storageType, forceUpdateUI, clientRpcParams);
            }


            if (playerType == PlayerType.ItemSelectionPlayer)
            {
                Swap(ref itemSelectionPlayer.PlayerInGameInventory.inGameInventory, slotIndex, storageType, forceUpdateUI);

                // NOTE! In case you know a list of ClientId's ahead of time, that does not need change,
                // Then please consider caching this (as a member variable), to avoid Allocating Memory every time you run this function
                ClientRpcParams clientRpcParams = new ClientRpcParams
                {
                    Send = new ClientRpcSendParams
                    {
                        TargetClientIds = new ulong[] { clientId }
                    }
                };
                SwapClientRpc(slotIndex, storageType, forceUpdateUI, clientRpcParams);
            }
        }


        [ClientRpc]
        private void SwapClientRpc(int slotIndex, StoredType storageType = StoredType.Another, bool forceUpdateUI = false, ClientRpcParams clientRpcParams = default)
        {
            if (!IsOwner || IsServer) return;

            Debug.Log("Server call client");

            if (playerType == PlayerType.IngamePlayer)
                Swap(ref player.PlayerInGameInventory.inGameInventory, slotIndex, storageType, forceUpdateUI);

            if (playerType == PlayerType.ItemSelectionPlayer)
                Swap(ref itemSelectionPlayer.PlayerInGameInventory.inGameInventory, slotIndex, storageType, forceUpdateUI);

            UIPlayerInGameInventory.Instance.UpdateInventoryUI();

        }

        /// <summary>
        /// Splits the item slot quantity in the specified inventory at the given slot index.
        /// </summary>
        /// <param name="inventory">The inventory array to split the item quantity in.</param>
        /// <param name="slotIndex">The index of the item slot to split the quantity of.</param>
        public void SplitItemSlotQuantityInInventory(ref List<ItemSlot> inventory, int slotIndex)
        {
            int itemQuantity = inventory[slotIndex].ItemQuantity;
            if (itemQuantity > 1)
            {
                int splitItemQuantity = itemQuantity / 2;
                inventory[slotIndex].SetItemQuantity(itemQuantity - splitItemQuantity);

                var chosenSlot = new ItemSlot(inventory[slotIndex]);
                chosenSlot.SetItemQuantity(splitItemQuantity);
                SetItem(chosenSlot, slotIndex, StoredType.PlayerInventory, true);
            }
            else
            {
                Swap(ref inventory, slotIndex, StoredType.PlayerInventory, true);
            }
        }


        /// <summary>
        /// Splits the item slot quantity in the specified inventory at the given slot index.
        /// </summary>
        /// <param name="inventory">The inventory array to split the item quantity in.</param>
        /// <param name="slotIndex">The index of the item slot to split the quantity of.</param>
        public void SplitItemSlotQuantityInInventory(ref ItemSlot[] inventory, int slotIndex)
        {
            int itemQuantity = inventory[slotIndex].ItemQuantity;
            if (itemQuantity > 1)
            {
                int splitItemQuantity = itemQuantity / 2;
                inventory[slotIndex].SetItemQuantity(itemQuantity - splitItemQuantity);

                var chosenSlot = new ItemSlot(inventory[slotIndex]);
                chosenSlot.SetItemQuantity(splitItemQuantity);
                SetItem(chosenSlot, slotIndex, StoredType.PlayerInventory, true);
            }
            else
            {
                Swap(ref inventory, slotIndex, StoredType.PlayerInventory, true);
            }
        }


        [ServerRpc]
        public void SplitItemSlotQuantityInInventoryServerRpc(ulong clientId, int slotIndex)
        {
            List<ItemSlot> inventory = player.PlayerInGameInventory.inGameInventory;
            SplitItemSlotQuantityInInventory(ref player.PlayerInGameInventory.inGameInventory, slotIndex);

            ClientRpcParams clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { clientId }
                }
            };
            SplitItemSlotQuantityInInventoryClientRpc(slotIndex, clientRpcParams);
        }


        [ClientRpc]
        private void SplitItemSlotQuantityInInventoryClientRpc(int slotIndex, ClientRpcParams clientRpcParams = default)
        {
            if (!IsOwner || IsServer) return;
            SplitItemSlotQuantityInInventory(ref player.PlayerInGameInventory.inGameInventory, slotIndex);
            UIPlayerInGameInventory.Instance.UpdateInventoryUI();
        }


        /// <summary>
        /// Attempts to pick up an item from the given ItemSlot in the world and places it in the player's hand.
        /// </summary>
        /// <param name="itemContainerSlot">The ItemSlot in the world to pick up the item from.</param>
        /// <returns>A boolean indicating whether the item was successfully picked up or not.</returns>
        public bool PickupItem(ref ItemSlot itemContainerSlot)
        {
            Debug.Log("PickupItem");

            bool canPickupItem;


            if (HasItemData() == false)
            {
                itemSlot = new ItemSlot(itemContainerSlot);
                itemContainerSlot.ClearSlot();
                uiItemInHand.UpdateItemInHandUI();
                canPickupItem = true;
            }
            else
            {
                if (itemSlot.ItemData.Equals(itemContainerSlot.ItemData))
                {
                    if (itemSlot.TryAddItem(itemContainerSlot) == true)
                    {
                        itemContainerSlot = itemSlot.AddItemsFromAnotherSlot(itemContainerSlot);
                        uiItemInHand.UpdateItemInHandUI();
                        canPickupItem = true;
                    }
                    else
                    {
                        //Debug.Log("The item quantity > item maxquantity");
                        canPickupItem = false;
                    }
                }
                else
                {
                    //Debug.Log("Not same");
                    canPickupItem = false;
                }
            }

            EventManager.TriggerItemInHandChangedEvent();
            return canPickupItem;
        }
        #endregion MOUSE EVENT METHODS HANDLER


        /// <summary>
        /// Sets the current item to the specified item.
        /// </summary>
        /// <param name="item">The item to set as the current item.</param>
        [ServerRpc]
        public void SetHandHoldItemIDServerRpc(int itemID)
        {
            currentHoldHandItemID.Value = itemID;
        }


        /// <summary>
        /// Gets the current item object.
        /// </summary>
        /// <returns>The current item.</returns>
        public Item GetItemObject()
        {
            return currentItemObject;
        }


        /// <summary>
        /// Checks whether the player currently has an item data in their hand or not.
        /// </summary>
        /// <returns>A boolean indicating whether the player currently has an item data in their hand or not.</returns>
        public bool HasItemData()
        {
            if (itemSlot == null)
                return false;

            return itemSlot.HasItemData();
        }


        /// <summary>
        /// Checks whether the player currently has an item object in their hand or not.
        /// </summary>
        /// <returns>A boolean indicating whether the player currently has an item object in their hand or not.</returns>
        public bool HasItemObject()
        {
            return currentItemObject != null;
        }


        public ItemSlot GetSlot() => itemSlot;


        /// <summary>
        /// Removes the item from the player's hand.
        /// </summary>
        public void ClearSlot()
        {
            itemSlot.ClearSlot();
            ItemGetFrom = new ItemSlotData
            {
                slotStoredType = StoredType.Another,
                slotIndex = -1
            };

            EventManager.TriggerItemInHandChangedEvent();
        }


        /// <summary>
        /// Use item if left mouse button is pressed
        /// </summary>
        [ServerRpc]
        public void UseItemServerRpc(Vector2 mousePosition)
        {
            if (IsMouseOverUI() == true)
                return;

            if(playerType != PlayerType.IngamePlayer) return;

            if (HasItemObjectInServer(player.PlayerInGameInventory.inGameInventoryData))
            {
                // Check if it's time to attack
                if (Time.time - lastUsedTime >= 1.0f / (currentItemObject.ItemData.usageVelocity + 0.001f))
                {
                    lastUsedTime = Time.time;
                    UseItem(mousePosition);
                }
                else if (firstUseItem)
                {
                    bool canUseItem = UseItem(mousePosition);
                    if (canUseItem)
                    {
                        firstUseItem = false;
                        lastUsedTime = Time.time;
                    }
                }
            }
        }


        /// <summary>
        /// Uses the item currently in the player's hand.
        /// </summary>
        /// <returns>A boolean indicating whether the item was successfully used or not.</returns>
        public bool UseItem(Vector2 mousePosition)
        {
            if (playerType != PlayerType.IngamePlayer) return false;

            if (isFirstTimeUsingItem)
            {
                isFirstTimeUsingItem = false;
                return false;
            }

            return currentItemObject.Use(player, mousePosition);
        }


        /// <summary>
        /// Removes one quantity of the item in the player's hand.
        /// </summary>
        public void RemoveItem()
        {
            itemSlot.RemoveItem();
            uiItemInHand.UpdateItemInHandUI();

            EventManager.TriggerItemInHandChangedEvent();
        }


        [ServerRpc]
        public void RemoveItemServerRpc(ulong clientId)
        {
            RemoveItem();

            ClientRpcParams clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { clientId }
                }
            };

            RemoveItemClientRpc(clientRpcParams);
        }


        [ClientRpc]
        private void RemoveItemClientRpc(ClientRpcParams clientRpcParams = default)
        {
            if (!IsOwner || IsServer) return;

            RemoveItem();

            UIItemInHand.Instance.UpdateItemInHandUI();
        }


        /// <summary>
        /// Checks whether the player's mouse is currently over a UI element or not.
        /// </summary>
        /// <returns>A boolean indicating whether the player's mouse is currently over a UI element or not.</returns>
        private bool IsMouseOverUI()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                // mouse click is on a UI element.
                return true;
            }
            else
            {
                // mouse click is not on a UI element
                return false;
            }
        }


        /// <summary>
        /// Handles the re-instantiation of the item in the player's hand. Destroys any existing items in the player's hand and
        /// creates a new item object in the hand if the item inventory slot is not empty.
        /// </summary>
        private void ReInstantiateItem()
        {
            if (!IsOwner) return;
            DespawnCurrentInHandNetworkObjectServerRpc();

            bool hasItemData = HasItemData();
            if (hasItemData)
            {
                int itemID = GameDataManager.Instance.GetItemID(itemSlot.ItemData);
                SetHandHoldItemIDServerRpc(itemID);
            }
            else
            {
                SetHandHoldItemIDServerRpc(-1);
            }
        }



        [ServerRpc]
        private void DespawnCurrentInHandNetworkObjectServerRpc()
        {
            if (currentItemObject == null)
            {
                //SetHandHoldItemIDServerRpc(-1);
                return;
            }


            NetworkObject networkObject = currentItemObject.GetComponent<NetworkObject>();
            if (networkObject != null && networkObject.IsSpawned && IsServer)
            {
                networkObject.Despawn();
            }

            //SetHandHoldItemIDServerRpc(-1);
        }


        [ServerRpc]
        private void InstantitateCurrentItemNetworkObjectServerRpc(ulong clientId, int itemID, int itemQuantity)
        {
            if (playerType != PlayerType.IngamePlayer) return;

            currentItemObject = Utilities.InstantiateItemNetworkObject(itemID, itemQuantity, player.HandHoldItem);
            currentItemObject.GetComponent<NetworkObject>().Spawn(true);
            currentItemObject.GetComponent<NetworkObject>().TrySetParent(player.HandHoldItem);
            currentItemObject.transform.localPosition = Vector3.zero;
            currentItemObject.transform.localScale = Vector3.one;
        }





        private void OnInHandItemIDChanged(int oldID, int newID)
        {
            if (!IsOwner) return;
            if (newID == -1) return;

            InstantitateCurrentItemNetworkObjectServerRpc(NetworkManager.LocalClientId, newID, 1);
        }




        // Testing
        // ==============================================================
        private void OnCurrentUseIndexItemIDChanged(int oldID, int newID)
        {
            if (!IsOwner) return;
            DespawnCurrentInHandNetworkObjectServerRpc();

            if (playerInputHandler.currentUseItemIndex.Value == -1)
            {
                return;
            }


            int itemID = GameDataManager.Instance.GetItemID(player.PlayerInGameInventory.inGameInventory[playerInputHandler.currentUseItemIndex.Value].ItemData);
            if (itemID == -1) return;

            InstantitateCurrentItemNetworkObjectServerRpc(NetworkManager.LocalClientId, itemID, 1);
        }
    }
}

