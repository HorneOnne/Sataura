using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Sataura
{
    public class UIPlayerInventory : Singleton<UIPlayerInventory>
    {
        [Header("REFERENCES")]
        [SerializeField] private InventoryPlayer _inventoryPlayer;

        private PlayerInventory playerInventory;
        private InputHandler playerInputHandler;
        private ItemInHand itemInHand;


        [Header("DATA")]
        [HideInInspector] public List<GameObject> itemSlotList;


        [Header("UI PREFAB")]
        public GameObject itemSlotPrefab;


        [Header("UI Event Properties")]
        private GameObject currentSlotDrag;
        private GameObject startingSlotDrag;
        private GameObject currentSlotClicked;


        [Header("INVENTORY SETTINGS")]
        public DragType dragType;
        [SerializeField] private float pressIntervalTime = 1.0f;


        // CACHED
        private bool handHasItem;
        private bool slotHasItem;
        private float pressIntervalTimeCount = 0.0f;


        private void OnEnable()
        {
            EventManager.OnPlayerInventoryUpdated += UpdateInventoryUI;
        }

        private void OnDisable()
        {
            EventManager.OnPlayerInventoryUpdated -= UpdateInventoryUI;
        }

        private void Start()
        {
            
        }


        public void SetPlayer(GameObject playerObject)
        {
            this._inventoryPlayer = playerObject.GetComponent<InventoryPlayer>();

            StartCoroutine(LoadReferences());
        }

        private IEnumerator LoadReferences()
        {
            yield return new WaitUntil(() => _inventoryPlayer != null);

            dragType = DragType.Swap;
            itemInHand = _inventoryPlayer.itemInHand;
            playerInventory = _inventoryPlayer.playerInventory;
            playerInputHandler = _inventoryPlayer.playerInputHandler;

            CreateInventory();
            // Update Inventory UI at the first time when start game.
            Invoke("UpdateInventoryUI", .1f);
        }


        public void CreateInventory()
        {
            int inventorySize = _inventoryPlayer.characterData.playerInventoryData.itemSlots.Count;

            for (int i = 0; i < inventorySize; i++)
            {
                GameObject slotObject = Instantiate(itemSlotPrefab, this.transform);
                slotObject.GetComponent<UIItemSlot>().SetIndex(i);
                slotObject.GetComponent<UIItemSlot>().SetData(null);
                Utilities.AddEvent(slotObject, EventTriggerType.PointerClick, (baseEvent) => OnClick(baseEvent, slotObject));
                Utilities.AddEvent(slotObject, EventTriggerType.PointerEnter, delegate { OnEnter(slotObject); });
                Utilities.AddEvent(slotObject, EventTriggerType.PointerExit, delegate { OnExit(slotObject); });
                Utilities.AddEvent(slotObject, EventTriggerType.BeginDrag, (baseEvent) => OnBeginDrag(baseEvent, slotObject));
                Utilities.AddEvent(slotObject, EventTriggerType.EndDrag, (baseEvent) => OnEndDrag(baseEvent, slotObject));

                itemSlotList.Add(slotObject);
            }
        }

        public void CreateInventoryType(ItemCategory _category)
        {
            int inventorySize = _inventoryPlayer.characterData.playerInventoryData.itemSlots.Count;

            for (int i = 0; i < inventorySize; i++)
            {
                if(_inventoryPlayer.characterData.playerInventoryData.itemSlots[i].HasItemData())
                {
                    if (_inventoryPlayer.characterData.playerInventoryData.itemSlots[i].ItemData.itemCategory == _category)
                    {
                        GameObject slotObject = Instantiate(itemSlotPrefab, this.transform);
                        slotObject.GetComponent<UIItemSlot>().SetIndex(i);
                        slotObject.GetComponent<UIItemSlot>().SetData(null);
                        Utilities.AddEvent(slotObject, EventTriggerType.PointerClick, (baseEvent) => OnClick(baseEvent, slotObject));
                        Utilities.AddEvent(slotObject, EventTriggerType.PointerEnter, delegate { OnEnter(slotObject); });
                        Utilities.AddEvent(slotObject, EventTriggerType.PointerExit, delegate { OnExit(slotObject); });
                        Utilities.AddEvent(slotObject, EventTriggerType.BeginDrag, (baseEvent) => OnBeginDrag(baseEvent, slotObject));
                        Utilities.AddEvent(slotObject, EventTriggerType.EndDrag, (baseEvent) => OnEndDrag(baseEvent, slotObject));

                        itemSlotList.Add(slotObject);
                    }
                }                     
            }
        }

        public void Clear()
        {
            for (int i = 0; i < itemSlotList.Count; i++)
            {
                Destroy(itemSlotList[i].gameObject);
            }
            itemSlotList.Clear();
        }



        private void Update()
        {
            if (itemInHand == null)
                return;

            if (itemInHand.HasItemData())
            {
                if (playerInputHandler.CurrentMouseState == PointerState.RightPressAfterWait)
                {
                    if (currentSlotClicked != null)
                    {
                        if (Time.time - pressIntervalTimeCount >= pressIntervalTime)
                        {
                            OnRightPress(GetItemSlotIndex(currentSlotClicked));
                            pressIntervalTimeCount = Time.time;
                        }
                    }
                }
            }
        }



        public void UpdateInventoryUI()
        {
            for (int i = 0; i < itemSlotList.Count; i++)
            {
                UpdateInventoryUIAt(i);
            }
        }

        public void UpdateInventoryUIAt(int index)
        {
            UIItemSlot uiSlot = itemSlotList[index].GetComponent<UIItemSlot>();
            uiSlot.SetData(playerInventory.playerInventory[index]);
        }

        

        // LOGIC 
        // ===================================================================
        #region Interactive Events
        public void OnClick(BaseEventData baseEvent, GameObject clickedObject)
        {
            PointerEventData pointerEventData = (PointerEventData)baseEvent;

            int index = GetItemSlotIndex(clickedObject);
            if (index == -1) return;

            if (pointerEventData.button == PointerEventData.InputButton.Left)   // Mouse Left Event
            {
                OnLeftClick(index);
            }


            if (pointerEventData.button == PointerEventData.InputButton.Right)   // Mouse Right Event
            {
                OnRightClick(index);
            }

            UpdateInventoryUI();
        }

        public void OnEnter(GameObject clickedObject)
        {
            currentSlotDrag = clickedObject;
            currentSlotClicked = clickedObject;

            if (clickedObject != null)
            {
                currentSlotDrag = clickedObject;
            }
        }

        public void OnExit(GameObject clickedObject)
        {
            currentSlotDrag = null;
            currentSlotClicked = null;
        }

        public void OnBeginDrag(BaseEventData baseEvent, GameObject clickedObject)
        {
            PointerEventData pointerEventData = (PointerEventData)baseEvent;
            int index = GetItemSlotIndex(clickedObject);
            if (index == -1) return;

            if (clickedObject != null)
            {
                currentSlotDrag = clickedObject;
                startingSlotDrag = clickedObject;
            }


            if (pointerEventData.button == PointerEventData.InputButton.Left)   // Mouse Left Event
            {
                OnLeftClick(index);
            }


            if (pointerEventData.button == PointerEventData.InputButton.Right)   // Mouse Right Event
            {
                OnRightClick(index);
            }

            UpdateInventoryUI();
        }


        public void OnEndDrag(BaseEventData baseEvent, GameObject clickedObject)
        {
            PointerEventData pointerEventData = (PointerEventData)baseEvent;
            if (pointerEventData.button == PointerEventData.InputButton.Left)
            {
                int index = GetItemSlotIndex(currentSlotDrag);
                if (playerInventory.HasSlot(index) == false) return;

                if (playerInventory.HasItem(index))
                {
                    bool isSameItem = ItemData.IsSameItem(playerInventory.playerInventory[index].ItemData, itemInHand.GetItemData());
                    if (isSameItem)
                    {
                        ItemSlot remainItems = playerInventory.playerInventory[index].AddItemsFromAnotherSlot(itemInHand.GetSlot());
                        itemInHand.SetItem(remainItems, index, StoredType.PlayerInventory, true);
                    }
                    else
                    {
                        itemInHand.Swap(ref playerInventory.playerInventory, index, StoredType.PlayerInventory, true);

                        if (dragType == DragType.Swap)
                        {
                            int startingSlotIndex = GetItemSlotIndex(startingSlotDrag);
                            if (playerInventory.playerInventory[startingSlotIndex].HasItemData() == false)
                            {
                                startingSlotDrag = null;
                                itemInHand.Swap(ref playerInventory.playerInventory, startingSlotIndex, StoredType.PlayerInventory, true);
                            }
                        }
                    }
                }
                else
                {
                    itemInHand.Swap(ref playerInventory.playerInventory, index, StoredType.PlayerInventory, true);
                }

                UpdateInventoryUI();
            }
        }
        #endregion



        #region Inventory interactive methods

        private void OnLeftClick(int index)
        {
            handHasItem = itemInHand.HasItemData();
            slotHasItem = playerInventory.playerInventory[index].HasItemData();

            if (handHasItem == false)
            {
                if (slotHasItem == false)
                {
                    //Debug.Log("HAND: EMPTY \t SLOT: EMPTY");
                }
                else
                {
                    //Debug.Log("HAND: EMPTY \t SLOT: HAS ITEM");
                    itemInHand.Swap(ref playerInventory.playerInventory, index, StoredType.PlayerInventory, true);
                }
            }
            else
            {
                if (slotHasItem == false)
                {
                    itemInHand.Swap(ref playerInventory.playerInventory, index, StoredType.PlayerInventory, true);
                }
                else
                {
                    //Debug.Log("HAND: HAS ITEM \t SLOT: HAS ITEM");
                    bool isSameItem = ItemData.IsSameItem(playerInventory.playerInventory[index].ItemData, itemInHand.GetItemData());
                    if (isSameItem)
                    {
                        ItemSlot remainItems = playerInventory.playerInventory[index].AddItemsFromAnotherSlot(itemInHand.GetSlot());
                        itemInHand.SetItem(remainItems, index, StoredType.PlayerInventory, true);
                    }
                    else
                    {
                        itemInHand.Swap(ref playerInventory.playerInventory, index, StoredType.PlayerInventory, true);
                    }
                }
            }
        }

        private void OnRightClick(int index)
        {
            handHasItem = itemInHand.HasItemData();
            slotHasItem = playerInventory.playerInventory[index].HasItemData();

            if (handHasItem == false)
            {
                if (slotHasItem == false)
                {
                    //Debug.Log("HAND: EMPTY \t SLOT: EMPTY");
                }
                else
                {
                    //Debug.Log("HAND: EMPTY \t SLOT: HAS ITEM");
                    itemInHand.SplitItemSlotQuantityInInventory(ref playerInventory.playerInventory, index);
                }
            }
            else
            {
                if (slotHasItem == false)
                {
                    //Debug.Log("HAND: HAS ITEM \t SLOT: EMPTY");
                    playerInventory.AddNewItemAt(index, itemInHand.GetItemData());
                    itemInHand.RemoveItem();

                }
                else
                {
                    //Debug.Log("HAND: HAS ITEM \t SLOT: HAS ITEM");
                    if (ItemData.IsSameItem(itemInHand.GetItemData(), playerInventory.GetItem(index)))
                    {
                        bool isSlotNotFull = playerInventory.AddItemAt(index);

                        if (isSlotNotFull)
                        {
                            itemInHand.RemoveItem();
                        }
                    }
                }
            }
        }


        private void OnRightPress(int index)
        {
            handHasItem = itemInHand.HasItemData();
            slotHasItem = playerInventory.playerInventory[index].HasItemData();

            if (handHasItem == true)
            {
                if (slotHasItem == false)
                {
                    //Debug.Log("HAND: HAS ITEM \t SLOT: EMPTY");
                    playerInventory.AddNewItemAt(index, itemInHand.GetItemData());
                    itemInHand.RemoveItem();

                }
                else
                {
                    //Debug.Log("HAND: HAS ITEM \t SLOT: HAS ITEM");

                    if (ItemData.IsSameItem(itemInHand.GetItemData(), playerInventory.GetItem(index)))
                    {
                        bool isSlotNotFull = playerInventory.AddItemAt(index);

                        if (isSlotNotFull)
                        {
                            itemInHand.RemoveItem();
                        }

                    }
                }
            }
        }

        /// <summary>
        /// This method get itemSlot object then return itemSlot object's index in itemSlotList
        /// </summary>
        /// <param name="itemSlot"></param>
        /// <returns>Index itemSlot at itemSlotList</returns>
        private int GetItemSlotIndex(GameObject itemSlot)
        {
            if (itemSlot == null) return -1;
            return itemSlot.GetComponent<UIItemSlot>().SlotIndex;
        }

        #endregion Inventory interactive methods
    }
}


