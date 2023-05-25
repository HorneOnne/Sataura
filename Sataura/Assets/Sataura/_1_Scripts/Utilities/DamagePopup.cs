using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace Sataura
{
    public class DamagePopup : NetworkBehaviour
    {
        private const float DISAPPEAR_TIMER_MAX = 1f;
        private float disappearTimer;
        private Color textColor;
        private Vector3 moveVector;
        private TextMeshPro textMesh;

        [Header("Runtime References")]
        [SerializeField] private NetworkObject networkObject;
        [SerializeField] private GameObject damagePopupPrefab;

   
        private void Awake()
        {
            textMesh = GetComponent<TextMeshPro>();
        }

        public override void OnNetworkSpawn()
        {
            networkObject = GetComponent<NetworkObject>();
            IngameInformationManager.Instance.currentDamagePopupCount++; 
        }

        public void SetUp(int damage, Color color, float size, Vector3 moveVector, Vector3 rotation)
        {
            this.textColor = color;
            textMesh.color = textColor;
            textMesh.fontSize = size;

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
            IngameInformationManager.Instance.currentDamagePopupCount--;
            if (networkObject.IsSpawned)
                networkObject.Despawn();
        }
    }
}