using UnityEngine.UI;

namespace Sataura
{
    public class Custom_ScrollRect : ScrollRect
    {
        override protected void LateUpdate()
        {
            base.LateUpdate();
            if (this.verticalScrollbar)
            {
                this.verticalScrollbar.size = 0.1f;
            }
        }

        override public void Rebuild(CanvasUpdate executing)
        {
            base.Rebuild(executing);
            if (this.verticalScrollbar)
            {
                this.verticalScrollbar.size = 0.1f;
            }
        }
    }
}

