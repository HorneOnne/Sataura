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
        [SerializeField] private IngamePlayer _player;


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

  


        public override void OnNetworkSpawn()
        {
            CreateEquimentObjects();
            StartCoroutine(LoadEquipmentData());
        }
        private void CreateEquimentObjects()
        {
            _helmetObject = Instantiate(_helmetPrefab);
            _chestplateObject = Instantiate(_chestplatePrefab);
            _leggingObject = Instantiate(_leggingPrefab);


            _bootsObject = Instantiate(_bootsPrefab);
            _bootsObject.SetOwner(_player);

            _hookObject = Instantiate(_hookPrefab);
            _hookObject.SetOwner(_player);
        }

        private IEnumerator LoadEquipmentData()
        {
            yield return new WaitUntil(() => _player.characterData != null);

            // Get equipemnts data
            _helmetData = _player.characterData.helmetData;
            _chestplateData = _player.characterData.chestplateData;
            _leggingData = _player.characterData.leggingData;
            _bootData = _player.characterData.bootsData;
            _hookData = _player.characterData.hookData;


            // Set equipments data
            _helmetObject.SetData(new ItemSlot(_helmetData, 1));
            _chestplateObject.SetData(new ItemSlot(_chestplateData, 1));
            _leggingObject.SetData(new ItemSlot(_leggingData, 1));
            _bootsObject.SetData(new ItemSlot(_bootData, 1));
            _hookObject.SetData(new ItemSlot(_hookData, 1));

            _helmetObject._networkObject.Spawn();
            _chestplateObject._networkObject.Spawn();
            _leggingObject._networkObject.Spawn();
            _bootsObject._networkObject.Spawn();
            _hookObject._networkObject.Spawn();

            _helmetObject._networkObject.TrySetParent(equipmentObjectsParent);
            _chestplateObject._networkObject.TrySetParent(equipmentObjectsParent);
            _leggingObject._networkObject.TrySetParent(equipmentObjectsParent);
            _bootsObject._networkObject.TrySetParent(equipmentObjectsParent);
            _hookObject._networkObject.TrySetParent(equipmentObjectsParent);

            _helmetObject.transform.localPosition = Vector3.zero;
            _chestplateObject.transform.localPosition = Vector3.zero;
            _leggingObject.transform.localPosition = Vector3.zero;
            _bootsObject.transform.localPosition = Vector3.zero;
            _hookObject.transform.localPosition = Vector3.zero;
        }    
    }
}
