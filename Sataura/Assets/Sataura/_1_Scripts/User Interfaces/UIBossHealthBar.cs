using UnityEngine;
using UnityEngine.UI;

namespace Sataura
{
    public class UIBossHealthBar : Singleton<UIBossHealthBar>
    {
        public static event System.Action OnBossAppeared;

        [SerializeField] private Slider _healthBarSlider;
        private KingSlime _kingSlime;



   

        public void SetBossHealthValue(KingSlime kingSlime)
        {
            this._kingSlime = kingSlime;

            _healthBarSlider.minValue = 0;
            _healthBarSlider.maxValue = _kingSlime.MaxHealth;
        }


        private void Update()
        {
            if (_kingSlime == null) return;
        
            _healthBarSlider.value = _kingSlime.CurrentHealth;
        }
    }
}

