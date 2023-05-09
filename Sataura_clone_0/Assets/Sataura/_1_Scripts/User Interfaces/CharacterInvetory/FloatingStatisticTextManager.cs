using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sataura
{
    public class FloatingStatisticTextManager : Singleton<FloatingStatisticTextManager>
    {
        [Header("References")]
        [SerializeField] private FloatingStatisticText _floatingStatTextPrefab;
        [SerializeField] private RectTransform _showPosition;
        [SerializeField] private Transform _parent;

        [Header("Properties")]
        public int _totalFloatingStatsTextOnScene = 0;

        public void ShowFloatingStatText(string statsName, float statsValue)
        {
            if (_totalFloatingStatsTextOnScene > 15) return;

            var floatingText = Instantiate(_floatingStatTextPrefab, _parent);
            floatingText.GetComponent<RectTransform>().position = _showPosition.position;
            floatingText.SetUp(statsName, statsValue);

            _totalFloatingStatsTextOnScene += 1;
        }

        public IEnumerator ShowFloatingStatTextAfter(string statsName, float statsValue, float waitTime = 0.0f)
        {
            if (_totalFloatingStatsTextOnScene > 15) yield break;
            yield return new WaitForSeconds(waitTime);

            var floatingText = Instantiate(_floatingStatTextPrefab, _parent);
            floatingText.GetComponent<RectTransform>().position = _showPosition.position;
            floatingText.SetUp(statsName, statsValue);

            _totalFloatingStatsTextOnScene += 1;
        }

    }
}
