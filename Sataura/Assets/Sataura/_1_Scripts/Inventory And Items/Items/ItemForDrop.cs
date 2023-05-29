using UnityEngine;

namespace Sataura
{
    public class ItemForDrop : Item
    {

        public void Set(ItemSlot itemSlot)
        {
            base.SetData(itemSlot);
        }


        public override void Collect(IngamePlayer player)
        {
            Debug.Log("Collect");
        }
    }
}
