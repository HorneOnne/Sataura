using UnityEngine;
using Unity.Netcode;

namespace Sataura
{
    public abstract class Item : NetworkBehaviour, IDroppable, ICollectible, IUseable
    {
        #region Properties
        public ItemData ItemData { get; private set; }
        public ItemSlot ItemSlot { get; private set; }
        protected GameObject Model { get; private set; }
        #endregion


        [Header("BaseItem References")]
        [HideInInspector] public SpriteRenderer spriteRenderer;
        public NetworkObject _networkObject;


        public override void OnNetworkSpawn()
        {
            //Debug.Log("OnNetworkSpawn");
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

            if(ItemData != null)
                spriteRenderer.sprite = ItemData.icon;       
            else
                spriteRenderer.sprite = null;
        }


        public virtual void Collect(IngamePlayer player)
        {

        }

    
        public virtual void Drop(IngamePlayer player, Vector2 position, Vector3 rotation, bool forceDestroyItemObject = false)
        {
            var itemSlotDrop = new ItemSlot(ItemSlot);
            var itemPrefab = GameDataManager.Instance.itemForDropPrefab;
            if (itemPrefab == null) 
            {
                throw new System.Exception($"Not found prefab name IP_ItemForDrop in GameDataManager.cs");
            }
            var itemObject = Instantiate(itemPrefab, position, Quaternion.Euler(rotation));
            itemObject.GetComponent<ItemForDrop>().Set(itemSlotDrop);
            itemObject.transform.localScale = new Vector3(2, 2, 1);

            if (forceDestroyItemObject)
            {
                Destroy(this.gameObject);
            }
        }



        public virtual bool Use(IngamePlayer player, Vector2 mousePosition)
        {
            return true;
        }     
    }
}