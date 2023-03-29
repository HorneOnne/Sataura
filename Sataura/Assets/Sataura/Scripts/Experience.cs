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



        public void Collect(Player player)
        {
            IngameInformationManager.Instance.GainExperience(expValue);
            networkObject.Despawn();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if(collision.gameObject.CompareTag("Ground"))
            {
                rb2D.isKinematic = true;
            }
        }
    }
}

