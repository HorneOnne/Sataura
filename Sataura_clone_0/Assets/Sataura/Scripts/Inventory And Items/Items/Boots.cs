using UnityEngine;

namespace Sataura
{
    public class Boots : Item 
    {
        [Header("Boots properties")]
        private bool canDoubleJump = false;


        public override void UsePassive(Player player, Vector2 mousePosition)
        {
            base.UsePassive(player, mousePosition);
        }
    }
}