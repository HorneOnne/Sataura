using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Sataura
{
    public class UIUpgradeSkill : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private ItemData itemData;

        [Header("UI")]
        [SerializeField] private Image skillIcon;
        [SerializeField] private TextMeshProUGUI skillNameText;
        [SerializeField] private TextMeshProUGUI skillDescText;


        public void SetData(ItemData _itemData)
        {
            this.itemData= _itemData;
        }

        public void UpdateData()
        {
            skillIcon.sprite = itemData.icon;
            skillNameText.text = itemData.name;
            skillDescText.text = itemData.description;
        }

        public void UpdateData(ItemData _itemData)
        {
            this.itemData = _itemData;

            skillIcon.sprite = itemData.icon;
            skillNameText.text = itemData.name;
            skillDescText.text = itemData.description;
        }

        private void OnEnable()
        {
            UpdateData();
        }


        public void OnUIUpgradeSkillButtonClicked()
        {
            Time.timeScale = 1.0f;
            UIManager.Instance.CloseUpgradeItemSkillPanel();
        }

    }
}


