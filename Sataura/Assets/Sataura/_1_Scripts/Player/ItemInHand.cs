using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace Sataura
{
    public class ItemInHand : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private ItemSelectionPlayer itemSelectionPlayer;

        private InputHandler playerInputHandler;


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



        #region Properties
        /// <summary>
        /// The item slot data that the item was obtained from.
        /// </summary>
        public ItemSlotData ItemGetFrom { get; private set; }

        #endregion




        public override void OnNetworkSpawn()
        {
            uiItemInHand = UIItemInHand.Instance;
            playerInputHandler = itemSelectionPlayer.playerInputHandler;

        }



        public bool HasHandHoldItemInServer()
        {
            return currentHoldHandItemID.Value != -1;
        }



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

            EventManager.TriggerItemInHandChangedEvent();

            if (forceUpdateUI)
            {
                uiItemInHand.UpdateItemInHandUI(parent);
            }
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
        /// Removes one quantity of the item in the player's hand.
        /// </summary>
        public void RemoveItem()
        {
            itemSlot.RemoveItem();
            uiItemInHand.UpdateItemInHandUI();

            EventManager.TriggerItemInHandChangedEvent();
        }
    }
}

