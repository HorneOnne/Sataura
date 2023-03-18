using UnityEngine;

namespace Sataura
{
    public class Enemy001 : BaseEnemy
    {
        protected override void ReturnToNetworkPool()
        {
            if(networkObject.IsSpawned)
                networkObject.Despawn(false);

            var enemyPrefab = GameDataManager.Instance.GetEnemyPrefab("EP_Enemies001");
            networkObjectPool.ReturnNetworkObject(networkObject, enemyPrefab);
        }
    }

}
