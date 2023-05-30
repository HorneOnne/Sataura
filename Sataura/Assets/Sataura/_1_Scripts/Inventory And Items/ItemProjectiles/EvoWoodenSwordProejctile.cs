using UnityEngine;

namespace Sataura
{
    public class EvoWoodenSwordProejctile : NetworkProjectile
    {
        [SerializeField] private Animator _anim;
        private SwordData _swordData;
        [SerializeField] private LayerMask _enemyProjectileLayer;
        [SerializeField] private LayerMask _enemyLayer;
        private IngamePlayer _player;


        // Healing
        private bool _canHealth = true;

        public void SetUp(IngamePlayer player, SwordData swordData, Vector2 nearestEnemyPosition)
        {
            this._swordData = swordData;
            this._player = player;
            transform.localScale *= swordData.size * player.characterData._currentArea;
        }


        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (_enemyProjectileLayer == (_enemyProjectileLayer | (1 << collision.gameObject.layer)))
            {
                // Object on the target layer entered the trigger
                Destroy(collision.gameObject);
            }

            if(_canHealth)
            {
                if (_enemyLayer == (_enemyLayer | (1 << collision.gameObject.layer)))
                {
                    // Object on the target layer entered the trigger
                    _player.playerCombat.Healing(7);
                    _player.playerCombat.UpdateHealthBarUI();
                    _canHealth = false;

                    // UI
                    var floatingTextMoveDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(0.5f, 1f));
                    var healFloatingTextPrefab = GameDataManager.Instance.healValueFloatingPrefab;
                    var healFloatingTextObject = Instantiate(healFloatingTextPrefab, _player.transform.position + new Vector3(0,2,0), Quaternion.identity);
                    healFloatingTextObject.NetworkObj.Spawn();
                    healFloatingTextObject.SetUp(7, floatingTextMoveDirection);
                    // ==
                }
            }        
        }


        public int GetDamage()
        {
            return _swordData.damage;
        }

       

        public float GetKnockback()
        {
            return _swordData.knockback;
        }


        public void DespawnNetworkObject()
        {
            if (base.networkObject.IsSpawned)
            {
                base.networkObject.Despawn();
            }

        }
    }
}

