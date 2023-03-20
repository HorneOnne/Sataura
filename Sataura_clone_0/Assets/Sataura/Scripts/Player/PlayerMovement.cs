using System.Collections;
using Unity.Netcode;
using UnityEditor;
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
        private CharacterData playerData;

        [SerializeField] private PlayerInputHandler playerInputHandler;


        [Header("Layer Properties")]
        public LayerMask groundLayer;
        public LayerMask platformLayer;
        public float roundCheckRadius;



        #region Properties

        /// <summary>
        /// Gets the direction the player is facing.
        /// </summary>
        public sbyte FacingDirection { get; private set; }
        #endregion Properties   

        public Vector2 CurrentVelocity { get { return rb.velocity; } }
        public bool isGrounded;
        public bool isOnPlatform;



        private float platformRationalOffsetWaitTime;

        public override void OnNetworkSpawn()
        {
            playerInputHandler = player.PlayerInputHandler;
            playerData = player.characterData;
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
                isOnPlatform = IsOnPlatform();


                HandleMovementOnGround(player.PlayerInputHandler.MovementInput);

                // Movement in AIR
                HandleMoveInAir(player.PlayerInputHandler.MovementInput, isGrounded);

                // Jump 
                // =========================================================================
                if(playerInputHandler.canJump)
                {
                    HandleJump();
                    playerInputHandler.canJump = false;
                }


                if(playerInputHandler.MovementInput.y < 0)
                {
                    HandleOneWayPlatformEffector2D();
                }
         

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
            return Physics2D.OverlapCircle(groundCheckPoint.position, roundCheckRadius, groundLayer);
        }

        public bool IsOnPlatform()
        {
            return Physics2D.OverlapCircle(groundCheckPoint.position, roundCheckRadius, platformLayer);
        }
        IEnumerator ResetOneWayEffector2D(PlatformEffector2D platformEffector2D)
        {
            yield return new WaitForSeconds(0.2f);
            platformEffector2D.rotationalOffset = 0;
        }


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

        private void HandleOneWayPlatformEffector2D()
        {
            var col = Physics2D.OverlapCircle(groundCheckPoint.position, roundCheckRadius, platformLayer);
            if (col != null)
            {
                PlatformEffector2D platformEffector2D = col.GetComponent<PlatformEffector2D>();
                if (platformEffector2D != null)
                {
                    platformEffector2D.rotationalOffset = 180;
                    StartCoroutine(ResetOneWayEffector2D(platformEffector2D));
                }
            }
        }


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


        public void HandleJump()
        {
            rb.velocity = new Vector2(rb.velocity.x, playerData.jumpForce);
        }


        private void HandleSetMaxVelocity()
        {
            if (rb.velocity.y > playerData.maxJumpVelocity)
                rb.velocity = new Vector2(rb.velocity.x, playerData.maxJumpVelocity);
            if (rb.velocity.y < playerData.maxFallVelocity)
                rb.velocity = new Vector2(rb.velocity.x, playerData.maxFallVelocity);
        }


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
