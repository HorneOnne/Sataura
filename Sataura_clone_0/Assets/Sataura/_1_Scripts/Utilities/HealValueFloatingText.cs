using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace Sataura
{
    public class HealValueFloatingText : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private NetworkObject networkObject;
        [SerializeField] private TextMeshPro textMesh;


        private const float DISAPPEAR_TIMER_MAX = 1f;
        private float disappearTimer;
        private Color textColor;
        private Vector3 moveVector;


        #region Properties
        public NetworkObject NetworkObj { get { return networkObject; } }    
        #endregion

        public void SetUp(int damage, Vector3 moveVector)
        {
            textMesh.text = damage.ToString();

            disappearTimer = DISAPPEAR_TIMER_MAX;
            this.moveVector = moveVector * 30f;
        }

        private void Update()
        {
            transform.position += moveVector * Time.deltaTime;
            moveVector -= moveVector * 8f * Time.deltaTime;


            disappearTimer -= Time.deltaTime;
            if (disappearTimer < 0)
            {
                // Start disappearing
                float disappearSpeed = 3f;
                textColor.a -= disappearSpeed * Time.deltaTime;
                textMesh.color = textColor;
                if (textColor.a < 0)
                {
                    Despawn();
                }
            }
        }


        private void Despawn()
        {
            if (networkObject.IsSpawned)
                networkObject.Despawn();
        }
    }
}