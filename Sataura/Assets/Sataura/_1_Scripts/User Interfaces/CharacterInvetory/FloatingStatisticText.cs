using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Sataura
{
    public class FloatingStatisticText : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TextMeshProUGUI _floatingStatName;
        [SerializeField] private TextMeshProUGUI _floatingStatValue;


        [Header("Properties")]
        [SerializeField] private Color _increaseStatsColor;
        [SerializeField] private Color _decreaseStatsColor;

        private float _timeElapse = 0.0f;
        private float _timeExist = 0.0f;
        private float _floatingSpeed = 3.0f;
        private bool alreadySetup = false;
        private bool isIncreaseStats = true;

      

        public void SetUp(string statsName, float statsValue)
        {
            if (statsValue >= 0.0f)
                isIncreaseStats = true;
            else
                isIncreaseStats = false;


            if(isIncreaseStats)
            {
                _timeExist = Random.Range(2.0f, 3.0f);

                _floatingStatName.text = statsName;
                _floatingStatValue.color = _increaseStatsColor;
                _floatingStatValue.text = $"+ {statsValue}";
            }
            else
            {
                _timeExist = Random.Range(1.0f, 2.0f);

                _floatingStatName.text = statsName;
                _floatingStatValue.color = _decreaseStatsColor;
                _floatingStatValue.text = $"- {Mathf.Abs(statsValue)}";
            }

            alreadySetup = true;
        }

        void Update()
        {
            if (alreadySetup == false) return;

            _timeElapse += Time.deltaTime;      
            if(_timeElapse < _timeExist)
            {
                if(isIncreaseStats)
                {
                    if (_floatingSpeed > 1.0f)
                    {
                        transform.position += Vector3.up * _floatingSpeed * Time.deltaTime;
                        _floatingSpeed -= Time.deltaTime;
                    }
                    else
                    {
                        transform.position += Vector3.up * Time.deltaTime;
                    }
                }
                else
                {
                    if (_floatingSpeed > 1.0f)
                    {
                        transform.position += Vector3.up * _floatingSpeed * Time.deltaTime;
                        _floatingSpeed -= Time.deltaTime + 0.02f;
                    }
                    else
                    {
                        transform.position += Vector3.up * Time.deltaTime;
                    }
                }
                
                
            }
            else
            {
                FloatingStatisticTextManager.Instance._totalFloatingStatsTextOnScene -= 1;
                Destroy(this.gameObject);
            }
        }
    }
}
