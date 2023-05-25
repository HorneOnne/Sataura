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
        private IngameInputHandler _ingameInputHandler;
        // Model & anim
        [SerializeField] private Transform _model;
        [SerializeField] private Animator _anim;

        [Header("Character properties")]
        private float _currentMovementForceInAir;

        [Header("Layer Properties")]
        public LayerMask groundLayer;
        public LayerMask platformLayer;
        public float groundCheckRadius;

        // Ground check
        public bool isGrounded;
        public bool isOnPlatform;


        // Others Skills
        public bool isDashing = false;


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

       

        public override void OnNetworkSpawn()
        {
            _ingameInputHandler = player.ingameInputHandler;
            characterMovementData = player.characterData.characterMovementData;
            FacingDirection = 1;


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



        private void Update()
        {
            if (player.IsGameOver()) return;
            isGrounded = IsGrounded();
            isOnPlatform = IsOnPlatform();

            if(Rb2D.velocity.y <= 0.1 && isGrounded)
            {
                NumOfJumps = 0;
            }


            // Set Movement animation
            _anim.SetBool("isGround", isGrounded);


            if (player.ingameInputHandler.MovementInput.x != 0)
            {
                _anim.SetBool("isMove", true);
                _anim.SetFloat("xMoveSpeed", player.characterData._currentMoveSpeed);
            }
            else
            {
                _anim.SetBool("isMove", false);
            }


            /*if(_ingameInputHandler.canJump)
            {
                _anim.SetTrigger("isJump");
                _anim.SetFloat("yVelocity", rb.velocity.y);            
            }
            else
            {
                _anim.ResetTrigger("isJump");
            }*/
            // ----------------------
        }

        private void FixedUpdate()
        {
            if (!IsOwner) return;
            if (isDashing) return;
            
            // Movement 
            // =========================================================================
            HandleMovementOnGround(player.ingameInputHandler.MovementInput);

            // Movement in AIR
            HandleMoveInAir(player.ingameInputHandler.MovementInput, isGrounded);

            // Jump 
            // =========================================================================
            if (_ingameInputHandler.canJump)
            {
                HandleJump();
                _ingameInputHandler.canJump = false;
            }



            if (_ingameInputHandler.MovementInput.y < 0)
            {
                HandleOneWayPlatformEffector2D();
            }


            // Pre-Calculate Physics
            HandleAddGravityMultiplier();

            //SetMaxVelocity();
            HandleSetMaxVelocity();
            //LimitVelocity()

            FlipCharacterFace(_ingameInputHandler.MovementInput.x);
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

                //_anim.SetFloat("xMovement", player.characterData._currentMoveSpeed);
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
            if ((XInput == 1 || XInput == -1) && XInput != FacingDirection)
            {
                Flip();
            }

        }

        private void Flip()
        {          
            FacingDirection *= -1;
            _model.localScale = new Vector3(Mathf.Abs(_model.localScale.x) * FacingDirection, _model.localScale.y, _model.localScale.z);
        }

        public void LimitVelocity(float maxVelocity)
        {   
            if(rb.velocity.magnitude > maxVelocity)
            {
                rb.velocity = rb.velocity.normalized * maxVelocity;
            }
        }
    }
}
