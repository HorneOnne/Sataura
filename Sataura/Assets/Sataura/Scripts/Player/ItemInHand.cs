using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using static UnityEditor.Progress;
using UnityEditor.PackageManager;

namespace Sataura
{
    /// <summary>
    /// Represents an item that is currently held in the player's hand.
    /// </summary>
    public class ItemInHand : NetworkBehaviour
    {
        [Header("References")]
        /// <summary>
        /// The player who is holding this item.
        /// </summary>
        [SerializeField] private Player player;


        /// <summary>
        /// The item slot that contains the item data and item quantity in this hand.
        /// </summary>
        [SerializeField] private ItemSlot itemSlot;
        [SerializeField] private ItemSlotStruct itemSlotStruct;


        /// <summary>
        /// The UI element that displays the item in this hand.
        /// </summary>
        private UIItemInHand uiItemInHand;

        /// <summary>
        /// The current item that is held in this hand.
        /// </summary>
        [SerializeField] private Item currentItem;
        public NetworkVariable<int> currentItemID = new NetworkVariable<int>(-1);


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
            if (player.canUseItem)
            {
                EventManager.OnItemInHandChanged += ReInstantiateItem;

                currentItemID.OnValueChanged += OnItemIDChanged;
            }

        }

        private void OnDisable()
        {
            if (player.canUseItem)
            {
                EventManager.OnItemInHandChanged -= ReInstantiateItem;

                currentItemID.OnValueChanged -= OnItemIDChanged;
            }
        }

        private void Start()
        {
            uiItemInHand = UIItemInHand.Instance;
        }


        private void Update()
        {
            if (!IsOwner) return;

            if (player.handleItem)
            {
                if (Input.GetKeyDown(KeyCode.C))
                {
                    Debug.Log(HasItem());
                }

                if (currentItemID.Value != -1)
                {
                    Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    RotateHoldItemServerRpc(mousePosition);
                }

            }
        }

        /// <summary>
        /// Rotate hold item if it can be shown
        /// </summary>
        [ServerRpc]
        private void RotateHoldItemServerRpc(Vector3 mousePosition)
        {
            if (GetICurrenttem() == null) return;
            if (GetICurrenttem().showIconWhenHoldByHand == true)
            {
                Utilities.RotateObjectTowardMouse2D(mousePosition, player.HandHoldItem, 0);
            }
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
            Debug.Log("HandleSwapInGameInventoryServerRpc");
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


        [ClientRpc]
        private void SwapClientRpc(int slotIndex, StoredType storageType = StoredType.Another, bool forceUpdateUI = false, ClientRpcParams clientRpcParams = default)
        {
            if (!IsOwner || IsServer) return;

            Debug.Log("Server call client");

            Swap(ref player.PlayerInGameInventory.inGameInventory, slotIndex, storageType, forceUpdateUI);
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
        /// Sets the current item to the specified item.
        /// </summary>
        /// <param name="item">The item to set as the current item.</param>
        [ServerRpc]
        public void SetCurrentItemIDServerRpc(int itemID)
        {
            currentItemID.Value = itemID;
        }


        /// <summary>
        /// Gets the current item.
        /// </summary>
        /// <returns>The current item.</returns>
        public Item GetICurrenttem()
        {
            return currentItem;
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


        /// <summary>
        /// Checks whether the player currently has an item data in their hand or not.
        /// </summary>
        /// <returns>A boolean indicating whether the player currently has an item data in their hand or not.</returns>
        public bool HasItemData()
        {
            if (itemSlot == null)
                return false;

            return itemSlot.HasItem();
        }


        /// <summary>
        /// Checks whether the player currently has an item object in their hand or not.
        /// </summary>
        /// <returns>A boolean indicating whether the player currently has an item object in their hand or not.</returns>
        public bool HasItem()
        {
            return currentItem != null;
        }


        /// <summary>
        /// Gets the ItemSlot currently in the player's hand.
        /// </summary>
        /// <returns>The ItemSlot currently in the player's hand.</returns>
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
            if (HasItemData() && GetICurrenttem() != null)
            {
                // Check if it's time to attack
                if (Time.time - lastUsedTime >= 1.0f / (GetItemData().usageVelocity + 0.001f))
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
            if (HasItemData() == false || HasItem() == false) return false;
            if (IsMouseOverUI() == true) return false;

            if (isFirstTimeUsingItem)
            {
                isFirstTimeUsingItem = false;
                return false;
            }

            return currentItem.Use(player, mousePosition);
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
            //firstUseItem = true;

            /*if (player.HandHoldItem.childCount != 0)
            {
                for (int i = 0; i < player.HandHoldItem.childCount; i++)
                {
                    Destroy(player.HandHoldItem.GetChild(i).gameObject);
                }
            }
            SetICurrentItem(null);


            if (HasItemData())
            {
                var itemObject = Utilities.InstantiateItemObject(GetSlot(), player.HandHoldItem);
                itemObject.GetComponent<NetworkObject>().Spawn(true);
                itemObject.GetComponent<NetworkObject>().TrySetParent(player.HandHoldItem);
                SetICurrentItem(itemObject.GetComponent<Item>());
                
                if (itemObject.showIconWhenHoldByHand)
                {
                    itemObject.spriteRenderer.enabled = true;
                }
                else
                    itemObject.spriteRenderer.enabled = false;
            }*/

            if (!IsOwner) return;

            DespawnCurrentInHandNetworkObjectServerRpc();
            bool hasItemData = HasItemData();
            if (hasItemData)
            {
                int itemID = GameDataManager.Instance.GetItemID(itemSlot.ItemData);
                int itemQuantity = itemSlot.ItemQuantity;
                SetCurrentItemIDServerRpc(itemID);
            }


        }

        [ServerRpc]
        private void DespawnCurrentInHandNetworkObjectServerRpc()
        {
            if (currentItem == null)
            {
                SetCurrentItemIDServerRpc(-1);
                return;
            }


            NetworkObject networkObject = currentItem.GetComponent<NetworkObject>();
            if (networkObject != null && networkObject.IsSpawned && IsServer)
            {
                networkObject.Despawn();
            }
            SetCurrentItemIDServerRpc(-1);
        }


        [ServerRpc]
        private void InstantitateNetworkObjectServerRpc(int itemID, int itemQuantity)
        {
            currentItem = Utilities.InstantiateItemNetworkObject(itemID, itemQuantity, player.HandHoldItem);
            currentItem.GetComponent<NetworkObject>().Spawn(true);
            currentItem.GetComponent<NetworkObject>().TrySetParent(player.HandHoldItem);
            currentItem.transform.localPosition = Vector3.zero;
            currentItem.transform.localScale = Vector3.one;


            /*if (currentItem.showIconWhenHoldByHand)
            {
                currentItem.spriteRenderer.enabled = true;
            }
            else
                currentItem.spriteRenderer.enabled = false;*/
        }

        private void OnItemIDChanged(int oldID, int newID)
        {
            if (!IsOwner) return;
            if (newID == -1) return;

            InstantitateNetworkObjectServerRpc(newID, 1);
        }


    }
}

