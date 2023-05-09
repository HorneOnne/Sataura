namespace Sataura
{
    public class ItemForDrop : Item
    {

        public void Set(ItemSlot itemSlot)
        {
            base.SetData(itemSlot);
        }


        public override void Collect(Player player)
        {
            var returnSlot = player.playerInGameInventory.AddItem(ItemSlot);


            if (returnSlot.HasItemData() == true)
            {
                Set(returnSlot);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
