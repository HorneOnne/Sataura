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
        public CharacterData characterData;
        [SerializeField] private PlayerInventory playerInventory;
        [SerializeField] private PlayerInGameInventory playerInGameInventory;
        [SerializeField] private ItemInHand itemInHand;
        [SerializeField] private PlayerMovement playerMovement;
        [SerializeField] private PlayerInputHandler playerInputHandler;
        [SerializeField] private PlayerEquipment playerEquipment;
        [SerializeField] private PlayerUseItem playerUseItem;



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
        [HideInInspector] public PlayerUseItem PlayerUseItem { get => playerUseItem; }
        [HideInInspector]
        public Transform HandHoldItem
        {
            get
            {
                if (handHoldItemInstance == null)
                    return null;
                else
                    return handHoldItemInstance.transform;
            }
        }
        #endregion


        public NetworkVariable<int> clientID = new NetworkVariable<int>();


        private void Awake()
        {
            //characterData = SaveManager.charactersData[SaveManager.selectionCharacterDataIndex];
        }

        public override void OnNetworkSpawn()
        {
            GameDataManager.Instance.AddNetworkPlayer(NetworkManager.LocalClientId, this);

            if (IsOwner)
            {
                // Camera
                if(GameManager.Instance.CinemachineVirtualCamera != null)
                    GameManager.Instance.CinemachineVirtualCamera.Follow = this.transform;

                UIPlayerInGameInventory.Instance.SetPlayer(this.gameObject);
                UIItemInHand.Instance.SetPlayer(this.gameObject);     
                
                /*if(UIPlayerInventory.Instance != null)
                {
                    UIPlayerInventory.Instance.SetPlayer(this);
                }*/
            }


            // Temp
            // ===========================================
            if (IsServer)
            {
                clientID.Value = NetworkManager.Singleton.ConnectedClientsIds.Count - 1;
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
                if(UIPlayerInGameInventory.Instance != null)
                {
                    UIPlayerInGameInventory.Instance.LoadReferences();
                }
                    

                if (UIItemInHand.Instance != null)
                    UIItemInHand.Instance.LoadReferences();


                /*if (UIPlayerInventory.Instance != null)
                {
                    UIPlayerInventory.Instance.LoadReferences();
                }  */             
            }

            if (IsOwner)
            {
                CameraBounds.Instance.localPlayer = this.transform;
            }

            StartCoroutine(TeleportPlayerToPosition(new Vector2(50, 30), 0.3f));




        }

        private IEnumerator TeleportPlayerToPosition(Vector2 position, float time)
        {
            playerMovement.Rb2D.isKinematic = true;
            yield return new WaitForSeconds(time);
            transform.position = position;
            playerMovement.Rb2D.isKinematic = false;
        }



    }
}