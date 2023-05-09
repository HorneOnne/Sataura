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
            damagePopupPrefab = GameDataManager.Instance.GetItemPrefab("UIP_DamagePopup");
        }

        public void SetUp(int damage, Color color, float size, Vector3 moveVector, Vector3 rotation)
        {
            this.textColor = color;
            textMesh.color = textColor;
            textMesh.fontSize = size;
            //this.transform.rotation = Quaternion.Euler(rotation);

            textMesh.text = damage.ToString();

            disappearTimer = DISAPPEAR_TIMER_MAX;
            this.moveVector = moveVector * 30f;
            //moveVector = new Vector3(Random.Range(-1f, 1f), Random.Range(0.5f, 1f)) * 60f;
        }

        private void Update()
        {
            transform.position += moveVector * Time.deltaTime;
            moveVector -= moveVector * 8f * Time.deltaTime;

            if (disappearTimer > DISAPPEAR_TIMER_MAX * .5f)
            {
                // First half of the popup lifetime
                //float increaseScaleAmount = 1f;
                //transform.localScale += Vector3.one * increaseScaleAmount * Time.deltaTime;
            }
            else
            {
                // Second half of the popup lifetime
                //float decreaseScaleAmount = 1f;
                //transform.localScale -= Vector3.one * decreaseScaleAmount * Time.deltaTime;
            }

            disappearTimer -= Time.deltaTime;
            if (disappearTimer < 0)
            {
                // Start disappearing
                float disappearSpeed = 3f;
                textColor.a -= disappearSpeed * Time.deltaTime;
                textMesh.color = textColor;
                if (textColor.a < 0)
                {
                    //Destroy(gameObject);
                    ReturnToPool();
                }
            }
        }



        private void ReturnToPool()
        {
            ResetProperties();
            //DamagePopupSpawner.Instance.Pool.Release(this.gameObject);
            if (networkObject.IsSpawned)
                networkObject.Despawn(false);

            NetworkObjectPool.Singleton.ReturnNetworkObject(networkObject, damagePopupPrefab);
        }

        private void ResetProperties()
        {
            transform.localScale = Vector3.one;
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}