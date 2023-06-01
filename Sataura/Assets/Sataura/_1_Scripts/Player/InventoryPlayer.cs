﻿using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Sataura
{
    public class InventoryPlayer : Player
    {
        [Header("CHARACTER DATA")]
        public PlayerInventory playerInventory;
        public PlayerEquipment playerEquipment;
        public PlayerSkills playerSkills;
        public ItemInHand itemInHand;
        public InputHandler playerInputHandler;


        #region Properties
        public PlayerInput PlayerInput { get; private set; }
        #endregion


        public override void OnNetworkSpawn()
        {
            PlayerInput = GetComponent<PlayerInput>();


            if (IsOwner)
            {
                UIPlayerInventory.Instance.SetPlayer(this.gameObject);
                UIItemInHand.Instance.SetPlayer(this.gameObject);
            }
           
        }
    }
}