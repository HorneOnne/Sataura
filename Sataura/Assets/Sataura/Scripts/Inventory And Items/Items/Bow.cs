using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


namespace Sataura
{
    public class Bow : Item
    {
        [Header("References")]
        private PlayerInGameInventory inGameInventory;
        public List<Transform> shootingPoints;


        [Header("Bow Properties")]
        private BowData bowItemData;
        //[SerializeField] private bool consumeArrow;


        private ArrowData arrowItemData;
        private int? arrowSlotIndex;
        private ItemSlot arrowSlotInPlayerInventory;
        private GameObject arrowProjectilePrefab;
        private ArrowProjectile_001 arrowProjectileObject;


        // Cached
        private ulong[] cachedClientIds;
        NetworkObject playerNetworkObject;
        Player playerObject;

        public override void OnNetworkSpawn()
        {
            bowItemData = (BowData)this.ItemData;
            arrowProjectilePrefab = GameDataManager.Instance.GetProjectilePrefab("PP_ArrowProjectile_001");


            if (IsServer)
            {
                int itemID = GameDataManager.Instance.GetItemID(bowItemData);
                SetDataServerRpc(itemID, 1);
            }
            
        }


        public override bool Use(Player player, Vector2 mousePosition)
        {                    
            inGameInventory = player.PlayerInGameInventory;
            arrowSlotIndex = inGameInventory.FindArrowSlotIndex();

            if (arrowSlotIndex == null) return false;
            if (arrowProjectilePrefab == null) return false;

            if (cachedClientIds == null || cachedClientIds.Length == 0)
            {
                InitializeCachedClientIds(player.GetComponent<NetworkObject>().OwnerClientId);            
            }



            switch (bowItemData.useType)
            {
                case 1:
                    UseType01(mousePosition);
                    break;
                case 2:
                    UseType02();
                    break;
                case 3:
                    UseType03();    
                    break;
                default: break;

            }

            /*if (consumeArrow)
            {
                ConsumeArrowServerRpc((int)arrowSlotIndex);
            }*/

            return true;
        }


        private void UseType01(Vector2 mousePosition)
        {
            arrowProjectilePrefab = GameDataManager.Instance.GetProjectilePrefab("PP_ArrowProjectile_001");
            var netObject = NetworkObjectPool.Singleton.GetNetworkObject(arrowProjectilePrefab, shootingPoints[0].position, transform.rotation);

            if (netObject.IsSpawned == false)
                netObject.Spawn();

            arrowProjectileObject = netObject.GetComponent<ArrowProjectile_001>();
            Utilities.RotateObjectTowardMouse2D(mousePosition, arrowProjectileObject.transform, -45);
            arrowSlotInPlayerInventory = inGameInventory.inGameInventory[(int)arrowSlotIndex];
            arrowItemData = (ArrowData)arrowSlotInPlayerInventory.ItemData;
           
            int arrowID = GameDataManager.Instance.GetItemID(arrowItemData);
            arrowProjectileObject.SetDataServerRpc(arrowID, true);
            arrowProjectileObject.Shoot(bowItemData, arrowItemData); 
        }


        private void UseType02()
        {
            for (int i = 0; i < 2; i++)
            {
                arrowProjectileObject = ArrowProjectileSpawner.Instance.Pool.Get().GetComponent<ArrowProjectile_001>();
                arrowProjectileObject.transform.position = shootingPoints[i].position;
                arrowProjectileObject.transform.rotation = transform.rotation;
                arrowSlotInPlayerInventory = inGameInventory.inGameInventory[(int)arrowSlotIndex];
                arrowItemData = (ArrowData)arrowSlotInPlayerInventory.ItemData;

                int arrowID = GameDataManager.Instance.GetItemID(arrowItemData);
                arrowProjectileObject.SetDataServerRpc(arrowID, true);
                arrowProjectileObject.Shoot(bowItemData, arrowItemData);
            }
        }

        private void UseType03()
        {
            for (int i = 0; i < 3; i++)
            {
                arrowProjectileObject = ArrowProjectileSpawner.Instance.Pool.Get().GetComponent<ArrowProjectile_001>();
                arrowProjectileObject.transform.position = shootingPoints[i].position;
                arrowProjectileObject.transform.rotation = transform.rotation;
                arrowSlotInPlayerInventory = inGameInventory.inGameInventory[(int)arrowSlotIndex];
                arrowItemData = (ArrowData)arrowSlotInPlayerInventory.ItemData;
                arrowProjectileObject.SetData(arrowItemData);
                arrowProjectileObject.Shoot(bowItemData, arrowItemData);
            }
        }


        
        private void InitializeCachedClientIds(ulong clientId)
        {
            Debug.Log("InitializeCachedClientIds");
            // Assuming that you have a list of client IDs that doesn't change and you want to cache them
            List<ulong> clientIds = new List<ulong>
            {
                clientId
            };

            cachedClientIds = clientIds.ToArray();
        }


        /*[ServerRpc]
        private void ConsumeArrowServerRpc(int arrowIndex)
        {
            inGameInventory.inGameInventory[arrowIndex].RemoveItem();
            UIPlayerInGameInventory.Instance.UpdateInventoryUIAt(arrowIndex);

            ClientRpcParams clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = cachedClientIds
                }
            };

            ConsumeArrowClientRpc(arrowIndex, clientRpcParams);
        }

        [ClientRpc]
        private void ConsumeArrowClientRpc(int arrowIndex, ClientRpcParams clientRpcParams = default)
        {
            if (IsServer) 
                return;

            playerNetworkObject = NetworkManager.LocalClient.PlayerObject;
            playerObject = playerNetworkObject.GetComponent<Player>();

            if(playerObject != null)
            {
                playerObject.PlayerInGameInventory.inGameInventory[arrowIndex].RemoveItem();
                UIPlayerInGameInventory.Instance.UpdateInventoryUIAt(arrowIndex);
            }            
        }*/
    }
}