using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

namespace Sataura
{
    public class SatauraButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("References")]
        [SerializeField] private TextMeshProUGUI btnText;
        [SerializeField] private Button button;
        protected MainMenuUIManager mainMenuUIManager;

        // Text properties
        // ---------------
        private const int DEFAULT_BTN_TEXT_FONTSIZE = 60;
        private const string defaultColor = "#FFFFFF";
        private const string highlightColor = "#FFE500";

        private void OnEnable()
        {
            button.onClick.AddListener(OnClick);

            btnText.fontSize = DEFAULT_BTN_TEXT_FONTSIZE;
            btnText.color = Utilities.HexToColor(defaultColor);
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(OnClick);
        }

        private void Start()
        {
            mainMenuUIManager = MainMenuUIManager.Instance;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            btnText.fontSize = DEFAULT_BTN_TEXT_FONTSIZE + (DEFAULT_BTN_TEXT_FONTSIZE * 15 / 100);
            btnText.color = Utilities.HexToColor(highlightColor);

            SoundManager.Instance.PlaySound(SoundType.MainMenuBtnHover);          
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            btnText.fontSize = DEFAULT_BTN_TEXT_FONTSIZE;
            btnText.color = Utilities.HexToColor(defaultColor);
        }

        public virtual void OnClick() { }
    
    }
}

