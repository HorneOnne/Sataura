using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Sataura
{
    /// <summary>
    /// Class that represents a player character.
    /// </summary>
    public class Player : NetworkBehaviour
    {
        //[Header("CHARACTER REFERENCES")]
        [SerializeField] private GameObject handHoldItemToSpawn;

        private GameObject handHoldItemInstance;
        private NetworkObject handHoldItemNetworkObject;


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
        [HideInInspector] public Transform HandHoldItem 
        {
            get 
            {
                if (handHoldItemInstance == null)
                    return null;
                else
                    return handHoldItemInstance.transform;
            }
        } 
        [HideInInspector] public PlayerInput PlayerInput { get; private set; }
        #endregion


        /*private void Awake()
        {
            if (!IsOwner) return;
            PlayerInput = GetComponent<PlayerInput>();
        }*/

        public NetworkVariable<int> clientID = new NetworkVariable<int>();

        public override void OnNetworkSpawn()
        {
            PlayerInput = GetComponent<PlayerInput>();
            
            if(IsServer || IsOwner)
            {

            }
           

            if (IsOwner)
            {
                // Camera
                GameManager.Instance.CinemachineVirtualCamera.Follow = this.transform;

                UIPlayerInGameInventory.Instance.SetPlayer(this);
                UIItemInHand.Instance.SetPlayer(this);
                UIPlayerEquipment.Instance.SetPlayer(this);
                UICreativeInventory.Instance.SetPlayer(this);
                UIChestInventory.Instance.SetPlayer(this);                                                
            }


            // Temp
            // ===========================================
            if (IsServer)
            {
                clientID.Value = NetworkManager.Singleton.ConnectedClientsIds.Count - 1;
            }
            if (IsServer || IsOwner)
            {
                if (playerInGameInventory.inGameInventoryData == null)
                    playerInGameInventory.inGameInventoryData = GameManager.Instance.inGameInventories[clientID.Value];
            }
            // ===========================================


          
            if (IsServer)
            {
                handHoldItemInstance = Instantiate(handHoldItemToSpawn);
                handHoldItemNetworkObject = handHoldItemInstance.GetComponent<NetworkObject>();            
                handHoldItemNetworkObject.Spawn();
                handHoldItemNetworkObject.TrySetParent(this.transform);
            }

            if (IsOwner)
            {
                UIPlayerInGameInventory.Instance.LoadReferences();
                UIItemInHand.Instance.LoadReferences();
                UIPlayerEquipment.Instance.LoadReferences();
                UICreativeInventory.Instance.LoadReferences();
                UIChestInventory.Instance.LoadReferences();
            }
        }

 

    }  
}