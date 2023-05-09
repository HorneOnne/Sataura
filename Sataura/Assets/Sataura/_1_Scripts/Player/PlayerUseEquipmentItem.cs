using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System.Collections;

namespace Sataura
{
    public class PlayerUseEquipmentItem : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private Player _player;


        [Header("Equipment Data  (Runtime)")]
        [SerializeField] private HelmetData _helmetData;
        [SerializeField] private ChestplateData _chestplateData;
        [SerializeField] private LeggingData _leggingData;
        [SerializeField] private BootData _bootData;
        [SerializeField] private HookData _hookData;


        [Header("Equipment Prefabs")]
        [SerializeField] private Helmet _helmetPrefab;
        [SerializeField] private Chestplate _chestplatePrefab;
        [SerializeField] private Legging _leggingPrefab;
        [SerializeField] private Boots _bootsPrefab;
        [SerializeField] private Hook _hookPrefab;

        [Header("UseEquipment Properties")]
        [SerializeField] private Transform equipmentObjectsParent;

        // Cached
        private Helmet _helmetObject; 
        private Chestplate _chestplateObject; 
        private Legging _leggingObject; 
        private Boots _bootsObject; 
        private Hook _hookObject;
        private PlayerInputAction playerInputAction;

  


        public override void OnNetworkSpawn()
        {
            CreateEquimentObjects();
            StartCoroutine(LoadEquipmentData());


            // Double jump boost object handle
            playerInputAction = new PlayerInputAction();
            playerInputAction.Player.Enable();
            playerInputAction.Player.Jump.performed += DoubleJumpHandle;
            // -------------------------------



        }
        private void CreateEquimentObjects()
        {
            _helmetObject = Instantiate(_helmetPrefab, equipmentObjectsParent);
            _chestplateObject = Instantiate(_chestplatePrefab, equipmentObjectsParent);
            _leggingObject = Instantiate(_leggingPrefab, equipmentObjectsParent);
            _bootsObject = Instantiate(_bootsPrefab, equipmentObjectsParent);
            _hookObject = Instantiate(_hookPrefab, equipmentObjectsParent);
        }

        private IEnumerator LoadEquipmentData()
        {
            yield return new WaitUntil(() => _player.characterData != null);

            // Get equipemnts data
            _helmetData = (HelmetData)GameDataManager.Instance.GetItemData(_player.characterData.helmetDataID);
            _chestplateData = (ChestplateData)GameDataManager.Instance.GetItemData(_player.characterData.chestplateDataID);
            _leggingData = (LeggingData)GameDataManager.Instance.GetItemData(_player.characterData.leggingDataID);
            _bootData = (BootData)GameDataManager.Instance.GetItemData(_player.characterData.bootsDataID);
            _hookData = (HookData)GameDataManager.Instance.GetItemData(_player.characterData.hookDataID);


            // Set equipments data
            _helmetObject.SetData(new ItemSlot(_helmetData, 1));
            _chestplateObject.SetData(new ItemSlot(_chestplateData, 1));
            _leggingObject.SetData(new ItemSlot(_leggingData, 1));
            _bootsObject.SetData(new ItemSlot(_bootData, 1));
            _hookObject.SetData(new ItemSlot(_hookData, 1));
        }

        

        private void DoubleJumpHandle(InputAction.CallbackContext obj)
        {
            //_bootsObject.DoubleJump(_player);
            Debug.Log("Handle double jump !!!");
        }
    }
}
