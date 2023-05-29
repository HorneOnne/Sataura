using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Sataura
{

    public class Hammer : Item, ICanCauseDamage
    {
        [Header("Runtime References")]
        [SerializeField] private HammerData _hammerData;

        // Cached
        private HammerProjectile hammerProjectilePrefab;
        private NoMoreHammerProjectile noMoreHammerProjectilePrefab;
        private List<Vector2> spawnPosition = new List<Vector2>()
        {
            new Vector2(10,0),
            new Vector2(-10,0),
            new Vector2(-20,0),
            new Vector2(-20,0),
        };



        public override void OnNetworkSpawn()
        {
            GameDataManager.Instance.GetProjectilePrefab(ProjectileType.HammerProjectile).TryGetComponent<HammerProjectile>(out hammerProjectilePrefab);
            GameDataManager.Instance.GetProjectilePrefab(ProjectileType.NoMoreHammerProjectile).TryGetComponent<NoMoreHammerProjectile>(out noMoreHammerProjectilePrefab);
           
            _hammerData = ((HammerData)ItemData);

            if (IsServer)
            {
                int itemID = GameDataManager.Instance.GetItemID(_hammerData);
                SetDataServerRpc(itemID, 1);
            }
        }

        public override bool Use(IngamePlayer player, Vector2 nearestEnemyPosition)
        {
            switch (_hammerData.useType)
            {
                case 1:
                    ThrowSingleHammer(player);
                    break;
                case 2:
                    ThrowDoubleHammer(player);
                    break;
                case 3:
                    ThrowTripleHammer(player);
                    break;
                case 4:
                    Evo(player);
                    break;
                default:
                    break;
            }



            return true;
        }

        private void ThrowSingleHammer(IngamePlayer player)
        {
            var _projectileObject = Instantiate(hammerProjectilePrefab, transform.position, Quaternion.identity);
            _projectileObject.transform.localScale *= _hammerData.size * player.characterData._currentArea;
            _projectileObject.SetData(_hammerData);
            _projectileObject._networkObject.Spawn();
        }

        private void ThrowDoubleHammer(IngamePlayer player)
        {
            ThrowSingleHammer(player);

            StartCoroutine(WaitAfter(0.2f, () =>
            {
                ThrowSingleHammer(player);
            }));
        }
        private void ThrowTripleHammer(IngamePlayer player)
        {
            ThrowSingleHammer(player);

            StartCoroutine(WaitAfter(0.2f, () =>
            {
                ThrowSingleHammer(player);
            }));

            StartCoroutine(WaitAfter(0.4f, () =>
            {
                ThrowSingleHammer(player);
            }));
        }


        private void ThrowSingleNoMoreHammer(IngamePlayer player, Vector2 spawnPosition)
        {
            var _projectileObject = Instantiate(noMoreHammerProjectilePrefab, spawnPosition, Quaternion.identity);
            _projectileObject.transform.localScale *= _hammerData.size * player.characterData._currentArea;
            _projectileObject.SetData(_hammerData);
            _projectileObject._networkObject.Spawn();
        }
        private void Evo(IngamePlayer player)
        {
            Vector2 baseSpawnPosition = (Vector2)player.transform.position + new Vector2(0, 50);
            ThrowSingleNoMoreHammer(player, baseSpawnPosition);
            for (int i = 0; i < spawnPosition.Count; i++)
            {
                ThrowSingleNoMoreHammer(player, baseSpawnPosition + spawnPosition[i]);
            }                   
        }


        private IEnumerator WaitAfter(float time, System.Action callback)
        {
            yield return new WaitForSeconds(time);
            callback?.Invoke();
        }

        public int GetDamage()
        {
            return ((HammerData)ItemData).damage;
        }

        public float GetKnockback()
        {
            return 0.0f;
        }
    }
}
