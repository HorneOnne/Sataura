using UnityEngine;
using System.Collections;
using Unity.Netcode;

namespace Sataura
{

    public class HammerProjectile : NetworkProjectile, ICanCauseDamage
    {
        [Header("Runtime References")]
        [SerializeField] private HammerData _hammerData = null;

        public void SetData(HammerData data)
        {
            _hammerData = data;
        }

        private IEnumerator ThrowUp()
        {
            yield return new WaitUntil(() => _hammerData != null);

            Vector2 throwUpVector = Vector2.up + new Vector2(Random.Range(-1f, 1f), 0f);
            rb.AddForce(throwUpVector.normalized * _hammerData.releaseForce, ForceMode2D.Impulse);
            rb.AddTorque(10f, ForceMode2D.Impulse);
        }

        public override void OnNetworkSpawn()
        {
            StartCoroutine(Despawn());
            StartCoroutine(ThrowUp());
        }



        private IEnumerator Despawn()
        {
            yield return new WaitUntil(() => _hammerData != null);
            yield return new WaitForSeconds(7.0f);
            if (_networkObject.IsSpawned)
                _networkObject.Despawn();
        }

        public int GetDamage()
        {
            return _hammerData.damage;
        }

        public float GetKnockback()
        {
            return 0.0f;
        }
    }
}

