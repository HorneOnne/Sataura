using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Sataura
{
    public class ItemSelectionPlayer : NetworkBehaviour
    {

        [Header("CHARACTER DATA")]
        public CharacterData characterData;
        public PlayerInventory playerInventory;
        public PlayerEquipment playerEquipment;
        public PlayerSkills playerSkills;
        public ItemInHand itemInHand;
        public PlayerInputHandler playerInputHandler;


        #region Properties
        public PlayerInput PlayerInput { get; private set; }
        #endregion


        public override void OnNetworkSpawn()
        {
            PlayerInput = GetComponent<PlayerInput>();


            if (IsOwner)
            {
                UIPlayerInventory.Instance.SetPlayer(this.gameObject);
                UIItemInHand.Instance.SetPlayer(this.gameObject);
            }
           
        }
    }
}