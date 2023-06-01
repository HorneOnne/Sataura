using TMPro;
using UnityEngine;

namespace Sataura
{
    public class UIEnterServerPort : SatauraCanvas
    {
        [SerializeField] private TMP_InputField _inputField;
        public override void DisplayCanvas(bool isDisplay)
        {
            base.DisplayCanvas(isDisplay);

            if (isDisplay)
            {
                _inputField.Select();
                _inputField.text = "";
            }

        }
    }

}
