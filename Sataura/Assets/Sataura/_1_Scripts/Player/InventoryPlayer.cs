using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Sataura
{
    public class InventoryPlayer : MonoBehaviour
    {
        [Header("CHARACTER DATA")]
        public PlayerInventory playerInventory;
        public PlayerEquipment playerEquipment;
        public PlayerSkills playerSkills;
        public ItemInHand itemInHand;
        public InputHandler playerInputHandler;


        [Header("Runtime References")]
        public CharacterData characterData;


        #region Properties
        public PlayerInput PlayerInput { get; private set; }
        #endregion

        private void Awake()
        {
            characterData = SaveManager.Instance.charactersData[SaveManager.Instance.selectionCharacterDataIndex];
        }

        public void Start()
        {
            PlayerInput = GetComponent<PlayerInput>();

            UIPlayerInventory.Instance.SetPlayer(this.gameObject);
            UIItemInHand.Instance.SetPlayer(this.gameObject);

        }
    }
}