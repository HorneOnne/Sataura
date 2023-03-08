using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

namespace Sataura
{
    public class UIPlayerInGameInventory : Singleton<UIPlayerInGameInventory>
    {
        [Header("REFERENCES")]
        public Player player;
        private PlayerInGameInventory playerInGameInventory;
        private PlayerInputHandler playerInputHandler;
        private ItemInHand itemInHand;


        [Header("DATA")]
        [HideInInspector] public List<GameObject> itemSlotList;


        [Header("UI PREFAB")]
        public GameObject itemSlotPrefab;
        [SerializeField] private float scaleUI = 1.0f;


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
            itemInHand = player.ItemInHand;
            playerInGameInventory = player.PlayerInGameInventory;
            playerInputHandler = player.PlayerInputHandler;
            dragType = DragType.Swap;


            for (int i = 0; i < playerInGameInventory.Capacity; i++)
            {
                GameObject slotObject = Instantiate(itemSlotPrefab, this.transform);
                slotObject.transform.localScale = Vector3.one * scaleUI;
                slotObject.GetComponent<UIItemSlot>().SetIndex(i);
                slotObject.GetComponent<UIItemSlot>().SetData(null);
                Utilities.AddEvent(slotObject, EventTriggerType.PointerClick, (baseEvent) => OnClick(baseEvent, slotObject));
                Utilities.AddEvent(slotObject, EventTriggerType.PointerEnter, delegate { OnEnter(slotObject); });
                Utilities.AddEvent(slotObject, EventTriggerType.PointerExit, delegate { OnExit(slotObject); });
                Utilities.AddEvent(slotObject, EventTriggerType.BeginDrag, (baseEvent) => OnBeginDrag(baseEvent, slotObject));
                Utilities.AddEvent(slotObject, EventTriggerType.EndDrag, (baseEvent) => OnEndDrag(baseEvent, slotObject));

                itemSlotList.Add(slotObject);
            }

            // Update Inventory UI at the first time when start game.
            Invoke("UpdateInventoryUI", .1f);
        }



        private void Update()
        {
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
            for (int i = 0; i < playerInGameInventory.Capacity; i++)
            {
                UpdateInventoryUIAt(i);
            }
        }

        public void UpdateInventoryUIAt(int index)
        {
            UIItemSlot uiSlot = itemSlotList[index].GetComponent<UIItemSlot>();
            uiSlot.SetData(playerInGameInventory.inGameInventory[index]);
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
                if (playerInGameInventory.HasSlot(index) == false) return;

                if (playerInGameInventory.HasItem(index))
                {
                    bool isSameItem = ItemData.IsSameItem(playerInGameInventory.inGameInventory[index].ItemData, itemInHand.GetItemData());
                    if (isSameItem)
                    {
                        ItemSlot remainItems = playerInGameInventory.inGameInventory[index].AddItemsFromAnotherSlot(itemInHand.GetSlot());
                        itemInHand.SetItem(remainItems, index, StoredType.PlayerInGameInventory, true);
                    }
                    else
                    {
                        itemInHand.Swap(ref playerInGameInventory.inGameInventory, index, StoredType.PlayerInGameInventory, true);

                        if (dragType == DragType.Swap)
                        {
                            int startingSlotIndex = GetItemSlotIndex(startingSlotDrag);
                            if (playerInGameInventory.inGameInventory[startingSlotIndex].HasItem() == false)
                            {
                                startingSlotDrag = null;
                                itemInHand.Swap(ref playerInGameInventory.inGameInventory, startingSlotIndex, StoredType.PlayerInGameInventory, true);
                            }
                        }
                    }
                }
                else
                {
                    itemInHand.Swap(ref playerInGameInventory.inGameInventory, index, StoredType.PlayerInGameInventory, true);
                }

                UpdateInventoryUI();
            }
        }
        #endregion



        #region Inventory interactive methods

        private void OnLeftClick(int index)
        {
            handHasItem = itemInHand.HasItemData();
            slotHasItem = playerInGameInventory.inGameInventory[index].HasItem();

            if (handHasItem == false)
            {
                if (slotHasItem == false)
                {
                    //Debug.Log("HAND: EMPTY \t SLOT: EMPTY");
                }
                else
                {
                    //Debug.Log("HAND: EMPTY \t SLOT: HAS ITEM");
                    itemInHand.Swap(ref playerInGameInventory.inGameInventory, index, StoredType.PlayerInGameInventory, true);
                }
            }
            else
            {
                if (slotHasItem == false)
                {
                    itemInHand.Swap(ref playerInGameInventory.inGameInventory, index, StoredType.PlayerInGameInventory, true);
                }
                else
                {
                    //Debug.Log("HAND: HAS ITEM \t SLOT: HAS ITEM");
                    bool isSameItem = ItemData.IsSameItem(playerInGameInventory.inGameInventory[index].ItemData, itemInHand.GetItemData());
                    if (isSameItem)
                    {
                        ItemSlot remainItems = playerInGameInventory.inGameInventory[index].AddItemsFromAnotherSlot(itemInHand.GetSlot());
                        itemInHand.SetItem(remainItems, index, StoredType.PlayerInGameInventory, true);
                    }
                    else
                    {
                        itemInHand.Swap(ref playerInGameInventory.inGameInventory, index, StoredType.PlayerInGameInventory, true);
                    }
                }
            }
        }

        private void OnRightClick(int index)
        {
            handHasItem = itemInHand.HasItemData();
            slotHasItem = playerInGameInventory.inGameInventory[index].HasItem();

            if (handHasItem == false)
            {
                if (slotHasItem == false)
                {
                    //Debug.Log("HAND: EMPTY \t SLOT: EMPTY");
                }
                else
                {
                    //Debug.Log("HAND: EMPTY \t SLOT: HAS ITEM");
                    itemInHand.SplitItemSlotQuantityInInventory(ref playerInGameInventory.inGameInventory, index);
                }
            }
            else
            {
                if (slotHasItem == false)
                {
                    //Debug.Log("HAND: HAS ITEM \t SLOT: EMPTY");
                    playerInGameInventory.AddNewItemAt(index, itemInHand.GetItemData());
                    itemInHand.RemoveItem();

                }
                else
                {
                    //Debug.Log("HAND: HAS ITEM \t SLOT: HAS ITEM");
                    if (ItemData.IsSameItem(itemInHand.GetItemData(), playerInGameInventory.GetItem(index)))
                    {
                        bool isSlotNotFull = playerInGameInventory.AddItemAt(index);

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
            slotHasItem = playerInGameInventory.inGameInventory[index].HasItem();

            if (handHasItem == true)
            {
                if (slotHasItem == false)
                {
                    //Debug.Log("HAND: HAS ITEM \t SLOT: EMPTY");
                    playerInGameInventory.AddNewItemAt(index, itemInHand.GetItemData());
                    itemInHand.RemoveItem();

                }
                else
                {
                    //Debug.Log("HAND: HAS ITEM \t SLOT: HAS ITEM");

                    if (ItemData.IsSameItem(itemInHand.GetItemData(), playerInGameInventory.GetItem(index)))
                    {
                        bool isSlotNotFull = playerInGameInventory.AddItemAt(index);

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


