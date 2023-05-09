using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace Sataura
{
    public class PlayerSkills : NetworkBehaviour
    {
        [SerializeField] private ItemSelectionPlayer _player;
        private ItemInHand itemInHand;


        [Header("INVENTORY SETTINGS")]
        // The list of all item slots in the inventory.
        public List<ItemSlot> weapons;
        public List<ItemSlot> accessories;


        [Header("Runtime References")]
        public InventoryData weaponsData;
        public InventoryData accessoriesData;

       
        #region Properties
        public int Capacity 
        { 
            get 
            {
                if (weaponsData == null)
                    return 0;
                else
                    return weaponsData.itemSlots.Count; 
            } 
        }

        #endregion


        public override void OnNetworkSpawn()
        {
            if(IsOwner || IsServer)
            {
                itemInHand = _player.itemInHand;

                StartCoroutine(UpdateCharacterData());
            }          
        }

        public IEnumerator UpdateCharacterData()
        {
            yield return new WaitUntil(() => _player.characterData != null);

            weaponsData = _player.characterData.weaponsData;
            weapons = weaponsData.itemSlots;

            accessoriesData = _player.characterData.accessoriesData;
            accessories = accessoriesData.itemSlots;
        }


   

        public ItemData GetItem(int slotIndex)
        {
            if (HasSlot(slotIndex))
            {
                return weapons[slotIndex].ItemData;
            }
            return null;
        }

        public bool AddWeapons(ItemData itemData)
        {
            if(itemData.itemCategory != ItemCategory.Skill_Weapons)
            {
                Debug.LogWarning($"Item {itemData.itemName} is not a weapon.");
                return false;
            }    

            bool canAddItem = false;

            for (int i = 0; i < weapons.Count; i++)
            {
                if (weapons[i].HasItemData() == false)
                {
                    weapons[i].AddNewItem(itemData);
                    canAddItem = true;
                    break;
                }
                else
                {
                    if (weapons[i].ItemData == itemData)
                    {
                        bool canAdd = weapons[i].AddItem();

                        if (canAdd == true)
                        {
                            canAddItem = true;
                            break;
                        }
                    }
                }
            }
                           
            return canAddItem;
        }

        public bool AddAccessories(ItemData itemData)
        {
            if (itemData.itemCategory != ItemCategory.Skill_Accessories)
            {
                Debug.LogWarning($"Item {itemData.itemName} is not a accessory.");
                return false;
            }

            bool canAddItem = false;

            for (int i = 0; i < accessories.Count; i++)
            {
                if (accessories[i].HasItemData() == false)
                {
                    accessories[i].AddNewItem(itemData);
                    canAddItem = true;
                    break;
                }
                else
                {
                    if (accessories[i].ItemData == itemData)
                    {
                        bool canAdd = accessories[i].AddItem();

                        if (canAdd == true)
                        {
                            canAddItem = true;
                            break;
                        }
                    }
                }
            }

            return canAddItem;
        }


        /// <summary>
        /// Adds an item slot to the player's inventory, starting from the first available empty slot
        /// </summary>
        /// <param name="itemSlot">The item slot to be added to the player's inventory</param>
        /// <returns>The item slot after it has been added to the inventory</returns>
        public ItemSlot AddItem(ItemSlot itemSlot)
        {
            ItemSlot copyItemSlot = new ItemSlot(itemSlot);
            ItemSlot returnItemSlot = new ItemSlot(itemSlot);

            for (int i = 0; i < weapons.Count; i++)
            {
                returnItemSlot = weapons[i].AddItemsFromAnotherSlot(copyItemSlot);

                if (returnItemSlot.HasItemData() == false)
                {
                    break;
                }

            }
            return returnItemSlot;
        }


        /// <summary>
        /// Adds an item to the inventory slot at the given index.
        /// </summary>
        /// <param name="index">The index of the inventory slot where the item should be added</param>
        /// <returns>True if the inventory slot is not full after adding the item, false otherwise</returns>
        public bool AddItemAt(int index)
        {
            bool isSlotNotFull = weapons[index].AddItem();
            EventManager.TriggerInventoryUpdatedEvent();

            return isSlotNotFull;
        }


        [ServerRpc]
        public void AddItemAtServerRpc(ulong clientId, int index)
        {
            bool isSlotNotFull = AddItemAt(index);

            if (isSlotNotFull)
            {
                itemInHand.RemoveItem();
            }

            ClientRpcParams clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { clientId }
                }
            };

            AddItemAtClientRpc(index);
        }

        [ClientRpc]
        public void AddItemAtClientRpc(int index, ClientRpcParams clientRpcParams = default)
        {
            if (!IsOwner || IsServer) return;

            bool isSlotNotFull = AddItemAt(index);

            if (isSlotNotFull)
            {
                itemInHand.RemoveItem();
            }
        }

        /// <summary>
        /// Adds a new item to the inventory slot at the given index.
        /// </summary>
        /// <param name="index">The index of the inventory slot where the new item should be added</param>
        /// <param name="item">The item data for the new item to be added</param>
        public void AddNewItemAt(int index, ItemData item)
        {
            weapons[index].AddNewItem(item);
            EventManager.TriggerInventoryUpdatedEvent();
        }

        [ServerRpc]
        public void AddNewItemAtServerRpc(ulong clientId, int index, int itemID)
        {
            ItemData itemData = GameDataManager.Instance.GetItemData(itemID);
            AddNewItemAt(index, itemData);

            ClientRpcParams clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { clientId }
                }
            };

            AddNewItemAtClientRpc(index, itemID, clientRpcParams);

        }

        [ClientRpc]
        private void AddNewItemAtClientRpc(int index, int itemID, ClientRpcParams clientRpcParams = default)
        {
            if (!IsOwner || IsServer) return;

            ItemData itemData = GameDataManager.Instance.GetItemData(itemID);
            AddNewItemAt(index, itemData);

            UIPlayerInGameSkills.Instance.UpdateUIWeaponAt(index);
        }


        /// <summary>
        /// Removes the item from the inventory slot at the given index.
        /// </summary>
        /// <param name="index">The index of the inventory slot where the item should be removed</param>
        public void RemoveItemAt(int index)
        {
            weapons[index].RemoveItem();
            EventManager.TriggerInventoryUpdatedEvent();
        }


        /// <summary>
        /// Returns true if the inventory slot at the given index exists, false otherwise
        /// </summary>
        /// <param name="slotIndex">The index of the inventory slot to check for existence</param>
        /// <returns>True if the inventory slot exists, false otherwise</returns>
        public bool HasSlot(int slotIndex)
        {
            try
            {
                weapons[slotIndex].HasItemData();
            }
            catch
            {
                return false;
            }

            return true;
        }


        /// <summary>
        /// Returns true if the inventory slot at the given index has an item in it, false otherwise
        /// </summary>
        /// <param name="slotIndex">The index of the inventory slot to check for an item</param>
        /// <returns>True if the inventory slot has an item, false otherwise</returns>
        public bool HasItem(int slotIndex)
        {
            if (HasSlot(slotIndex))
            {
                return weapons[slotIndex].HasItemData();
            }
            return false;
        }
    }

}
