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



        private void OnEnable()
        {
            button.onClick.AddListener(OnClick);
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(OnClick);
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

