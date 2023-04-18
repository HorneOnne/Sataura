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

        #region Properties
        public int ExpValue { get { return expValue;} }

        #endregion


        public override void OnNetworkSpawn()
        {
            rb2D = GetComponent<Rigidbody2D>();
            networkObject = GetComponent<NetworkObject>();

            //StartCoroutine(KinematicPerformance());
        }


        private IEnumerator KinematicPerformance()
        {
            yield return new WaitForSeconds(5.0f);
            rb2D.isKinematic = true;
        }

        bool canCollect = false;
        Transform player;
        private void FixedUpdate()
        {
            if (canCollect == false) return;
  

            var dir = player.position - transform.position;
            float distance = dir.magnitude;
            //float distance = Vector2.Distance(player.transform.position, transform.position);

            if (distance < 2.0f)
            {
                IngameInformationManager.Instance.GainExperience(expValue);
                networkObject.Despawn();
            }
            else
            {
                rb2D.velocity = dir.normalized * 20f;
            }
        }

        
        public void Collect(Player player)
        {
            //Debug.Log("Collect");
            canCollect = true;
            this.player = player.transform;
            rb2D.isKinematic = true;
        }
    }
}

