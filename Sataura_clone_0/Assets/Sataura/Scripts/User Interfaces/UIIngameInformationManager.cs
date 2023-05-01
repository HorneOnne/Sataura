using System.Collections.Generic;
using System.Linq;
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
        [SerializeField] private GameObject uIUpgradeSkillPrefab;
        [SerializeField] private Transform uIUpgradeSkillParent;
        [SerializeField] private List<UIUpgradeSkill> uIUpgradeSkills = new List<UIUpgradeSkill>();


        [Header("Runtime References")]
        [SerializeField] private IngameInformationManager ingameInformationManager;





        private const float updateTimeFrequency = 0.5f;
        private float updateTimeCount = 0.0f;

        private void OnEnable()
        {
            IngameInformationManager.OnPlayerLevelUp += SetSliderLevelValue;
            UIUpgradeSkill.OnUpgradeButtonClicked += UpdateLevelText;
            IngameInformationManager.OnPlayerLevelUp += FakeFullExpSliderWhenLevelUp;

            IngameInformationManager.OnPlayerLevelUp += GenerateUpgradeList;

        }


        private void OnDisable()
        {
            IngameInformationManager.OnPlayerLevelUp -= SetSliderLevelValue;
            UIUpgradeSkill.OnUpgradeButtonClicked -= UpdateLevelText;
            IngameInformationManager.OnPlayerLevelUp -= FakeFullExpSliderWhenLevelUp;

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
            if (Time.time - updateTimeCount >= updateTimeFrequency)
                updateTimeCount = Time.time;
            else
                return;

            killAmountText.text = ingameInformationManager.totalEnemiesKilled.ToString();
            levelSlider.value = ingameInformationManager.Experience;
        }

        public void UpdateLevelText()
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
            Debug.LogWarning("Optimize this.");

            var upgradeItemDict = ItemEvolutionManager.Instance.GenerateUpgradeItemData(GameDataManager.Instance.singleModePlayer.GetComponent<Player>());

            for (int i = 0; i < uIUpgradeSkills.Count; i++)
            {
                Destroy(uIUpgradeSkills[i].gameObject);
            }
            uIUpgradeSkills.Clear();


            for (int i = 0; i < upgradeItemDict.Count; i++)
            {
                var uiUpgradeSkillObject = Instantiate(uIUpgradeSkillPrefab, uIUpgradeSkillParent);
                uIUpgradeSkills.Add(uiUpgradeSkillObject.GetComponent<UIUpgradeSkill>());
             
                ItemData upgradeItem = upgradeItemDict.ElementAt(i).Key;
                uIUpgradeSkills[i].UpdateData(upgradeItem, upgradeItemDict.ElementAt(i).Value);
            }
        }
    }
}


