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

        public override bool Use(Player player, Vector2 nearestEnemyPosition)
        {           
            switch(_swordData.useType)
            {
                case 1:
                    SingleProjectile(player, nearestEnemyPosition);
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


       


        private void SingleProjectile(Player player, Vector2 nearestEnemyPosition, bool upSide = true)
        {
            var swordOjbect = Instantiate(woodenSwordProjectilePrefab, transform.position, Quaternion.identity);
            var passiveProjectile = swordOjbect.GetComponent<NetworkProjectile>();
            passiveProjectile._networkObject.Spawn();

            swordOjbect.GetComponent<WoodenSwordProejctile>().SetUp(player, _swordData, nearestEnemyPosition, upSide);

            // Sound
            SoundManager.Instance.PlaySound(SoundType.Sword, playRandom: true);
            // -----
        }

        private void MultipleProjectile(Player player, Vector2 nearestEnemyPosition)
        {
            SingleProjectile(player, nearestEnemyPosition, true);
            StartCoroutine(WaitFor(0.3f, () =>
            {
                SingleProjectile(player, nearestEnemyPosition, false);
            }));

        }

        private void Evo(Player player, Vector2 nearestEnemyPosition)
        {
            var evoSwordOjbect = Instantiate(evoWoodenSwordProjectilePrefab, transform.position, Quaternion.identity);
            var passiveProjectile = evoSwordOjbect.GetComponent<NetworkProjectile>();
            passiveProjectile._networkObject.Spawn();

            evoSwordOjbect.GetComponent<EvoWoodenSwordProejctile>().SetUp(player, _swordData, nearestEnemyPosition);
        }

        private IEnumerator WaitFor(float time, Action callback)
        {
            yield return new WaitForSeconds(time);
            callback?.Invoke();
        }
    }
}