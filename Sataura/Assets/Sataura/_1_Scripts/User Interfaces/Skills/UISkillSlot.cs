using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Sataura
{
    public class UISkillSlot : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] protected Image _defaultImage;
        [SerializeField] protected Image _itemImage;
        [SerializeField] protected Image _lockImage;
        [SerializeField] protected Image _overlayImage;


        [Header("Properties")]
        [SerializeField] protected bool isLocking;
        [SerializeField] protected ItemCategory itemSlotCategory;
        [SerializeField] private Sprite _defaultWeaponSprite;
        [SerializeField] private Sprite _defaultAccessorySprite;
        [SerializeField] private Color _defaultWeaponSpriteColor;
        [SerializeField] private Color _defaultAccessorySpriteColor;
        [SerializeField] private Color _overlaySpriteColorWhenUnlocking;



        #region Properties
        public bool IsLocking { get => isLocking; }
        public ItemCategory ItemSlotCategory { get { return itemSlotCategory; } }
        #endregion

        private void Awake()
        {
            if (isLocking)
            {
                SetLockState();
            }
            else
            {
                SetUnlockState();
            }
        }


        public void SetLockState()
        {
            isLocking = true;

            _defaultImage.gameObject.SetActive(false);
            _itemImage.gameObject.SetActive(false);
            _lockImage.gameObject.SetActive(true);
        }

        public void SetUnlockState()
        {
            isLocking = false;

            _defaultImage.gameObject.SetActive(true);
            _itemImage.gameObject.SetActive(false);
            _lockImage.gameObject.SetActive(false);

            // Set overlayImage color when this slot unlock.
            _overlayImage.color = _overlaySpriteColorWhenUnlocking;
        }

        public void SetItemState()
        {
            isLocking = false;

            _defaultImage.gameObject.SetActive(false);
            _itemImage.gameObject.SetActive(true);
            _lockImage.gameObject.SetActive(false);
        }

        public void SetNoItemState()
        {
            isLocking = false;

            _defaultImage.gameObject.SetActive(true);
            _itemImage.gameObject.SetActive(false);
            _lockImage.gameObject.SetActive(false);
        }


        public void SetItemCategory(ItemCategory itemCategory)
        {
            this.itemSlotCategory = itemCategory;

            // Set default sprite icon.
            switch(this.itemSlotCategory)
            {
                case ItemCategory.Skill_Weapons:
                    _defaultImage.sprite = _defaultWeaponSprite;
                    _defaultImage.color = _defaultWeaponSpriteColor;
                    break;
                case ItemCategory.Skill_Accessories:
                    _defaultImage.sprite = _defaultAccessorySprite;
                    _defaultImage.color = _defaultAccessorySpriteColor;
                    break;
                default:break;
            }
        }

        public virtual void UpdateItemImage(ItemData itemData)
        {
            if (IsLocking) return;

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
    }
}
