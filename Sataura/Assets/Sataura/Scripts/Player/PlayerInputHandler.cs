using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using System;

namespace Sataura
{
    /// <summary>
    /// Handles player input and sends events to other components.
    /// </summary>
    public class PlayerInputHandler : NetworkBehaviour
    {
        [Header("REFERENCES")]
        [SerializeField] private Player player;
        private PlayerInventory playerInventory;
        private PlayerInGameInventory playerInGameInventory;
        private PlayerEquipment playerEquipment;
        private ItemInHand itemInHand;
        private PlayerMovement playerMovement;

        /// <summary>
        /// The value of the horizontal movement input.
        /// </summary>
        private float movementInput;

        /// <summary>
        /// The number of seconds left in the jump buffer.
        /// </summary>
        private float jumpBufferCount;

        /// <summary>
        /// The time left for the player to hang in the air after leaving the ground.
        /// </summary>
        private float hangCounter;

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
        public KeyCode utilityKeyBinding = KeyCode.LeftShift;

        /// <summary>
        /// The key used to drop the current item.
        /// </summary>
        public KeyCode dropItemKey = KeyCode.T;

        PlayerInputAction playerInputAction;
        private InputAction jump;


        #region Events handler
        private void OnEnable()
        {
            if (player.canUseItem)
            {
                //EventManager.OnItemInHandChanged += ReInstantiateItem;
                EventManager.OnItemInHandChanged += FastEquipItem;
            }

        }

        private void OnDisable()
        {
            if (player.canUseItem)
            {
                //EventManager.OnItemInHandChanged -= ReInstantiateItem;
                EventManager.OnItemInHandChanged -= FastEquipItem;
            }
        }
        #endregion



        /*private void Start()
        {          
            playerInputAction = new PlayerInputAction();         
            playerInputAction.Player.Enable();
            jump = playerInputAction.Player.Jump;
            jump.Enable();
            jump.performed += SettingsJump;


            if (player.handleMovement)
            {
                playerMovement = player.PlayerMovement;
            }
                

            if(player.handleItem)
            {
                playerInventory = player.PlayerInventory;
                playerInGameInventory = player.PlayerInGameInventory;
                playerEquipment = player.PlayerEquipment;
                itemInHand = player.ItemInHand;
            }
        }*/

        public bool canJump;

        public override void OnNetworkSpawn()
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
                playerEquipment = player.PlayerEquipment;
                itemInHand = player.ItemInHand;
            }
        }

        private void Jump(InputAction.CallbackContext context)
        {
            jumpBufferCount = player.characterData.jumpBufferLength;
        }


        private void Update()
        {
            if (!IsOwner) return;

            if (player.handleMovement)
            {
                JumpInput = playerInputAction.Player.Jump.ReadValue<float>();
                MovementInput = playerInputAction.Player.Movement.ReadValue<Vector2>();
                RotateWeaponInput = playerInputAction.Player.RotateWeapon.ReadValue<Vector2>();


                if (playerMovement.isGrounded || playerMovement.isOnPlatform)
                    hangCounter = player.characterData.hangTime;
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
                    if (itemInHand.GetICurrenttem() != null)
                    {
                        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        itemInHand.GetICurrenttem().Drop(player, mousePosition, Vector3.zero, true);
                        itemInHand.ClearSlot();
                    }
                }


                if (player.handleItem)
                {
                    /*if (itemInHand.currentItemID.Value != -1)
                    {
                        RotateHoldItemServerRpc();
                    }*/


                    HandleMouseEvents();


                    if (CurrentMouseState == PointerState.DoubleLeftClick)
                    {
                        StackItem();
                    }
                    else if (CurrentMouseState == PointerState.SingleLeftClick)
                    {
                        OpenCloseChest();
                        //UseItem();
                    }
                    else if (CurrentMouseState == PointerState.LeftPress)
                    {
                        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        itemInHand.UseItemServerRpc(mousePosition);
                    }
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
            if (itemInHand.HasItem() == false) return;

            switch (itemInHand.ItemGetFrom.slotStoredType)
            {
                case StoredType.PlayerInventory:
                    playerInventory.StackItem();
                    break;
                case StoredType.ChestInventory:
                    player.currentOpenChest.Inventory.StackItem();
                    break;
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
        /// Handles fast equipment of items with a utility key press.
        /// </summary>
        private void FastEquipItem()
        {
            if (PressUtilityKeyInput)
            {
                if (itemInHand.HasItemData() == false) return;
                if (itemInHand.ItemGetFrom.slotStoredType == StoredType.PlayerInventory)
                {
                    ItemSlot equipItemSlot = itemInHand.GetSlot();
                    ItemSlot currentEquipmentSlot = null;
                    bool canEquip;

                    switch (itemInHand.GetItemData().itemType)
                    {
                        case ItemType.Helm:
                            if (playerEquipment.Helm.HasItem() == true)
                                currentEquipmentSlot = new ItemSlot(playerEquipment.Helm);
                            canEquip = playerEquipment.Equip(ItemType.Helm, equipItemSlot);
                            break;
                        case ItemType.ChestArmor:
                            if (playerEquipment.Chest.HasItem() == true)
                                currentEquipmentSlot = new ItemSlot(playerEquipment.Chest);
                            canEquip = playerEquipment.Equip(ItemType.ChestArmor, equipItemSlot);
                            break;
                        case ItemType.Shield:
                            if (playerEquipment.Shield.HasItem() == true)
                                currentEquipmentSlot = new ItemSlot(playerEquipment.Shield);
                            canEquip = playerEquipment.Equip(ItemType.Shield, equipItemSlot);
                            break;
                        default:
                            canEquip = false;
                            break;
                    }

                    if (canEquip)
                    {
                        if (currentEquipmentSlot != null)
                        {
                            playerInventory.AddNewItemAt(itemInHand.ItemGetFrom.slotIndex, currentEquipmentSlot.ItemData);
                        }
                        itemInHand.RemoveItem();
                        UIPlayerEquipment.Instance.UpdateEquipmentUI();
                        EventManager.TriggerPlayerEquipmentChangedEvent();

                    }
                }
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



        public float GetTimeLeftGround()
        {
            return Mathf.Abs(hangCounter - player.characterData.hangTime);
        }
    }
}
