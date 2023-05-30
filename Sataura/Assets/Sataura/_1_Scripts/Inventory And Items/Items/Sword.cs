using Unity.Netcode;
using UnityEngine;
using System.Collections;
using System;

namespace Sataura
{
    public class Sword : Item
    {
        [SerializeField] private SwordData _swordData;
        private GameObject woodenSwordProjectilePrefab;
        private GameObject evoWoodenSwordProjectilePrefab;

        public override void OnNetworkSpawn()
        {
            woodenSwordProjectilePrefab = GameDataManager.Instance.GetProjectilePrefab(ProjectileType.WoodenSwordProjectile);
            evoWoodenSwordProjectilePrefab = GameDataManager.Instance.GetProjectilePrefab(ProjectileType.EvoWoodenSwordProjectile);

            _swordData = ((SwordData)ItemData);

            if(IsServer)
            {
                int itemID = GameDataManager.Instance.GetItemID(_swordData);
                SetDataServerRpc(itemID, 1);       
            }
        }

        public override bool Use(IngamePlayer player, Vector2 nearestEnemyPosition)
        {           
            switch(_swordData.useType)
            {
                case 1:
                    SingleProjectile(player, nearestEnemyPosition, true);
                    break;
                case 2:
                    MultipleProjectile(player, nearestEnemyPosition);
                    break;
                case 3:
                    Evo(player, nearestEnemyPosition);
                    break;
                default:
                    Debug.LogWarning($"Not found useType {_swordData.useType} in SwordData.");
                    break;
            }

            return true;
        }


       


        private void SingleProjectile(IngamePlayer player, Vector2 nearestEnemyPosition, bool upSide)
        {
            var swordOjbect = Instantiate(woodenSwordProjectilePrefab, transform.position, Quaternion.identity);
            var woodenSwordProjectile = swordOjbect.GetComponent<WoodenSwordProjectile>();

            woodenSwordProjectile.SetUpside(upSide);
            woodenSwordProjectile.Fire(player, nearestEnemyPosition, _swordData);

            // Sound
            SoundManager.Instance.PlaySound(SoundType.Sword, playRandom: true);
            // -----
        }

        private void MultipleProjectile(IngamePlayer player, Vector2 nearestEnemyPosition)
        {
            SingleProjectile(player, nearestEnemyPosition, true);
            StartCoroutine(WaitFor(0.3f, () =>
            {
                SingleProjectile(player, nearestEnemyPosition, false);
            }));

        }

        private void Evo(IngamePlayer player, Vector2 nearestEnemyPosition)
        {
            var evoSwordOjbect = Instantiate(evoWoodenSwordProjectilePrefab, transform.position, Quaternion.identity);
            var passiveProjectile = evoSwordOjbect.GetComponent<NetworkProjectile>();
            passiveProjectile.NetworkSpawn();

            evoSwordOjbect.GetComponent<EvoWoodenSwordProejctile>().SetUp(player, _swordData, nearestEnemyPosition);
        }

        private IEnumerator WaitFor(float time, Action callback)
        {
            yield return new WaitForSeconds(time);
            callback?.Invoke();
        }
    }
}