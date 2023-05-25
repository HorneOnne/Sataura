using UnityEngine;
using TMPro;
using System;

namespace Sataura
{
    public class CharacterStatsSlot : MonoBehaviour
    {
        public CharacterStats _characterStat;
        [SerializeField] private TextMeshProUGUI _valueText;
        [SerializeField] private TextMeshProUGUI _additionValueText;


        public void SetValueUI(float value, float additionValue)
        {
            if (value == 0)
            {
                this._valueText.text = "-";
            }
            else
            {
                this._valueText.text = value.ToString("0.0");
            }

            if (additionValue == 0)
            {
                this._additionValueText.text = "";
            }
            else
            {
                this._additionValueText.text = $"|+{additionValue.ToString("0.0")}|";
            }
        }
    }
}
