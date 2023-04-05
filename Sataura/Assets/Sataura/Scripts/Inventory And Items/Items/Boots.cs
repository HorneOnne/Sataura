using UnityEngine;

namespace Sataura
{
    public class Boots : Item 
    {
        [Header("References")]
        [SerializeField] private GameObject vfxDoubleJumpPrefab;

        public void DoubleJump(Player player)
        {
            if(player.PlayerMovement.NumOfJumps == 1)
            {
                // VFX
                var doubleJumpVFXObj = Instantiate(vfxDoubleJumpPrefab, player.PlayerMovement.groundCheckPoint.position, Quaternion.identity);
                Destroy(doubleJumpVFXObj, 0.3f);


                // Double jump
                player.PlayerMovement.Rb2D.velocity = Vector2.zero;
                player.PlayerMovement.HandleJump();            
            }


            
        }
    }
}