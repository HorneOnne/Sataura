using UnityEngine;

namespace Sataura
{
    public class CharacterInventoryUIManager : Singleton<CharacterInventoryUIManager>
    {
        public Canvas _floatingTextCanvas;
        public Canvas _itemDescCanvas;
        public RectTransform _itemDescPanel;

        private void Start()
        {
            if (_itemDescCanvas != null)
                _itemDescCanvas.enabled = false;
        }


    }
}
