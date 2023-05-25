using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Sataura
{
    public class Experience : NetworkBehaviour, ICollectible
    {
        [SerializeField] private int expValue;


        [Header("References")]
        private Rigidbody2D rb2D;
        private NetworkObject networkObject;

        [Header("Runtime References")]
        private Transform _playerTransform;

        [Header("Properties")]
        private float updateInterval = 5.0f; // The desired update interval in seconds
        private float updateTimer = 0f; // Timer to track the elapsed time

        #region Properties
        public int ExpValue { get { return expValue; } }

        #endregion


        public override void OnNetworkSpawn()
        {
            rb2D = GetComponent<Rigidbody2D>();
            networkObject = GetComponent<NetworkObject>();
            _playerTransform = GameDataManager.Instance.singleModePlayer.transform;

            Invoke(nameof(Despawn), 30f);

            IngameInformationManager.Instance.currentExpCount++;
        }


        private IEnumerator KinematicPerformance()
        {
            yield return new WaitForSeconds(5.0f);
            rb2D.isKinematic = true;
        }

        private void Despawn()
        {
            IngameInformationManager.Instance.currentExpCount--;
            networkObject.Despawn();
        }

        bool canCollect = false;
        Transform player;
        private void FixedUpdate()
        {
            if (canCollect == false)
            {
                updateTimer += Time.fixedDeltaTime; // Increase the timer by the time since the last FixedUpdate()
                                                    // Check if the desired update interval has passed
                if (updateTimer >= updateInterval)
                {
                    // Perform actions at the desired interval
                    if ((_playerTransform.position - transform.position).sqrMagnitude > 4900f)
                    {
                        Despawn();
                    }

                    // Reset the timer
                    updateTimer = 0f;
                }
                return;
            }



            var dir = player.position - transform.position;
            float distance = dir.magnitude;
            //float distance = Vector2.Distance(player.transform.position, transform.position);

            if (distance < 2.0f)
            {
                IngameInformationManager.Instance.GainExperience(expValue);
                Despawn();
            }
            else
            {
                rb2D.velocity = dir.normalized * 20f;
            }



        }


        public void Collect(Player player)
        {
            canCollect = true;
            this.player = player.transform;
            rb2D.isKinematic = true;
        }
    }
}

