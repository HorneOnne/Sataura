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
        private CharacterData characterData;

        [SerializeField] private PlayerInputHandler playerInputHandler;


        [Header("Layer Properties")]
        public LayerMask groundLayer;
        public LayerMask platformLayer;
        public float groundCheckRadius;



        #region Properties

        /// <summary>
        /// Gets the direction the player is facing.
        /// </summary>
        public sbyte FacingDirection { get; private set; }
        #endregion Properties   

        public Vector2 CurrentVelocity { get { return rb.velocity; } }
        public Rigidbody2D Rb2D { get { return rb; } }

        public bool isGrounded;
        public bool isOnPlatform;



        private void OnEnable()
        {
   
        }

        private void OnDisable()
        {
            
        }


        public override void OnNetworkSpawn()
        {
            playerInputHandler = player.PlayerInputHandler;
            characterData = player.characterData;
            FacingDirection = 1;

            SetMovementSpeed();
            SetJumpForce();
        }


        [Header("Character properties")]
        [SerializeField] private float currentMovementSpeed;
        [SerializeField] private float currentMovementForceInAir;
        [SerializeField] private float currentJumpForce;


        public void SetMovementSpeed(BootData _bootData = null)
        {
            if(_bootData != null)
            {
                currentMovementSpeed = characterData.movementSpeed + (characterData.movementSpeed * _bootData.hastePercent / 100);
                currentMovementForceInAir = characterData.movementForceInAir + (characterData.movementForceInAir * _bootData.hastePercent / 100);
            }            
            else
            {
                currentMovementSpeed = characterData.movementSpeed;
                currentMovementForceInAir = characterData.movementForceInAir;
            }
                
        }

        public void SetJumpForce(BootData _bootData = null)
        {
            if (_bootData != null)
                currentJumpForce = characterData.jumpForce + (characterData.jumpForce * _bootData.leapPercent/100);
            else
                currentJumpForce = characterData.jumpForce;
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
            return Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);
        }

        public bool IsOnPlatform()
        {
            return Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, platformLayer);
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
                rb.velocity = new Vector2(movementInputVector.x * currentMovementSpeed, rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
        }

        private void HandleOneWayPlatformEffector2D()
        {
            var col = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, platformLayer);
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
                    rb.velocity = new Vector2(rb.velocity.x * characterData.airDragMultiplier, rb.velocity.y);
                }
                else
                {
                    Vector2 forceToAdd = new Vector2(currentMovementForceInAir * movementInputVector.x, rb.velocity.y);
                    rb.velocity = forceToAdd;

                    if (Mathf.Abs(rb.velocity.x) > characterData.maxMovementSpeed)
                    {
                        rb.velocity = new Vector2(characterData.maxMovementSpeed * movementInputVector.x, rb.velocity.y);
                    }
                }
            }
        }


        public void HandleJump()
        {
            rb.velocity = new Vector2(rb.velocity.x, currentJumpForce);
        }


        private void HandleSetMaxVelocity()
        {
            if (rb.velocity.y > characterData.maxJumpVelocity)
                rb.velocity = new Vector2(rb.velocity.x, characterData.maxJumpVelocity);
            if (rb.velocity.y < characterData.maxFallVelocity)
                rb.velocity = new Vector2(rb.velocity.x, characterData.maxFallVelocity);
        }


        private void HandleAddGravityMultiplier()
        {
            if (rb.velocity.y < 0)
                rb.velocity += Vector2.up * Physics2D.gravity * characterData.fallMultiplier;
            else if (rb.velocity.y > 0)
                rb.velocity += Vector2.up * Physics2D.gravity * characterData.lowMultiplier;
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
