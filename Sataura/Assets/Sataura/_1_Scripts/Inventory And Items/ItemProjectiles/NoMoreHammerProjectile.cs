using UnityEngine;
using System.Collections;

namespace Sataura
{
    public class NoMoreHammerProjectile : NetworkProjectile, ICanCauseDamage
    {
        [Header("Runtime References")]
        [SerializeField] private HammerData _hammerData = null;

        public void SetData(HammerData data)
        {
            _hammerData = data;
        }

        private IEnumerator Fall()
        {
            yield return new WaitUntil(() => _hammerData != null);
            rb.AddForce(Vector2.down * _hammerData.releaseForce, ForceMode2D.Impulse);
        }

        public override void OnNetworkSpawn()
        {
            StartCoroutine(Despawn());
            StartCoroutine(Fall());
        }



        private IEnumerator Despawn()
        {
            yield return new WaitUntil(() => _hammerData != null);
            yield return new WaitForSeconds(5.0f);
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

