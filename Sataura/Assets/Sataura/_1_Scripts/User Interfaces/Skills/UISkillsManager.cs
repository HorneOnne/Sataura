using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

namespace Sataura
{
    public class UISkillsManager : MonoBehaviour
    {
        public static UISkillsManager Instance { get; private set; }

        [Header("References")]
        [SerializeField] private InventoryPlayer inventoryPlayer;
        [SerializeField] private Transform uiWeaponsParent;
        [SerializeField] private Transform uiAccessoriesParent;
        [SerializeField] private UISkillSlot uiSkillSlotPrefab;  
        private ItemInHand itemInHand;
        public List<UISkillSlot> weapons;
        public List<UISkillSlot> accessories;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            itemInHand = inventoryPlayer.itemInHand;
            LoadUI();
            UpdateUI();
        }



        private void LoadUI()
        {
            for (int i = 0; i < inventoryPlayer.playerSkills.weaponsData.itemSlots.Count; i++)
            {
                var skillSlotObject = Instantiate(uiSkillSlotPrefab, uiWeaponsParent.transform);
                skillSlotObject.SetItemCategory(ItemCategory.Skill_Weapons);
                AddUIEvent(skillSlotObject);
                weapons.Add(skillSlotObject);

                if (i == 0)
                {
                    skillSlotObject.SetUnlockState();
                }
            }


            for (int i = 0; i < inventoryPlayer.playerSkills.accessoriesData.itemSlots.Count; i++)
            {
                var skillSlotObject = Instantiate(uiSkillSlotPrefab, uiAccessoriesParent.transform);
                skillSlotObject.SetItemCategory(ItemCategory.Skill_Accessories);
                AddUIEvent(skillSlotObject);
                accessories.Add(skillSlotObject);

                if (i == 0)
                {
                    skillSlotObject.SetUnlockState();
                }
            }
        }


        private void AddUIEvent(UISkillSlot slot)
        {
            GameObject slotObject = slot.gameObject;
            Utilities.AddEvent(slotObject, EventTriggerType.PointerDown, (baseEvent) => OnClick(baseEvent, slotObject));
        }


        public void OnClick(BaseEventData baseEvent, GameObject clickedObject)
        {
            PointerEventData pointerEventData = (PointerEventData)baseEvent;
            UISkillSlot clickedUISkillSlot = clickedObject.GetComponent<UISkillSlot>();
            if (clickedUISkillSlot.IsLocking) return;

            var slotCategory = clickedUISkillSlot.ItemSlotCategory;
            int index = GetClickedUISkillSlotIndex(clickedUISkillSlot, slotCategory);
            ItemSlot skillSlotData = GetPlayerSkillSlot(slotCategory, index);


            if (itemInHand.HasItemData() == false)
            {
                if (skillSlotData.HasItemData() == false)
                {
                    //Debug.Log("HAND: EMPTY \t SLOT: EMPTY");

                }
                else
                {
                    //Debug.Log("HAND: EMPTY \t SLOT: HAS ITEM");
                    inventoryPlayer.playerSkills.UpdateStatsUnequip(skillSlotData.ItemData);

                    itemInHand.SetItem(skillSlotData);
                    skillSlotData.ClearSlot();
                }
            }
            else
            {
                if (itemInHand.GetItemData().itemCategory != clickedUISkillSlot.ItemSlotCategory)
                    return;

                if (skillSlotData.HasItemData() == false)
                {
                    //Debug.Log("HAND: HAS ITEM \t SLOT: EMPTY");
                    skillSlotData.AddItemsFromAnotherSlot(itemInHand.GetSlot());
                    itemInHand.ClearSlot();

                    inventoryPlayer.playerSkills.UpdateStatsEquip(skillSlotData.ItemData);
                }
                else
                {
                    //Debug.Log("HAND: HAS ITEM \t SLOT: HAS ITEM");
                }

            }

            UpdateUI();
            UIItemInHand.Instance.UpdateItemInHandUI();
            EventManager.TriggerPlayerEquipmentChangedEvent();
        }


        private ItemSlot GetPlayerSkillSlot(ItemCategory itemSlotCategory, int index)
        {
            switch (itemSlotCategory)
            {
                case ItemCategory.Skill_Weapons:
                    return inventoryPlayer.playerSkills.weaponsData.itemSlots[index];
                case ItemCategory.Skill_Accessories:
                    return inventoryPlayer.playerSkills.accessoriesData.itemSlots[index];
                default:
                    throw new System.Exception();
            }
            throw new System.Exception();
        }

        private int GetClickedUISkillSlotIndex(UISkillSlot clickedObject, ItemCategory itemSlotCategory)
        {
            switch (itemSlotCategory)
            {
                case ItemCategory.Skill_Weapons:
                    for (int i = 0; i < weapons.Count; i++)
                    {
                        if (clickedObject == weapons[i])
                            return i;
                    }
                    break;
                case ItemCategory.Skill_Accessories:
                    for (int i = 0; i < accessories.Count; i++)
                    {
                        if (clickedObject == accessories[i])
                            return i;
                    }
                    break;
                default:
                    throw new System.Exception();
            }
            throw new System.Exception();
        }

        private void UpdateUI()
        {
            for (int i = 0; i < weapons.Count; i++)
            {
                weapons[i].UpdateItemImage(inventoryPlayer.playerSkills.weaponsData.itemSlots[i].ItemData);
            }

            for (int i = 0; i < accessories.Count; i++)
            {
                accessories[i].UpdateItemImage(inventoryPlayer.playerSkills.accessoriesData.itemSlots[i].ItemData);
            }
        }
    }
}
