using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;

namespace Sataura
{
    /// <summary>
    /// Handles player input and sends events to other components.
    /// </summary>
    public class PlayerInputHandler : NetworkBehaviour
    {
        // Event
        public static event Action<int,int> OnCurrentUseItemIndexChanged;


        [Header("REFERENCES")]
        [SerializeField] private Player player;
        private PlayerInventory playerInventory;
        private PlayerInGameInventory playerInGameInventory;
        private PlayerEquipment playerEquipment;
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
            if (player.canUseItem)
            {
                //EventManager.OnItemInHandChanged += ReInstantiateItem;
                //EventManager.OnItemInHandChanged += FastEquipItem;

                EventManager.OnItemInHandChanged += ResetOnCurrentUseItemIndex;
            }

        }

        private void OnDisable()
        {
            if (player.canUseItem)
            {
                //EventManager.OnItemInHandChanged -= ReInstantiateItem;
                //EventManager.OnItemInHandChanged -= FastEquipItem;

                EventManager.OnItemInHandChanged -= ResetOnCurrentUseItemIndex;
            }
        }
        #endregion





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


        private void ResetOnCurrentUseItemIndex()
        {
            OnCurrentUseItemIndexChanged.Invoke(currentUseItemIndex.Value, -1);

            if(IsServer)
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

            for(int i = 0; i < selectItemBindingKeys.Count; i++)
            {
                if (Input.GetKeyDown(selectItemBindingKeys[i]))
                {
                    // Do nothing if player holding item in hand.
                    if(itemInHand.HasHandHoldItemInServer())
                    {
                        return;
                    }


                    /*int oldIndex = currentUseItemIndex.Value;
                    int newIndex = i;

                    OnCurrentUseItemIndexChanged.Invoke(oldIndex, newIndex);
                    currentUseItemIndex.Value = newIndex;*/


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
            if (itemInHand.HasItemObject() == false) return;
            //if (itemInHand.HasHandHoldItemInServer() == false) return;

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
                            if (playerEquipment.Helm.HasItemData() == true)
                                currentEquipmentSlot = new ItemSlot(playerEquipment.Helm);
                            canEquip = playerEquipment.Equip(ItemType.Helm, equipItemSlot);
                            break;
                        case ItemType.ChestArmor:
                            if (playerEquipment.Chest.HasItemData() == true)
                                currentEquipmentSlot = new ItemSlot(playerEquipment.Chest);
                            canEquip = playerEquipment.Equip(ItemType.ChestArmor, equipItemSlot);
                            break;
                        case ItemType.Shield:
                            if (playerEquipment.Shield.HasItemData() == true)
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
