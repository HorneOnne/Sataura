using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sataura
{
    public class UIIngameInformationManager : Singleton<UIIngameInformationManager>
    {
        [Header("UI Kill")]
        [SerializeField] private TextMeshProUGUI killAmountText;


        [Header("UI Level")]
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private Slider levelSlider;


        [Header("UI Upgrade Skills")]
        [SerializeField] private UIUpgradeSkill[] uIUpgradeSkills = new UIUpgradeSkill[3]; 


        [Header("Runtime References")]
        [SerializeField] private IngameInformationManager ingameInformationManager;





        private const float updateTimeFrequency = 0.5f;
        private float updateTimeCount = 0.0f;

        private void OnEnable()
        {
            IngameInformationManager.OnPlayerLevelUp += SetSliderLevelValue;
            IngameInformationManager.OnPlayerLevelUp += UpdateLevelText;

            IngameInformationManager.OnPlayerLevelUp += GenerateUpgradeList;
            
        }


        private void OnDisable()
        {
            IngameInformationManager.OnPlayerLevelUp -= SetSliderLevelValue;
            IngameInformationManager.OnPlayerLevelUp -= UpdateLevelText;

            IngameInformationManager.OnPlayerLevelUp -= GenerateUpgradeList;
        }


        private void Start()
        {
            ingameInformationManager = IngameInformationManager.Instance;

            // Reset level text.
            levelText.text = ingameInformationManager.Level.ToString();

            // Reset Level Slider.
            SetSliderLevelValue();
        }


        private void Update()
        {
            if(Time.time - updateTimeCount >= updateTimeFrequency)
                updateTimeCount = Time.time;
            else
                return;

            killAmountText.text = ingameInformationManager.totalEnemiesKilled.ToString();
            levelSlider.value = ingameInformationManager.Experience;
        }

        private void UpdateLevelText()
        {
            levelText.text = ingameInformationManager.Level.ToString();
        }


        private void SetSliderLevelValue()
        {
            levelSlider.minValue = ingameInformationManager.Experience;
            levelSlider.maxValue = ingameInformationManager.ExperienceToNextLevel;
        }

        
        public void FakeFullExpSliderWhenLevelUp()
        {
            levelSlider.value = levelSlider.maxValue;
        }

        private void GenerateUpgradeList()
        {
            // Retrieve an array of all values in the enum
            ItemType[] itemTypes = (ItemType[])Enum.GetValues(typeof(ItemType));

            // Iterate through the array using a foreach loop
            foreach (ItemType itemType in itemTypes)
            {
                Debug.Log(itemType);
            }   

            for (int i = 0; i < uIUpgradeSkills.Length; i++)
            {
                if (uIUpgradeSkills[i] != null)
                {
                    ItemData itemData = GameDataManager.Instance.upgradeSkills[i];
                    uIUpgradeSkills[i].UpdateData(itemData);
                }
            }
        }
    }
}

