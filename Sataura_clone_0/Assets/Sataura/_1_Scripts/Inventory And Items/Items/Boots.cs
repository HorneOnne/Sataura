using UnityEngine;

namespace Sataura
{
    public class Boots : Item 
    {
        [Header("References")]
        [SerializeField] private GameObject vfxDoubleJumpPrefab;

        public void DoubleJump(Player player)
        {
            if(player.playerMovement.NumOfJumps == 1)
            {
                // VFX
                var doubleJumpVFXObj = Instantiate(vfxDoubleJumpPrefab, player.playerMovement.groundCheckPoint.position, Quaternion.identity);
                Destroy(doubleJumpVFXObj, 0.3f);


                // Double jump
                player.playerMovement.Rb2D.velocity = Vector2.zero;
                player.playerMovement.HandleJump();            
            }


            
        }
    }
}