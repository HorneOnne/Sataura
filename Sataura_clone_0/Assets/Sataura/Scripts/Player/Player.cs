using UnityEngine;
using UnityEngine.InputSystem;

namespace Sataura
{
    /// <summary>
    /// Class that represents a player character.
    /// </summary>
    public class Player : MonoBehaviour
    {
        //[Header("CHARACTER REFERENCES")]
        [SerializeField] private Transform handHoldItem;


        [Header("CHARACTER DATA")]
        public PlayerData playerData;
        [SerializeField] private PlayerInventory playerInventory;
        [SerializeField] private PlayerInGameInventory playerInGameInventory;
        [SerializeField] private ItemInHand itemInHand;
        [SerializeField] private PlayerMovement playerMovement;
        [SerializeField] private PlayerInputHandler playerInputHandler;
        [SerializeField] private PlayerEquipment playerEquipment;


       
        /// <summary>
        /// The chest currently open by the player.
        /// </summary>
        [HideInInspector] public Chest currentOpenChest { get; set; }


        // RESTRICT FEATURES
        [Header("OPTIONAL RESTRICT FEATURES")]
        public bool handleItem;
        public bool handleMovement;
        public bool handleEquipment;

        public bool canUseItem;


        #region Properties
        [HideInInspector] public PlayerInventory PlayerInventory { get => playerInventory; }
        [HideInInspector] public PlayerInGameInventory PlayerInGameInventory { get => playerInGameInventory; }
        [HideInInspector] public ItemInHand ItemInHand { get => itemInHand; }
        [HideInInspector] public PlayerMovement PlayerMovement { get => playerMovement; }
        [HideInInspector] public PlayerInputHandler PlayerInputHandler { get => playerInputHandler; }
        [HideInInspector] public PlayerEquipment PlayerEquipment { get => playerEquipment; }
        [HideInInspector] public Transform HandHoldItem { get => handHoldItem; }
        [HideInInspector] public PlayerInput PlayerInput { get; private set; }
        #endregion


        private void Awake()
        {
            /*playerInventory = GetComponent<PlayerInventory>();
            itemInHand = GetComponent<ItemInHand>();
            playerMovement = GetComponent<PlayerMovement>();
            playerInputHandler = GetComponent<PlayerInputHandler>();
            playerEquipment = GetComponent<PlayerEquipment>();*/
            
            
            PlayerInput = GetComponent<PlayerInput>();
        }     
    }
}