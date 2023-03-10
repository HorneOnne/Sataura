using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Sataura
{
    /// <summary>
    /// Handles player movement, jumping and flipping in both grounded and aerial states.
    /// </summary>
    public class PlayerMovement : NetworkBehaviour
    {
        [Header("REFERENCES")]
        [SerializeField] private Player player;
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] Transform groundCheckPoint;
        private PlayerData playerData;

        [SerializeField] private PlayerInputHandler playerInputHandler;
        


        #region Properties

        /// <summary>
        /// Gets the direction the player is facing.
        /// </summary>
        public sbyte FacingDirection { get; private set; }
        #endregion Properties   

        public Vector2 CurrentVelocity { get { return rb.velocity; } }

        public bool isGrounded;

        /*private void Start()
        {
            playerInputHandler = player.PlayerInputHandler;
            playerData = player.playerData;
            FacingDirection = 1;
        }*/

        public override void OnNetworkSpawn()
        {
            playerInputHandler = player.PlayerInputHandler;
            playerData = player.playerData;
            FacingDirection = 1;
        }

        private void FixedUpdate()
        {
            if (!IsOwner) return;

            // Movement 
            // =========================================================================
            if(player.handleMovement)
            {
                isGrounded = IsGrounded();

                HandleMovementOnGround(player.PlayerInputHandler.MovementInput);

                // Movement in AIR
                HandleMoveInAir(player.PlayerInputHandler.MovementInput, isGrounded);

                // Jump 
                // =========================================================================
                HandleJump(playerInputHandler.JumpInput == 1, isGrounded);

                // Pre-Calculate Physics
                HandleAddGravityMultiplier();


                //SetMaxVelocity();
                HandleSetMaxVelocity();

                FlipCharacterFace(playerInputHandler.MovementInput.x);
            }         
        }


        /// <summary>
        /// Checks if the player is on the ground.
        /// </summary>
        /// <returns>True if the player is grounded, false otherwise.</returns>
        public bool IsGrounded()
        {
            return Physics2D.OverlapCircle(groundCheckPoint.position, playerData.roundCheckRadius, playerData.groundLayer);
        }



        //[ServerRpc]
        private void HandleMovementOnGround(Vector2 movementInputVector)
        {
            if (movementInputVector.x != 0)
            {
                rb.velocity = new Vector2(movementInputVector.x * playerData.movementSpeed, rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
        }


        //[ServerRpc]
        private void HandleMoveInAir(Vector2 movementInputVector, bool isOnGrounded)
        {
            if (isOnGrounded == false && movementInputVector.x != 0)
            {
                if (movementInputVector.x == 0)
                {
                    rb.velocity = new Vector2(rb.velocity.x * playerData.airDragMultiplier, rb.velocity.y);
                }
                else
                {
                    Vector2 forceToAdd = new Vector2(playerData.movementForceInAir * movementInputVector.x, rb.velocity.y);
                    rb.velocity = forceToAdd;

                    if (Mathf.Abs(rb.velocity.x) > playerData.maxMovementSpeed)
                    {
                        rb.velocity = new Vector2(playerData.maxMovementSpeed * movementInputVector.x, rb.velocity.y);
                    }
                }
            }
        }



        //[ServerRpc]
        private void HandleJump(bool triggerJump, bool isOnGrounded)
        {
            if(isOnGrounded && triggerJump)
            {
                rb.velocity = new Vector2(rb.velocity.x, playerData.jumpForce);
            }
        }



        //[ServerRpc]
        private void HandleSetMaxVelocity()
        {
            if (rb.velocity.y > playerData.maxJumpVelocity)
                rb.velocity = new Vector2(rb.velocity.x, playerData.maxJumpVelocity);
            if (rb.velocity.y < playerData.maxFallVelocity)
                rb.velocity = new Vector2(rb.velocity.x, playerData.maxFallVelocity);
        }


        //[ServerRpc]
        private void HandleAddGravityMultiplier()
        {
            if (rb.velocity.y < 0)
                rb.velocity += Vector2.up * Physics2D.gravity * playerData.fallMultiplier;
            else if (rb.velocity.y > 0)
                rb.velocity += Vector2.up * Physics2D.gravity * playerData.lowMultiplier;
        }

        public void FlipCharacterFace(float XInput)
        {
            if (XInput != 0 && XInput != FacingDirection)
            {
                Flip();
            }

        }

        private void Flip()
        {
            FacingDirection *= -1;
        }
    }
}
