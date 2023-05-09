using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Sataura
{
    public class UIEquipSlot : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] protected Image _defaultImage;
        [SerializeField] protected Image _itemImage;
        [SerializeField] protected Image _lockImage;
               


        [Header("Properties")]
        [SerializeField] protected bool isLocking;
        [SerializeField] protected EquipmentSlotType equipmentType;


        #region Properties
        public bool IsLocking { get => isLocking; }
        public EquipmentSlotType EequipmentType { get { return equipmentType; } }
        #endregion


        private void Start() 
        {
            if(isLocking)
            {
                SetLockState();
            }
            else
            {
                SetUnlockState();
            }
        }


        protected void SetLockState()
        {
            isLocking = true;

            _defaultImage.gameObject.SetActive(false);
            _itemImage.gameObject.SetActive(false);
            _lockImage.gameObject.SetActive(true);
        }

        protected void SetUnlockState()
        {
            isLocking = false;

            _defaultImage.gameObject.SetActive(true);
            _itemImage.gameObject.SetActive(false);
            _lockImage.gameObject.SetActive(false);
        }

        protected void SetItemState()
        {
            isLocking = false;

            _defaultImage.gameObject.SetActive(false);
            _itemImage.gameObject.SetActive(true);
            _lockImage.gameObject.SetActive(false);
        }

        protected void SetNoItemState()
        {
            isLocking = false;

            _defaultImage.gameObject.SetActive(true);
            _itemImage.gameObject.SetActive(false);
            _lockImage.gameObject.SetActive(false);
        }

        public virtual void UpdateItemImage(ItemData itemData)
        {
            if (isLocking) return;

            if (itemData == null)
            {
                SetNoItemState();
            }
            else
            {
                SetItemState();

                _itemImage.sprite = itemData.icon;
            }         
        }


        public enum EquipmentSlotType
        {
            Hook,
            Boots,
            Helmet,
            Chestplate,
            Legging,
            Accessory,
        }
    }
}
