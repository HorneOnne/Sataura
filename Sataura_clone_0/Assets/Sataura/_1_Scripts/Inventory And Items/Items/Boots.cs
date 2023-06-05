using System.Collections;
using UnityEngine;

namespace Sataura
{
    public class Boots : Item 
    {
        [Header("Runtime References")]
        private IngamePlayer _player;
        [SerializeField] private BootData _bootsData;
        [SerializeField] private Rigidbody2D _rb2D;


        // Cached
        private float dashTimeThreshold = 0.2f; // The time window to detect double taps
        private float lastTapTimeDKey;
        private float lastTapTimeAKey;
        private bool canDash = true;

        public override void OnNetworkSpawn()
        {
            _bootsData = ((BootData)ItemData);
            
            if (IsServer)
            {
                int itemID = GameDataManager.Instance.GetItemID(_bootsData);
                SetDataServerRpc(itemID, 1);
            }
        }

        private void Update()
        {
            if (_bootsData == null) return;
            if (canDash == false) return;

            if (Input.GetKeyDown(KeyCode.D))
            {
                if (Time.time - lastTapTimeDKey <= dashTimeThreshold)
                {
                    // Perform dash
                    Dash();
                    lastTapTimeDKey = -dashTimeThreshold; // Reset the last tap time

                    canDash = false;
                    Invoke(nameof(EnableDash), _bootsData.dashCooldown);
                }
                else
                {
                    lastTapTimeDKey = Time.time; // Record the time of the first tap
                }
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                if (Time.time - lastTapTimeAKey <= dashTimeThreshold)
                {
                    // Perform dash
                    Dash();
                    lastTapTimeAKey = -dashTimeThreshold; // Reset the last tap time

                    canDash = false;
                    Invoke(nameof(EnableDash), _bootsData.dashCooldown);
                }
                else
                {
                    lastTapTimeAKey = Time.time; // Record the time of the first tap
                }
            }
        }


        public void SetOwner(IngamePlayer ingamePlayer)
        {
            _player = ingamePlayer;
            _rb2D = _player.playerMovement.Rb2D;
        }

        private void Dash()
        {
            _player.playerMovement.isDashing = true;

            // Play ghost effect
            _player.ghostEffect.isGhosting = true;

            // Determine the direction based on the key pressed
            float dashDirection = Input.GetKeyDown(KeyCode.D) ? 1f : -1f;

            // Apply force for dashing
            _rb2D.AddForce(new Vector2(_bootsData.dashForce * dashDirection, 5f), ForceMode2D.Impulse);

            // Stop dash
            StartCoroutine(StopDash());
            StartCoroutine(StopGhost());
        }

        private IEnumerator StopDash()
        {
            yield return new WaitForSeconds(_bootsData.dashTime);
            _player.playerMovement.isDashing = false;
        }

        private IEnumerator StopGhost()
        {
            yield return new WaitForSeconds(_bootsData.dashTime - (_bootsData.dashTime * 10 / 100f));
            _player.ghostEffect.isGhosting = false;
        }

        private void EnableDash()
        {
            canDash = true;
        }
    }
}