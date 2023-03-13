using UnityEngine;
using Unity.Netcode;

namespace Sataura
{
    public abstract class Item : NetworkBehaviour, IDroppable, ICollectible, IUseable
    {
        #region Properties
        [field: SerializeField]
        public ItemData ItemData { get; private set; }
        public ItemSlot ItemSlot { get; private set; }
        protected GameObject Model { get; private set; }
        #endregion


        [Header("References")]
        [HideInInspector] public SpriteRenderer spriteRenderer;

        [Header("Item Properties")]
        [Tooltip("Indicate this item can be shown when held in hand.")]
        public bool showIconWhenHoldByHand;


        public override void OnNetworkSpawn()
        {
            Debug.Log("OnNetworkSpawn");
            LoadComponents();
        }



        protected void LoadComponents()
        {
            Model = GetComponentInChildren<SpriteRenderer>().gameObject;
            spriteRenderer = Model.GetComponent<SpriteRenderer>();
        }


        public void SetData(ItemSlot itemSlot)
        {
            this.ItemSlot = itemSlot;
            this.ItemData = itemSlot.ItemData;
            UpdateData();
        }


        [ServerRpc]
        public void SetDataServerRpc(int itemID, int itemQuantity)
        {
            ItemData itemData = GameDataManager.Instance.GetItemData(itemID);
            ItemSlot itemSlot = new ItemSlot(itemData, itemQuantity);
            SetData(itemSlot);
            SetDataClientRpc(itemID, itemQuantity);
        }

        [ClientRpc]
        private void SetDataClientRpc(int itemID, int itemQuantity)
        {
            ItemData itemData = GameDataManager.Instance.GetItemData(itemID);
            ItemSlot itemSlot = new ItemSlot(itemData, itemQuantity);
            SetData(itemSlot);
        }

        private void UpdateData()
        {
            if (spriteRenderer == null)
                LoadComponents();

            spriteRenderer.sprite = ItemData.icon;          
        }


        public virtual void Collect(Player player)
        {

        }

    
        public virtual void Drop(Player player, Vector2 position, Vector3 rotation, bool forceDestroyItemObject = false)
        {
            var itemSlotDrop = new ItemSlot(ItemSlot);
            var itemPrefab = GameDataManager.Instance.GetItemPrefab($"IP_ItemForDrop");
            if (itemPrefab == null) 
            {
                throw new System.Exception($"Not found prefab name IP_ItemForDrop in GameDataManager.cs");
            }
            var itemObject = Instantiate(itemPrefab, position, Quaternion.Euler(rotation), SaveManager.Instance.itemContainerParent);
            itemObject.GetComponent<ItemForDrop>().Set(itemSlotDrop);
            itemObject.transform.localScale = new Vector3(2, 2, 1);

            if (forceDestroyItemObject)
            {
                Destroy(this.gameObject);
            }
        }



        public virtual bool Use(Player player, Vector2 mousePosition)
        {
            return true;
        }

  
    }
}