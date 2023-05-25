using System.Collections;
using UnityEngine;

namespace Sataura
{
    public class LightningStaff : Item
    {
        private GameObject _projectilePrefab;
        private GameObject _blackWandprojectilePrefab;
        [SerializeField] private LightningStaffData _lightningStaffData;


        public override void OnNetworkSpawn()
        {
            _projectilePrefab = GameDataManager.Instance.GetProjectilePrefab(ProjectileType.LightningStaffProjectile);
            _blackWandprojectilePrefab = GameDataManager.Instance.GetProjectilePrefab(ProjectileType.BlackWandProjectile);

            _lightningStaffData = (LightningStaffData)ItemData;
            

            if (IsServer)
            {
                int itemID = GameDataManager.Instance.GetItemID(_lightningStaffData);
                SetDataServerRpc(itemID, 1);
            }
        }


        public override bool Use(Player player, Vector2 nearestEnemy)
        {
            switch(_lightningStaffData.useType)
            {
                case 1:
                    SummonSingleLightning(player, nearestEnemy);
                    break;
                case 2:
                    SummonDoubleLightning(player, nearestEnemy);
                    break;
                case 3:
                    SummonTripleLightning(player, nearestEnemy);
                    break;
                case 4:
                    Evo(player, nearestEnemy);
                    break;
                default: 
                    break;
            }
                        
            return true;
        }   

        private void SummonSingleLightning(Player player, Vector2 nearestEnemy)
        {
            var _projectileObject = Instantiate(_projectilePrefab, transform.position, Quaternion.identity);
            var _lightningStaffProjectile = _projectileObject.GetComponent<LightningStaffProjectile>();
            _lightningStaffProjectile.SetData(_lightningStaffData, nearestEnemy, _lightningStaffData.size * player.characterData._currentArea);
            _lightningStaffProjectile._networkObject.Spawn();
        }

        private void SummonDoubleLightning(Player player, Vector2 nearestEnemy)
        {
            SummonSingleLightning(player, nearestEnemy);
            StartCoroutine(WaitAfter(0.2f, () =>
            {
                SummonSingleLightning(player, player.playerUseItem.DetectNearestEnemyPosition());
            }));
        }

        private void SummonTripleLightning(Player player, Vector2 nearestEnemy)
        {
            SummonSingleLightning(player, nearestEnemy);

            StartCoroutine(WaitAfter(0.2f, () =>
            {
                SummonSingleLightning(player, player.playerUseItem.DetectNearestEnemyPosition());
            }));

            StartCoroutine(WaitAfter(0.4f, () =>
            {
                SummonSingleLightning(player, player.playerUseItem.DetectNearestEnemyPosition());
            }));
        }


        private void SummonSingleEvoLightning(Player player, Vector2 nearestEnemy)
        {
            var _projectileObject = Instantiate(_blackWandprojectilePrefab, transform.position, Quaternion.identity);
            var _lightningStaffProjectile = _projectileObject.GetComponent<BlackWandProjectile>();
            _lightningStaffProjectile._networkObject.Spawn();
            _lightningStaffProjectile.SetData(_lightningStaffData, nearestEnemy, _lightningStaffData.size * player.characterData._currentArea);
           
        }
        private void Evo(Player player, Vector2 nearestEnemy)
        {
            SummonSingleEvoLightning(player, nearestEnemy);

            /*StartCoroutine(WaitAfter(0.1f, () =>
            {
                SummonSingleEvoLightning(player, player.playerUseItem.DetectNearestEnemyPosition());
            }));
            StartCoroutine(WaitAfter(0.2f, () =>
            {
                SummonSingleEvoLightning(player, player.playerUseItem.DetectNearestEnemyPosition());
            }));
            StartCoroutine(WaitAfter(0.3f, () =>
            {
                SummonSingleEvoLightning(player, player.playerUseItem.DetectNearestEnemyPosition());
            }));*/
        }

        private IEnumerator WaitAfter(float time, System.Action callback)
        {
            yield return new WaitForSeconds(time);
            callback?.Invoke();
        }
    }
}