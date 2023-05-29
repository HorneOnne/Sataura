using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Sataura
{
    public class UIItemInHand : Singleton<UIItemInHand>
    {
        [Header("Runtime References")]
        [SerializeField] private InventoryPlayer _inventoryPlayer;

        private ItemInHand itemInHand;
        [SerializeField] GameObject uiSlotPrefab;
        [HideInInspector] public GameObject uiSlotDisplay;
        [SerializeField] private float scaleUI = 1.0f;


        public Image UISlotImage { get; private set; }


        // Cached
        Camera mainCamera;
        Vector2 mainCameraPosition;

        private void OnEnable()
        {
            EventManager.OnItemInHandChanged += ResetUIItemInHandColor;
        }

        private void OnDisable()
        {
            EventManager.OnItemInHandChanged -= ResetUIItemInHandColor;
        }


    
        private bool alreadyLoadReferences;
        private IEnumerator LoadReferences()
        {
            yield return new WaitUntil(() => _inventoryPlayer != null);
            mainCamera = Camera.main;
            itemInHand = _inventoryPlayer.itemInHand;

            alreadyLoadReferences = true;
        }


        public void SetPlayer(GameObject playerObject)
        {
            this._inventoryPlayer = playerObject.GetComponent<InventoryPlayer>();
            StartCoroutine(LoadReferences());
        }

        private void Update()
        {
            if (alreadyLoadReferences == false) return;


            if (_inventoryPlayer.itemInHand.HasItemData() && uiSlotDisplay != null)
            {
                mainCameraPosition = (Vector2)mainCamera.ScreenToWorldPoint(Input.mousePosition);
                uiSlotDisplay.GetComponent<RectTransform>().transform.position = mainCameraPosition;
            }
            else
            {
                if (uiSlotDisplay != null)
                {
                    DestroyImmediate(uiSlotDisplay);
                }
            }
        }


        public void UpdateItemInHandUI(Transform parent = null)
        {
            if (itemInHand.HasItemData() == false)
            {
                itemInHand.ClearSlot();
                return;
            }

            if (uiSlotDisplay != null || UISlotImage != null)
            {
                UISlotImage.sprite = itemInHand.GetItemData().icon;               
                SetItemQuantityText();
            }
            else
            {
                uiSlotDisplay = Instantiate(uiSlotPrefab, this.transform.parent.transform);
                uiSlotDisplay.transform.localScale = Vector3.one * scaleUI;
                UISlotImage = uiSlotDisplay.GetComponent<UIItemSlot>().mainImage;
                UISlotImage.sprite = itemInHand.GetItemData().icon;
                SetItemQuantityText();

                if (parent != null)
                {
                    uiSlotDisplay.transform.SetParent(parent.transform);
                }

                uiSlotDisplay.name = $"InHandItem";
                uiSlotDisplay.GetComponent<RectTransform>().SetAsLastSibling();
            }
            
        }

        private void SetItemQuantityText()
        {
            int itemQuantity = itemInHand.GetSlot().ItemQuantity;
            if (itemQuantity > 1)
                uiSlotDisplay.GetComponent<UIItemSlot>().amountItemInSlotText.text = itemInHand.GetSlot().ItemQuantity.ToString();
            else
                uiSlotDisplay.GetComponent<UIItemSlot>().amountItemInSlotText.text = "";
        }

        private void ResetUIItemInHandColor()
        {
            if (uiSlotDisplay != null || UISlotImage != null)
                UISlotImage.color = new Color(1, 1, 1, 1);
        }
    }
}