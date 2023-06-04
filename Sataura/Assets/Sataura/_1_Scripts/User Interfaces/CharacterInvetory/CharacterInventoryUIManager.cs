using UnityEngine;

namespace Sataura
{
    public class CharacterInventoryUIManager : MonoBehaviour
    {
        public static CharacterInventoryUIManager Instance { get; private set; }

        public Canvas _floatingTextCanvas;
        public Canvas _itemDescCanvas;
        public RectTransform _itemDescPanel;


        private void Awake()
        {
            Instance = this;    
        }

        private void Start()
        {
            if (_itemDescCanvas != null)
                _itemDescCanvas.enabled = false;
        }


    }
}
