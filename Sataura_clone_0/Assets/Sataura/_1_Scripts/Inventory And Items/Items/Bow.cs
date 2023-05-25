using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


namespace Sataura
{
    public class Bow : Item
    {
        [Header("References")]
        public List<Transform> shootingPoints;
        [SerializeField] private ArrowData arrowData;
        private GameObject normalArrowProjectilePrefab;
        private GameObject evoArrowProjectilePrefab;

        [Header("Runtime References")]
        [SerializeField] private BowData bowItemData;
        


        // Cached
        private ulong[] cachedClientIds;


        public override void OnNetworkSpawn()
        {
            bowItemData = (BowData)this.ItemData;
            normalArrowProjectilePrefab = GameDataManager.Instance.GetProjectilePrefab(ProjectileType.NormalArrow);
            evoArrowProjectilePrefab = GameDataManager.Instance.GetProjectilePrefab(ProjectileType.EvoArrow);

            if (IsServer)
            {
                int itemID = GameDataManager.Instance.GetItemID(bowItemData);
                SetDataServerRpc(itemID, 1);
            }
            
        }


        public override bool Use(Player player, Vector2 nearestEnemyPosition)
        {                    
            if (cachedClientIds == null || cachedClientIds.Length == 0)
            {
                InitializeCachedClientIds(player.GetComponent<NetworkObject>().OwnerClientId);            
            }



            switch (bowItemData.useType)
            {
                case 1:
                    ShootSingleArrow(player, nearestEnemyPosition);
                    break;
                case 2:
                    ShootDoubleArrows(player, nearestEnemyPosition);
                    break;
                case 3:
                    ShootTripleArrows(player, nearestEnemyPosition);
                    break;
                case 4:
                    EvoShooting(player, nearestEnemyPosition);
                    break;
                default:
                    break;
            }

            return true;
        }

        private IEnumerator WaitAfter(float time, System.Action callback)
        {
            yield return new WaitForSeconds(time);
            callback?.Invoke();
        }

        private void ShootSingleArrow(Player player, Vector2 nearestEnemyPosition)
        {
            var arrowObject = Instantiate(normalArrowProjectilePrefab, shootingPoints[0].position, transform.rotation);
            arrowObject.transform.localScale *= bowItemData.size * player.characterData._currentArea;
            var normalArrowProjectile = arrowObject.GetComponent<NormalArrowProjectile>();
            Utilities.RotateObjectTowardMouse2D(nearestEnemyPosition, normalArrowProjectile.transform, -45);
           
            int arrowID = GameDataManager.Instance.GetItemID(arrowData);
            normalArrowProjectile.SetDataServerRpc(arrowID, true);  
            normalArrowProjectile.Shoot(bowItemData, arrowData);

            if (normalArrowProjectile._networkObject.IsSpawned == false)
                normalArrowProjectile._networkObject.Spawn();

            SoundManager.Instance.PlaySound(SoundType.Bow, playRandom: true, 0.5f);
        }

        private void ShootDoubleArrows(Player player, Vector2 nearestEnemyPosition)
        {
            ShootSingleArrow(player, nearestEnemyPosition);

            StartCoroutine(WaitAfter(0.2f, () =>
            {
                ShootSingleArrow(player, player.playerUseItem.DetectNearestEnemyPosition());
            }));    
        }

        private void ShootTripleArrows(Player player, Vector2 nearestEnemyPosition)
        {
            ShootSingleArrow(player, nearestEnemyPosition);

            StartCoroutine(WaitAfter(0.2f, () =>
            {
                ShootSingleArrow(player, player.playerUseItem.DetectNearestEnemyPosition());
            }));

            StartCoroutine(WaitAfter(0.4f, () =>
            {
                ShootSingleArrow(player, player.playerUseItem.DetectNearestEnemyPosition());
            }));
        }



        private void ShootSingleEvoArrow(Player player, Vector2 nearestEnemyPosition)
        {
            var arrowObject = Instantiate(evoArrowProjectilePrefab, shootingPoints[0].position, transform.rotation);
            arrowObject.transform.localScale *= bowItemData.size * player.characterData._currentArea;
            var normalArrowProjectile = arrowObject.GetComponent<EvoArrowProjectile>();
            Utilities.RotateObjectTowardMouse2D(nearestEnemyPosition, normalArrowProjectile.transform, -45);

            int arrowID = GameDataManager.Instance.GetItemID(arrowData);
            normalArrowProjectile.SetDataServerRpc(arrowID, true);


            var nearestEnemy = player.playerUseItem.DetectNearestEnemy();
            if (nearestEnemy != null)
            {
                normalArrowProjectile.Shoot(bowItemData, arrowData, nearestEnemy);
            }
            else
            {
                normalArrowProjectile.Shoot(bowItemData, arrowData, nearestEnemy);
            }



            if (normalArrowProjectile._networkObject.IsSpawned == false)
                normalArrowProjectile._networkObject.Spawn();

            SoundManager.Instance.PlaySound(SoundType.Bow, playRandom: true, 0.5f);
        }
        private void EvoShooting(Player player, Vector2 nearestEnemyPosition)
        {
            ShootSingleEvoArrow(player, nearestEnemyPosition);

            StartCoroutine(WaitAfter(0.1f, () =>
            {
                ShootSingleEvoArrow(player, nearestEnemyPosition);
            }));

            StartCoroutine(WaitAfter(0.2f, () =>
            {
                ShootSingleEvoArrow(player, nearestEnemyPosition);
            }));

            StartCoroutine(WaitAfter(0.3f, () =>
            {
                ShootSingleEvoArrow(player, nearestEnemyPosition);
            }));
        }

        



        private void InitializeCachedClientIds(ulong clientId)
        {
            Debug.Log("InitializeCachedClientIds");
            // Assuming that you have a list of client IDs that doesn't change and you want to cache them
            List<ulong> clientIds = new List<ulong>
            {
                clientId
            };

            cachedClientIds = clientIds.ToArray();
        }
    }
}