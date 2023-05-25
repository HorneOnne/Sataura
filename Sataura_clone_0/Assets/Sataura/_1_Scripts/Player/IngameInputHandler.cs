using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

namespace Sataura
{
    public class IngameInputHandler : NetworkBehaviour
    {

        [Header("REFERENCES")]
        [SerializeField] private Player _player;
        private PlayerInGameSkills _playerInGameInventory;
        private PlayerMovement _playerMovement;

        // New input system
        private PlayerInputAction playerInputAction;
        private InputAction jump;

        /// <summary>
        /// The number of seconds left in the jump buffer.
        /// </summary>
        private float jumpBufferCount;

        /// <summary>
        /// The time left for the player to hang in the air after leaving the ground.
        /// </summary>
        private float hangCounter;


        public bool canJump;



        public Vector2 MovementInput { get; private set; }
        public Vector2 RotateWeaponInput { get; private set; }
        public bool PressUtilityKeyInput { get; private set; }
        public float JumpInput { get; private set; }



        public override void OnNetworkSpawn()
        {
            playerInputAction = new PlayerInputAction();
            playerInputAction.Player.Enable();
            jump = playerInputAction.Player.Jump;
            jump.Enable();
            jump.performed += Jump;


            _playerMovement = _player.playerMovement;

            _playerInGameInventory = _player.playerIngameSkills;
        }

        private void Jump(InputAction.CallbackContext context)
        {
            jumpBufferCount = _player.characterData.characterMovementData.jumpBufferLength;
        }




        private void Update()
        {
            if (!IsOwner) return;
            if (_player.IsGameOver()) return;



            JumpInput = playerInputAction.Player.Jump.ReadValue<float>();
            MovementInput = playerInputAction.Player.Movement.ReadValue<Vector2>();
            RotateWeaponInput = playerInputAction.Player.RotateWeapon.ReadValue<Vector2>();

            if (_playerMovement.isGrounded || _playerMovement.isOnPlatform)
                hangCounter = _player.characterData.characterMovementData.hangTime;
            else
                hangCounter -= Time.deltaTime;



            // calculate Jump Buffer
            if (jumpBufferCount >= 0)
            {
                jumpBufferCount -= Time.deltaTime;
            }


            if (jumpBufferCount > 0 && hangCounter > 0)
            {
                canJump = true;
                jumpBufferCount = 0;
            }

        }
    }
}
