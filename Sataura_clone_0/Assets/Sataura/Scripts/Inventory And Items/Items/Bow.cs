using System.Collections.Generic;
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
        [SerializeField] private bool consumeArrow;


        private ArrowData arrowItemData;
        private int? arrowSlotIndex;
        private ItemSlot arrowSlotInPlayerInventory;
        private GameObject arrowProjectilePrefab;
        private ArrowProjectile_001 arrowProjectileObject;


        protected override void Start()
        {
            base.Start();

            bowItemData = (BowData)this.ItemData;
            arrowProjectilePrefab = GameDataManager.Instance.GetProjectilePrefab("PP_ArrowProjectile_001");

        }



        public override bool Use(Player player)
        {
            inGameInventory = player.PlayerInGameInventory;
            arrowSlotIndex = inGameInventory.FindArrowSlotIndex();

            if (arrowSlotIndex == null) return false;
            if (arrowProjectilePrefab == null) return false;


            switch (bowItemData.useType)
            {
                case 1:
                    UseType01();
                    break;
                case 2:
                    UseType02();
                    break;
                case 3:
                    UseType03();    
                    break;
                default: break;

            }

            if (consumeArrow)
                ConsumeArrow();
            return true;
        }


        private void UseType01()
        {
            arrowProjectileObject = ArrowProjectileSpawner.Instance.Pool.Get().GetComponent<ArrowProjectile_001>();
            arrowProjectileObject.transform.position = shootingPoints[0].position;
            arrowProjectileObject.transform.rotation = transform.rotation;
            arrowSlotInPlayerInventory = inGameInventory.inGameInventory[(int)arrowSlotIndex];
            arrowItemData = (ArrowData)arrowSlotInPlayerInventory.ItemData;                               
            arrowProjectileObject.SetData(arrowItemData);           
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
                arrowProjectileObject.SetData(arrowItemData);
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



        /// <summary>
        /// Consumes an arrow from the player's inventory if the consumeArrow flag is set.
        /// </summary>
        private void ConsumeArrow()
        {
            arrowSlotInPlayerInventory.RemoveItem();
            UIPlayerInGameInventory.Instance.UpdateInventoryUIAt((int)arrowSlotIndex);
        }
    }
}