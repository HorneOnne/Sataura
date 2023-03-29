using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Sataura
{
    public class Player : NetworkBehaviour
    {
        //[Header("CHARACTER REFERENCES")]
        [SerializeField] private GameObject handHoldItemToSpawn;

        private GameObject handHoldItemInstance;
        private NetworkObject handHoldItemNetworkObject;


        [Header("CHARACTER DATA")]
        public PlayerData playerData;
        public CharacterData characterData;
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
        //[HideInInspector] public PlayerEquipment PlayerEquipment { get => playerEquipment; }
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

      
        public NetworkVariable<int> clientID = new NetworkVariable<int>();


        // Events



        public override void OnNetworkSpawn()
        {
            PlayerInput = GetComponent<PlayerInput>();
            GameDataManager.Instance.AddNetworkPlayer(NetworkManager.LocalClientId, this);

            if (IsOwner)
            {
                // Camera
                GameManager.Instance.CinemachineVirtualCamera.Follow = this.transform;
                
                UIPlayerInGameInventory.Instance.SetPlayer(this);
                UIItemInHand.Instance.SetPlayer(this);
                //UIPlayerEquipment.Instance.SetPlayer(this);
                UICreativeInventory.Instance.SetPlayer(this);
                //UIChestInventory.Instance.SetPlayer(this);                                                
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
                handHoldItemInstance.name = "Hand Hold Item";
                handHoldItemNetworkObject = handHoldItemInstance.GetComponent<NetworkObject>();            
                handHoldItemNetworkObject.Spawn();
                handHoldItemNetworkObject.TrySetParent(this.transform);
                handHoldItemNetworkObject.transform.localPosition = Vector3.zero;
            }
    

            if (IsOwner)
            {
                UIPlayerInGameInventory.Instance.LoadReferences();
                UIItemInHand.Instance.LoadReferences();
                //UIPlayerEquipment.Instance.LoadReferences();
                UICreativeInventory.Instance.LoadReferences();
                //UIChestInventory.Instance.LoadReferences();              
            }

            if(IsOwner)
            {
                CameraBounds.Instance.localPlayer = this.transform;               
            }

            StartCoroutine(TeleportPlayerInPosition(new Vector2(50, 30), 0.3f));


            
        }

        private IEnumerator TeleportPlayerInPosition(Vector2 position, float time)
        {
            playerMovement.Rb2D.isKinematic = true;
            yield return new WaitForSeconds(time);
            transform.position = position;
            playerMovement.Rb2D.isKinematic = false;
        }



    }  
}