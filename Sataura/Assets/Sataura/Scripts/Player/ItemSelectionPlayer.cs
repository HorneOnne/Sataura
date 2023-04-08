using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Sataura
{
    public class ItemSelectionPlayer : NetworkBehaviour
    {

        [Header("CHARACTER DATA")]
        public CharacterData characterData;
        [SerializeField] private PlayerInventory playerInventory;
        [SerializeField] private PlayerInGameInventory playerInGameInventory;
        [SerializeField] private ItemInHand itemInHand;
        [SerializeField] private PlayerInputHandler playerInputHandler;




        #region Properties
        [HideInInspector] public PlayerInventory PlayerInventory { get => playerInventory; }
        [HideInInspector] public PlayerInGameInventory PlayerInGameInventory { get => playerInGameInventory; }
        [HideInInspector] public ItemInHand ItemInHand { get => itemInHand; }
        [HideInInspector] public PlayerInputHandler PlayerInputHandler { get => playerInputHandler; }
        [HideInInspector] public PlayerInput PlayerInput { get; private set; }
        #endregion



        public override void OnNetworkSpawn()
        {
            PlayerInput = GetComponent<PlayerInput>();
            //GameDataManager.Instance.AddNetworkPlayer(NetworkManager.LocalClientId, this);

            Debug.Log(characterData == null);

            if (IsOwner)
            {
                UIPlayerInGameInventory.Instance.SetPlayer(this.gameObject);
                UIPlayerInventory.Instance.SetPlayer(this.gameObject);
                UIItemInHand.Instance.SetPlayer(this.gameObject);
            }


            // ===========================================
            /*if (IsOwner)
            {
                if (UIPlayerInGameInventory.Instance != null)
                {
                    UIPlayerInGameInventory.Instance.LoadReferences();
                }

                if (UIPlayerInventory.Instance != null)
                {
                    UIPlayerInventory.Instance.LoadReferences();
                }


                if (UIItemInHand.Instance != null)
                {
                    UIItemInHand.Instance.LoadReferences();
                }                   
            }*/

            
        }


    }
}