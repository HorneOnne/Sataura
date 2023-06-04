using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Sataura
{
    public class TabManager : MonoBehaviour
    {
        public static TabManager Instance { get; private set; }

        [SerializeField] private Sprite _defaultSprite;
        [SerializeField] private Sprite _selectedSprite;
        [SerializeField] private Button _allTabs;
        [SerializeField] private Button _weaponsTabs;
        [SerializeField] private Button _armorTabs;
        [SerializeField] private Button _currentTab;



        private void Awake()
        {
            Instance = this;
        }

        private void OnEnable()
        {          
            _allTabs.onClick.AddListener(AllTabClicked);
            _weaponsTabs.onClick.AddListener(WeaponsTabClicked);
            _armorTabs.onClick.AddListener(ArmorsTabClicked);
        }

        private void Start()
        {
            AllTabClicked();
        }



        /* public void OnInventoryTabClick(Button clickedBtn)
         {
             for(int i = 0; i < _inventoryTabs.Count; i++)
             {
                 if (_inventoryTabs[i] == clickedBtn)
                 {
                     _currentInventoryTabIndex = i;
                     Debug.Log($"index at: {i}");
                     break;
                 }
             }
         }

         private void UpdateInventoryTabsUI()
         {
             for (int i = 0; i < _inventoryTabs.Count; i++)
             {
                 if(_currentInventoryTabIndex == i)
                 {
                     _inventoryTabs[i].GetComponent<Image>().sprite = _selectedSprite;
                 }
                 else
                 {
                     _inventoryTabs[i].GetComponent<Image>().sprite = _defaultSprite;
                 }           
             }
         }
 */
        private void OnTabChangedMethod()
        {
            _allTabs.GetComponent<Image>().sprite = _defaultSprite;
            _weaponsTabs.GetComponent<Image>().sprite = _defaultSprite;
            _armorTabs.GetComponent<Image>().sprite = _defaultSprite;

            _currentTab.GetComponent<Image>().sprite = _selectedSprite;
        }

        private void AllTabClicked()
        {
            _currentTab = _allTabs;
            OnTabChangedMethod();

        }
        private void WeaponsTabClicked()
        {
            _currentTab = _weaponsTabs;

            OnTabChangedMethod();
        }
        private void ArmorsTabClicked()
        {
            _currentTab = _armorTabs;
            OnTabChangedMethod();
        }

    }
}
