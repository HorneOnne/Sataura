using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine;

namespace Sataura
{
    public class UIEquipmentManager : MonoBehaviour
    {
        public static UIEquipmentManager Instance{get; private set;}


        [Header("References")]
        [SerializeField] private InventoryPlayer inventoryPlayer;


        private ItemInHand itemInHand;
        [Header("Preview Camera References")]
        public Transform _previewCameraParent;

        [Header("Equipment References")]
        [SerializeField] private UIEquipSlot _uiHook;
        [SerializeField] private UIEquipSlot _uiBoots;
        [SerializeField] private UIEquipSlot _helmet;
        [SerializeField] private UIEquipSlot _chestplate;
        [SerializeField] private UIEquipSlot _legging;
        [SerializeField] private UIEquipSlot _accessory;
 


        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            itemInHand = inventoryPlayer.itemInHand;

            AddUIItemSLotEvent(_uiHook);
            AddUIItemSLotEvent(_uiBoots);
            AddUIItemSLotEvent(_helmet);
            AddUIItemSLotEvent(_chestplate);
            AddUIItemSLotEvent(_legging);
            AddUIItemSLotEvent(_accessory);
        }


 

        private void AddUIItemSLotEvent(UIEquipSlot slot)
        {
            GameObject slotObject = slot.gameObject;

            Utilities.AddEvent(slotObject, EventTriggerType.PointerDown, (baseEvent) => OnClick(baseEvent, slotObject));
        }

        public void OnClick(BaseEventData baseEvent, GameObject clickedObject)
        {
            if (clickedObject.GetComponent<UIEquipSlot>().IsLocking) return;
            PointerEventData pointerEventData = (PointerEventData)baseEvent;

            var equipmentSlotType = clickedObject.GetComponent<UIEquipSlot>().EequipmentType;
            var equipmentData = inventoryPlayer.playerEquipment.GetEquipmentData(equipmentSlotType);

            if (itemInHand.HasItemData() == false)
            {
                if (equipmentData == null)
                {
                    //Debug.Log("HAND: EMPTY \t SLOT: EMPTY");

                }
                else
                {
                    //Debug.Log("HAND: EMPTY \t SLOT: HAS ITEM");
                    itemInHand.SetItem(new ItemSlot(equipmentData,1), slotIndex: -1, storageType: StoredType.Another, true);
                    inventoryPlayer.playerEquipment.ClearData(equipmentSlotType);
                }
            }
            else
            {
                if (equipmentData == null)
                {
                    //Debug.Log("HAND: HAS ITEM \t SLOT: EMPTY");

                    bool canEquip = inventoryPlayer.playerEquipment.TryEquip(itemInHand.GetItemData(), equipmentSlotType);
                    if (canEquip)
                    {
                        itemInHand.ClearSlot();
                    }
                }
                else
                {
                    //Debug.Log("HAND: HAS ITEM \t SLOT: HAS ITEM");
                }

            }

            UpdateUI();
            EventManager.TriggerPlayerEquipmentChangedEvent();
        }

       
        public void UpdateUI()
        {
            _uiHook.UpdateItemImage(inventoryPlayer.playerEquipment._hookData);
            _uiBoots.UpdateItemImage(inventoryPlayer.playerEquipment._bootsData);
            _helmet.UpdateItemImage(inventoryPlayer.playerEquipment._helmetData);
            _chestplate.UpdateItemImage(inventoryPlayer.playerEquipment._chestplateData);
            _legging.UpdateItemImage(inventoryPlayer.playerEquipment._leggingData);
            _accessory.UpdateItemImage(inventoryPlayer.playerEquipment._accessoryData);
        }
    }
}




