using UnityEngine;

namespace Sataura
{
    public class SatauraCanvas : MonoBehaviour
    {
        [SerializeField] protected Canvas _canvas;

        #region Properties
        public Canvas Canvas { get { return _canvas; } }
        #endregion

        public virtual void DisplayCanvas(bool isDisplay)
        {
            _canvas.enabled = isDisplay;
        }
    }

}
