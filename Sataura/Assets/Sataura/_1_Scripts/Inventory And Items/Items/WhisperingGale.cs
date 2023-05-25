using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Sataura
{
    public class WhisperingGale : Item
    {      
        [Header("Runtime References")]
        [SerializeField] private WhisperingGaleData _whisperingGaleData;

        // Cached variables
        private GameObject _projectilePrefab;
        private BlackHoleProjectile _blackHoleProjectilePrefab;

        public override void OnNetworkSpawn()
        {
            _projectilePrefab = GameDataManager.Instance.GetProjectilePrefab(ProjectileType.BookOfWindProjectile);
             GameDataManager.Instance.GetProjectilePrefab(ProjectileType.BlackHole).TryGetComponent<BlackHoleProjectile>(out _blackHoleProjectilePrefab);

            _whisperingGaleData = ((WhisperingGaleData)ItemData);

            if (IsServer)
            {
                int itemID = GameDataManager.Instance.GetItemID(_whisperingGaleData);
                SetDataServerRpc(itemID, 1);
            }
        }

        public override bool Use(Player player, Vector2 nearestEnemyPosition)
        {
            switch(_whisperingGaleData.useType)
            {
                case 1:
                    FireSingleProjectile(player, nearestEnemyPosition);
                    break;
                case 2:
                    FireDoubleProjectiles(player, nearestEnemyPosition);
                    break;
                case 3:
                    Evo(player, nearestEnemyPosition);
                    break;
                default: 
                    break;
            }
            
          
            return true;
        }

        private void FireSingleProjectile(Player player, Vector2 nearestEnemyPosition)
        {
            Vector2 direction = nearestEnemyPosition - (Vector2)transform.position;
            var _projectileObject = Instantiate(_projectilePrefab, transform.position, Quaternion.identity);
            _projectileObject.transform.localScale *= _whisperingGaleData.size * player.characterData._currentArea;
            var _bookOfWindProjectile = _projectileObject.GetComponent<BookOfWindProjectile>();
            _bookOfWindProjectile.SetData(_whisperingGaleData, direction.normalized);
            _bookOfWindProjectile._networkObject.Spawn();
        }

        private void FireDoubleProjectiles(Player player, Vector2 nearestEnemyPosition)
        {
            FireSingleProjectile(player, nearestEnemyPosition);

            StartCoroutine(WaitAfter(0.2f, () =>
            {
                FireSingleProjectile(player, player.playerUseItem.DetectNearestEnemyPosition());
            }));
        }

        private void Evo(Player player, Vector2 nearestEnemyPosition)
        {
            FireDoubleProjectiles(player, nearestEnemyPosition);

            if(Random.Range(0f,1f) > 0.5f)
            {
                var blackHoleSpawnPosition = (Vector2)player.transform.position + Random.insideUnitCircle * Random.Range(15f, 30f);
                var blackHoleObject = Instantiate(_blackHoleProjectilePrefab, blackHoleSpawnPosition, Quaternion.identity);
                blackHoleObject.SetData(_whisperingGaleData);
                blackHoleObject._networkObject.Spawn();
            }       
        }


        private IEnumerator WaitAfter(float time, System.Action callback)
        {
            yield return new WaitForSeconds(time);
            callback?.Invoke();
        }
    }
}   