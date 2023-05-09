using Unity.Netcode;
using UnityEngine;

namespace Sataura
{
    public class EnemyBoss : NetworkBehaviour
    {
        [SerializeField] protected Rigidbody2D rb2D;
        [SerializeField] protected BoxCollider2D boxCollider2D;


        [SerializeField] private Transform playerTranform;
        [SerializeField] protected NetworkVariable<int> currentHealth = new NetworkVariable<int>(0);

        public void SetFollowTarget(Transform target)
        {
            playerTranform = target;
        }


        public override void OnNetworkSpawn()
        {
            SetFollowTarget(GameDataManager.Instance.singleModePlayer.transform);
        }



        float timeElapse = 0.0f;
        protected virtual void FixedUpdate()
        {
            if (!IsServer) return;

            if (Time.time - timeElapse >= 0.035f)
            {
                timeElapse = Time.time;
                MoveAI(playerTranform.position);
            }
        }

        public void MoveAI(Vector2 target)
        {
            Vector2 direction = target - (Vector2)transform.position;
            direction.Normalize();
            rb2D.MovePosition((Vector2)transform.position + direction * 10 * Time.fixedDeltaTime);
        }
    }

}

