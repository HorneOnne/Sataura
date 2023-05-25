using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

namespace Sataura
{
    public class UIItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Internal References")]
        public Image mainImage;
        public Image defaultImage;
        public TextMeshProUGUI amountItemInSlotText;
        [SerializeField] private int slotIndex;
        private bool isEnter = false;
        private static Camera _mainCamera;


        private ItemSlot currentItemSlot = new ItemSlot();
        public int SlotIndex { get { return slotIndex; } protected set { slotIndex = value; } }

        private void Start()
        {
            if(_mainCamera == null)
            {
                _mainCamera = Camera.main;
            }
        }


        private void Update()
        {
            if(isEnter)
            {
                CharacterInventoryUIManager.Instance._itemDescPanel.position = (Vector2)_mainCamera.ScreenToWorldPoint(Input.mousePosition) + new Vector2(7, -3);                    
            }
        }

        public void SetIndex(int slotIndex = -1)
        {
            this.slotIndex = slotIndex;
        }

        
        public void SetData(ItemSlot itemSlot)
        {
            if (itemSlot == null || itemSlot.HasItemData() == false)
            {
                defaultImage.enabled = true;
                this.mainImage.sprite = null;
                this.mainImage.enabled = false;
                amountItemInSlotText.text = "";

                currentItemSlot.ClearSlot();
            }
            else
            {
                defaultImage.enabled = false;
                this.mainImage.enabled = true;
                this.mainImage.sprite = itemSlot.GetItemIcon();
                if (itemSlot.ItemQuantity > 1)
                    amountItemInSlotText.text = $"{itemSlot.ItemQuantity}";
                else
                    amountItemInSlotText.text = "";

                currentItemSlot = new ItemSlot(itemSlot);
            }
        }

        public void SetData(ItemSlot itemSlot, float opacity)
        {
            Color mainImageColor = mainImage.color;
            mainImageColor.a = opacity;

            Color defaultTextColor = amountItemInSlotText.color;
            defaultTextColor.a = opacity;

            if (itemSlot == null || itemSlot.HasItemData() == false)
            {
                defaultImage.enabled = true;
                this.mainImage.sprite = null;
                this.mainImage.enabled = false;
                amountItemInSlotText.text = "";

                currentItemSlot.ClearSlot();
            }
            else
            {
                defaultImage.enabled = false;
                this.mainImage.enabled = true;
                this.mainImage.sprite = itemSlot.GetItemIcon();
                if (itemSlot.ItemQuantity > 1)
                    amountItemInSlotText.text = $"{itemSlot.ItemQuantity}";
                else
                    amountItemInSlotText.text = "";

                currentItemSlot = new ItemSlot(itemSlot);
            }

            mainImage.color = mainImageColor;
            amountItemInSlotText.color = defaultTextColor;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (currentItemSlot.HasItemData())
            {
                isEnter = true;
                CharacterInventoryUIManager.Instance._itemDescCanvas.enabled = true;
                ItemDesc.Instance.CreateDesc(currentItemSlot.ItemData);
            }                     
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if(isEnter)
            {
                isEnter = false;
                CharacterInventoryUIManager.Instance._itemDescCanvas.enabled = false;
            }       
        }
    }
}