using System.Collections;
using Unity.Netcode;
using UnityEngine;


namespace Sataura
{
    public class Player : NetworkBehaviour
    {
        [SerializeField] private GameObject handHoldItemToSpawn;

        private GameObject handHoldItemInstance;
        private NetworkObject handHoldItemNetworkObject;


        [Header("CHARACTER DATA")]
        public CharacterData characterData;
        public PlayerInGameSkills playerIngameSkills;
        public PlayerMovement playerMovement;
        public PlayerCombat playerCombat;
        public IngameInputHandler ingameInputHandler;
        public PlayerUseItem playerUseItem;
        public PlayerUseEquipmentItem playerUseEquipmentItem;
        private IngameInformationManager ingameInformationManager;

        [Header("Effects")]
        public GhostEffect ghostEffect;


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


        public NetworkVariable<int> clientID = new NetworkVariable<int>();


        private void Awake()
        {
            characterData = SaveManager.charactersData[SaveManager.selectionCharacterDataIndex];
        }

        public override void OnNetworkSpawn()
        {
            GameDataManager.Instance.AddNetworkPlayer(NetworkManager.LocalClientId, this);
            GameDataManager.Instance.singleModePlayer = this.gameObject;

            if (IsOwner)
            {
                // Camera
                if(GameManager.Instance.CinemachineVirtualCamera != null)
                    GameManager.Instance.CinemachineVirtualCamera.Follow = this.transform;

                ingameInformationManager = IngameInformationManager.Instance;

                UIPlayerInGameSkills.Instance.SetPlayer(this.gameObject);
                UIItemInHand.Instance.SetPlayer(this.gameObject);


                // Update inventory data.
                playerIngameSkills.UpdateCharacterData();
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
                
                if (UIPlayerInGameSkills.Instance != null)
                {
                    UIPlayerInGameSkills.Instance.LoadReferences();
                }                 
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

        public bool IsGameOver()
        {
            return ingameInformationManager.IsGameOver();
        }
    }
}