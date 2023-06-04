using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

namespace Sataura
{
    public class SatauraButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("References")]
        protected TextMeshProUGUI btnText;
        protected Button button;


        #region Properties
        public Button Button { get { return button; } }    
        #endregion

        private void OnEnable()
        {
            button.onClick.AddListener(OnClick);
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(OnClick);
        }


        private void Awake()
        {
            button = GetComponent<Button>();
            btnText = button.GetComponentInChildren<TextMeshProUGUI>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            SoundManager.Instance.PlaySound(SoundType.MainMenuBtnHover, playRandom: false);          
        }

        public void OnPointerExit(PointerEventData eventData)
        {

        }

        public virtual void OnClick() { }
    


    }
}

