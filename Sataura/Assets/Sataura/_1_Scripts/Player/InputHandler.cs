using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System;

namespace Sataura
{
    public class InputHandler : NetworkBehaviour
    {
        [Header("REFERENCES")]
        [SerializeField] private InventoryPlayer _inventoryPlayer;

        private PlayerInventory playerInventory;
        private PlayerInGameSkills playerInGameInventory;
        private ItemInHand itemInHand;
        private PlayerMovement playerMovement;

        // New input system
        private PlayerInputAction playerInputAction;
        private InputAction jump;

        /// <summary>
        /// The number of seconds left in the jump buffer.
        /// </summary>
        private float jumpBufferCount;

        /// <summary>
        /// The time left for the player to hang in the air after leaving the ground.
        /// </summary>
        private float hangCounter;

        /// <summary>
        /// The time in seconds between clicks to register as a double click.
        /// </summary>
        float doubleClickTime = 0.2f;

        /// <summary>
        /// The time of the last click.
        /// </summary>
        float lastClickTime = 0;

        /// <summary>
        /// The time in seconds between presses of the right mouse button to register as a double press.
        /// </summary>
        float rightPressIntervalTime = 1.0f;

        /// <summary>
        /// The time left in the right mouse button double press interval.
        /// </summary>
        private float lastRightPressIntervalTimeCount = 0.0f;


        /// <summary>
        /// Tracks whether the left mouse button is currently being clicked.
        /// </summary>
        bool isLeftClicking = false;

        // Cached
        private Vector2 mousePosition;



        /// <summary>
        /// The key used to activate the utility ability.
        /// </summary>
        [Header("KEY BINDING")]
        [SerializeField] private KeyCode utilityKeyBinding = KeyCode.LeftShift;




        #region Properties
        public Vector2 MovementInput { get; private set; }
        public Vector2 RotateWeaponInput { get; private set; }
        public bool PressUtilityKeyInput { get; private set; }
        public float JumpInput { get; private set; }

        /// <summary>
        /// State of mouse pointer
        /// </summary>
        [field: SerializeField] public PointerState CurrentMouseState { get; private set; }
        #endregion Properties




        public override void OnNetworkSpawn()
        {
            playerInventory = _inventoryPlayer.playerInventory;
            itemInHand = _inventoryPlayer.itemInHand;
        }

    
    

    
        private void Update()
        {
            if (!IsOwner) return;

            PressUtilityKeyInput = Input.GetKey(utilityKeyBinding);
            HandleMouseEvents();


            if (CurrentMouseState == PointerState.DoubleLeftClick)
            {
                StackItem();
            }
            else if (CurrentMouseState == PointerState.SingleLeftClick)
            {
                //OpenCloseChest();
            }
            else if (CurrentMouseState == PointerState.LeftPress)
            {
                mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
        }



        //// <summary>
        /// Handles mouse events and updates the current mouse state.
        /// </summary>
        private void HandleMouseEvents()
        {
            CurrentMouseState = PointerState.Null;

            if (Input.GetMouseButtonDown(0))
            {
                CurrentMouseState = PointerState.SingleLeftClick;

                // check if last click was within doubleClickTime
                if (Time.time - lastClickTime < doubleClickTime)
                {
                    //Debug.Log("Double click detected");                
                    isLeftClicking = false;
                    CurrentMouseState = PointerState.DoubleLeftClick;
                }
                else
                {
                    // first click
                    lastClickTime = Time.time;
                    isLeftClicking = true;
                }
            }
            else if (Input.GetMouseButtonUp(0) && isLeftClicking)
            {
                //Debug.Log("Single click detected");           
                isLeftClicking = false;
                CurrentMouseState = PointerState.Null;
            }
            else if (Input.GetMouseButton(0))
            {
                CurrentMouseState = PointerState.LeftPress;
            }
            else if (Input.GetMouseButton(1))
            {
                CurrentMouseState = PointerState.RightPress;
                if (Time.time - lastRightPressIntervalTimeCount >= rightPressIntervalTime)
                {
                    CurrentMouseState = PointerState.RightPressAfterWait;
                }
            }
            else if (Input.GetMouseButtonUp(1))
            {
                CurrentMouseState = PointerState.Null;
                lastRightPressIntervalTimeCount = Time.time;
            }
        }



        /// <summary>
        /// Stack items if double left clicked
        /// </summary>
        private void StackItem()
        {
            if (itemInHand.HasHandHoldItemInServer() == false) return;

            switch (itemInHand.ItemGetFrom.slotStoredType)
            {
                case StoredType.PlayerInventory:
                    playerInventory.StackItem();
                    break;
                case StoredType.CraftingTable:
                    break;
                default: 
                    break;

            }
        }
    }
}
