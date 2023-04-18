using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System;

namespace Sataura
{
    /// <summary>
    /// Handles player input and sends events to other components.
    /// </summary>
    public class PlayerInputHandler : NetworkBehaviour
    {
        // Event
        public static event Action<int, int> OnCurrentUseItemIndexChanged;
        public PlayerType playerType;

        [Header("REFERENCES")]
        [SerializeField] private Player player;
        [SerializeField] private ItemSelectionPlayer itemSelectionPlayer;

        private PlayerInventory playerInventory;
        private PlayerInGameInventory playerInGameInventory;
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

        /// <summary>
        /// The key used to drop the current item.
        /// </summary>
        [SerializeField] private KeyCode dropItemKey = KeyCode.T;

        [SerializeField]
        private List<KeyCode> selectItemBindingKeys = new List<KeyCode>
        {
            KeyCode.Alpha1,
            KeyCode.Alpha2,
            KeyCode.Alpha3,
            KeyCode.Alpha4,
            KeyCode.Alpha5,
            KeyCode.Alpha6,
            KeyCode.Alpha7,
            KeyCode.Alpha8,
            KeyCode.Alpha9,
        };
        //[SerializeField] private int currentUseItemIndex = -1;
        public NetworkVariable<int> currentUseItemIndex = new NetworkVariable<int>(-1);



        #region Properties
        public Vector2 MovementInput { get; private set; }
        public Vector2 RotateWeaponInput { get; private set; }
        public bool PressUtilityKeyInput { get; private set; }
        public float JumpInput { get; private set; }
        public int CurrentUseItemIndex { get => currentUseItemIndex.Value; }

        /// <summary>
        /// State of mouse pointer
        /// </summary>
        [field: SerializeField] public PointerState CurrentMouseState { get; private set; }
        #endregion Properties






        #region Events handler
        private void OnEnable()
        {
            if (playerType == PlayerType.IngamePlayer)
            {
                if (player.canUseItem)
                {
                    EventManager.OnItemInHandChanged += ResetOnCurrentUseItemIndex;
                }
            }


        }

        private void OnDisable()
        {
            if (playerType == PlayerType.IngamePlayer)
            {
                if (player.canUseItem)
                {
                    EventManager.OnItemInHandChanged -= ResetOnCurrentUseItemIndex;
                }
            }

        }
        #endregion


        public bool canJump;


        public override void OnNetworkSpawn()
        {
            if (playerType == PlayerType.IngamePlayer)
            {
                playerInputAction = new PlayerInputAction();
                playerInputAction.Player.Enable();
                jump = playerInputAction.Player.Jump;
                jump.Enable();
                jump.performed += Jump;


                if (player.handleMovement)
                {
                    playerMovement = player.PlayerMovement;
                }

                if (player.handleItem)
                {
                    playerInventory = player.PlayerInventory;
                    playerInGameInventory = player.PlayerInGameInventory;
                    itemInHand = player.ItemInHand;
                }
            }

            if (playerType == PlayerType.ItemSelectionPlayer)
            {
                playerInventory = itemSelectionPlayer.PlayerInventory;
                playerInGameInventory = itemSelectionPlayer.PlayerInGameInventory;
                itemInHand = itemSelectionPlayer.ItemInHand;
            }
        }

        private void Jump(InputAction.CallbackContext context)
        {
            jumpBufferCount = player.characterData.characterMovementData.jumpBufferLength;
        }


        private void ResetOnCurrentUseItemIndex()
        {
            OnCurrentUseItemIndexChanged.Invoke(currentUseItemIndex.Value, -1);

            if (IsServer)
            {
                currentUseItemIndex.Value = -1;
            }

        }

        [ServerRpc]
        private void AAServerRpc(int oldIndex, int newIndex)
        {
            currentUseItemIndex.Value = newIndex;
        }



        private void Update()
        {
            if (!IsOwner) return;
            if (player.IsGameOver()) return;


            if (playerType == PlayerType.IngamePlayer)
            {
                for (int i = 0; i < selectItemBindingKeys.Count; i++)
                {
                    if (Input.GetKeyDown(selectItemBindingKeys[i]))
                    {
                        // Do nothing if player holding item in hand.
                        if (itemInHand.HasHandHoldItemInServer())
                        {
                            return;
                        }

                        int oldIndex = currentUseItemIndex.Value;
                        int newIndex = i;
                        OnCurrentUseItemIndexChanged.Invoke(oldIndex, newIndex);
                        AAServerRpc(oldIndex, newIndex);
                    }
                }


                if (player.handleMovement)
                {
                    JumpInput = playerInputAction.Player.Jump.ReadValue<float>();
                    MovementInput = playerInputAction.Player.Movement.ReadValue<Vector2>();
                    RotateWeaponInput = playerInputAction.Player.RotateWeapon.ReadValue<Vector2>();

                    if (playerMovement.isGrounded || playerMovement.isOnPlatform)
                        hangCounter = player.characterData.characterMovementData.hangTime;
                    else
                        hangCounter -= Time.deltaTime;



                    // calculate Jump Buffer
                    if (jumpBufferCount >= 0)
                    {
                        jumpBufferCount -= Time.deltaTime;
                    }


                    if (jumpBufferCount > 0 && hangCounter > 0)
                    {
                        canJump = true;
                        jumpBufferCount = 0;
                    }
                }


                if (player.handleItem)
                {
                    PressUtilityKeyInput = Input.GetKey(utilityKeyBinding);

                    if (Input.GetKeyDown(dropItemKey))
                    {
                        if (itemInHand.GetItemObject() != null)
                        {
                            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                            itemInHand.GetItemObject().Drop(player, mousePosition, Vector3.zero, true);
                            itemInHand.ClearSlot();
                        }
                    }


                    if (player.handleItem)
                    {
                        HandleMouseEvents();


                        if (CurrentMouseState == PointerState.DoubleLeftClick)
                        {
                            StackItem();
                        }
                        else if (CurrentMouseState == PointerState.SingleLeftClick)
                        {
                            OpenCloseChest();
                        }
                        else if (CurrentMouseState == PointerState.LeftPress)
                        {
                            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                            itemInHand.UseItemServerRpc(mousePosition);
                        }
                    }
                }
            }


            if (playerType == PlayerType.ItemSelectionPlayer)
            {
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
                    itemInHand.UseItemServerRpc(mousePosition);
                }
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
            //if (itemInHand.HasHandHoldItemInServer() == false) return;

            switch (itemInHand.ItemGetFrom.slotStoredType)
            {
                case StoredType.PlayerInventory:
                    playerInventory.StackItem();
                    break;
                /*case StoredType.ChestInventory:
                    player.currentOpenChest.Inventory.StackItem();
                    break;*/
                case StoredType.CraftingTable:
                    CraftingTable.Instance.StackItem();
                    break;
                case StoredType.PlayerInGameInventory:
                    playerInGameInventory.StackItem();
                    break;

                default: break;

            }
        }





        /// <summary>
        /// Open or close chest if single left clicked
        /// </summary>
        private void OpenCloseChest()
        {
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (hit.collider != null)
            {
                var chest = hit.collider.GetComponent<Chest>();
                if (chest == null) return;

                if (player.currentOpenChest != null && chest.ChestState != Chest.ChestStateEnum.Placed)
                {
                    if (player.currentOpenChest != chest)
                    {
                        player.currentOpenChest.Close(player);
                        chest.Open(player);
                        return;
                    }
                }
                chest.Toggle(player);
            }
        }
    }
}
