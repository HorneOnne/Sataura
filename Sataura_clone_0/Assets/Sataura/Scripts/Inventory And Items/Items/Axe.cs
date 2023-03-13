using UnityEngine;

namespace Sataura
{
    public class Axe : Item, IUseable
    {
        public override bool Use(Player player, Vector2 mousePosition)
        {
            return true;
        }
    }
}
