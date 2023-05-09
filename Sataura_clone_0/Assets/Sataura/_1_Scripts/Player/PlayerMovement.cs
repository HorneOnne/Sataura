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
        [SerializeField] public Transform groundCheckPoint;
        private CharacterMovementData characterMovementData;

        [SerializeField] private PlayerInputHandler playerInputHandler;


        [Header("Layer Properties")]
        public LayerMask groundLayer;
        public LayerMask platformLayer;
        public float groundCheckRadius;

        // Ground check
        public bool isGrounded;
        public bool isOnPlatform;


        #region Properties

        /// <summary>
        /// Gets the direction the player is facing.
        /// </summary>
        public sbyte FacingDirection { get; private set; }
         

        public Vector2 CurrentVelocity { get { return rb.velocity; } }
        public Rigidbody2D Rb2D { get { return rb; } }
        
        
        // Double jump Properties
        public int NumOfJumps { get; private set; } = 0;
        #endregion Properties  

        [Header("Character properties")]
        [SerializeField] private float _currentMovementForceInAir;


        public override void OnNetworkSpawn()
        {
            playerInputHandler = player.playerInputHandler;
            characterMovementData = player.characterData.characterMovementData;
            FacingDirection = 1;

            SetMovementSpeed();
            SetJumpForce();


            float additionMoveSpeed = player.characterData._currentMoveSpeed - player.characterData._defaultMoveSpeed;

            if (additionMoveSpeed > 0)
            {
                _currentMovementForceInAir = characterMovementData.movementForceInAir;
            }
            else
            {
                _currentMovementForceInAir = characterMovementData.movementForceInAir + (characterMovementData.movementForceInAir * additionMoveSpeed / 100);
            }     
        }






        public void SetMovementSpeed(BootData _bootData = null)
        {
            /*if(_bootData != null)
            {
                currentMovementSpeed = characterMovementData.movementSpeed + (characterMovementData.movementSpeed * _bootData.additionMoveSpeed / 100);
                currentMovementForceInAir = characterMovementData.movementForceInAir + (characterMovementData.movementForceInAir * _bootData.additionMoveSpeed / 100);
            }            
            else
            {
                currentMovementSpeed = characterMovementData.movementSpeed;
                currentMovementForceInAir = characterMovementData.movementForceInAir;
            }*/
                
        }

        public void SetJumpForce(BootData _bootData = null)
        {
            /*if (_bootData != null)
                currentJumpForce = characterMovementData.jumpForce + (characterMovementData.jumpForce * _bootData.additionJumpForce/100);
            else
                currentJumpForce = characterMovementData.jumpForce;*/
        }

        private void Update()
        {
            if (player.IsGameOver()) return;
            isGrounded = IsGrounded();
            isOnPlatform = IsOnPlatform();

            if(Rb2D.velocity.y <= 0.1 && isGrounded)
            {
                NumOfJumps = 0;
            }
        }

        private void FixedUpdate()
        {
            if (!IsOwner) return;

            // Movement 
            // =========================================================================
            HandleMovementOnGround(player.playerInputHandler.MovementInput);

            // Movement in AIR
            HandleMoveInAir(player.playerInputHandler.MovementInput, isGrounded);

            // Jump 
            // =========================================================================
            if (playerInputHandler.canJump)
            {
                HandleJump();
                playerInputHandler.canJump = false;
            }



            if (playerInputHandler.MovementInput.y < 0)
            {
                HandleOneWayPlatformEffector2D();
            }


            // Pre-Calculate Physics
            HandleAddGravityMultiplier();

            //SetMaxVelocity();
            HandleSetMaxVelocity();

            FlipCharacterFace(playerInputHandler.MovementInput.x);
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
                rb.velocity = new Vector2(movementInputVector.x * player.characterData._currentMoveSpeed , rb.velocity.y);
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
                    rb.velocity = new Vector2(rb.velocity.x * characterMovementData.airDragMultiplier, rb.velocity.y);
                }
                else
                {
                    Vector2 forceToAdd = new Vector2((_currentMovementForceInAir + player.characterData._currentMoveSpeed) * movementInputVector.x, rb.velocity.y);
                    rb.velocity = forceToAdd;

                    if (Mathf.Abs(rb.velocity.x) > characterMovementData.maxMovementSpeed)
                    {
                        rb.velocity = new Vector2(characterMovementData.maxMovementSpeed * movementInputVector.x, rb.velocity.y);
                    }
                }
            }
        }


        public void HandleJump()
        {
            NumOfJumps++;
            rb.velocity = new Vector2(rb.velocity.x, player.characterData._currentJumpForce);
        }


        private void HandleSetMaxVelocity()
        {
            if (rb.velocity.y > characterMovementData.maxJumpVelocity)
                rb.velocity = new Vector2(rb.velocity.x, characterMovementData.maxJumpVelocity);
            if (rb.velocity.y < characterMovementData.maxFallVelocity)
                rb.velocity = new Vector2(rb.velocity.x, characterMovementData.maxFallVelocity);
        }


        private void HandleAddGravityMultiplier()
        {
            if (rb.velocity.y < 0)
                rb.velocity += Vector2.up * Physics2D.gravity * characterMovementData.fallMultiplier;
            else if (rb.velocity.y > 0)
                rb.velocity += Vector2.up * Physics2D.gravity * characterMovementData.lowMultiplier;
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
