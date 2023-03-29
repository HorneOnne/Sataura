using UnityEngine;

namespace Sataura
{
    public abstract class GroundEnemy : BaseEnemy
    {
        [Header("Ground check properties")]
        [SerializeField] protected LayerMask groundLayer;
        [SerializeField] protected Transform groundCheckPoint;      
        [SerializeField] protected float groundCheckRadius;

        protected bool IsGrounded()
        {
            return Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);
        }
    }

}
