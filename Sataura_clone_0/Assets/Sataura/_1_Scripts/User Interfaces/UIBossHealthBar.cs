using UnityEngine;
using UnityEngine.UI;

namespace Sataura
{
    public class UIBossHealthBar : MonoBehaviour
    {
        public static UIBossHealthBar Instance { get;private set; }
        public static event System.Action OnBossAppeared;

        [SerializeField] private Slider _healthBarSlider;
        private KingSlime _kingSlime;



        private void Awake()
        {
            Instance = this;
        }

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

