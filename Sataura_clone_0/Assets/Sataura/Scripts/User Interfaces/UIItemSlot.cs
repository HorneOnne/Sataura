using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

namespace Sataura
{
    public class UIItemSlot : MonoBehaviour//, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Internal References")]
        public Image mainImage;
        public Image defaultImage;
        public TextMeshProUGUI amountItemInSlotText;
        [SerializeField] private int slotIndex;

        private ItemSlot currentItemSlot = new ItemSlot();
        public int SlotIndex { get { return slotIndex; } protected set { slotIndex = value; } }



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
                amountItemInSlotText.text = "";

                currentItemSlot.ClearSlot();
            }
            else
            {
                defaultImage.enabled = false;
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
                amountItemInSlotText.text = "";

                currentItemSlot.ClearSlot();
            }
            else
            {
                defaultImage.enabled = false;
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


       



        /*public void OnPointerEnter(PointerEventData eventData)
        {
            if(currentItemSlot.HasItem())
            {
                ItemDescriptionManager.Instance.SetItemData(currentItemSlot.ItemData);
                StartCoroutine(Show());
            }         
        }

        IEnumerator Show()
        {
            yield return new WaitForSeconds(0.3f);
            ItemDescriptionManager.Instance.Show();
            UIManager.Instance.ItemDescCanvas.SetActive(true);
                  
        }



        public void OnPointerExit(PointerEventData eventData)
        {
            StopAllCoroutines();
            if (currentItemSlot.HasItem())
            {
                ItemDescriptionManager.Instance.SetItemData(null);
                ItemDescriptionManager.Instance.Hide();
                UIManager.Instance.ItemDescCanvas.SetActive(false);
            }
            
        }*/

    }
}