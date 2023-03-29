using UnityEngine;

namespace Sataura
{
    public class Enemy001 : BaseEnemy
    {
        public override void MoveAI(Vector2 playerPosition)
        {
            Vector2 direction = playerPosition - (Vector2)transform.position;
            direction.Normalize();
            rb2D.MovePosition((Vector2)transform.position + direction * 10 * Time.fixedDeltaTime);
        }

        protected override void ReturnToNetworkPool()
        {
            if(networkObject.IsSpawned)
                networkObject.Despawn(false);

            var enemyPrefab = GameDataManager.Instance.GetEnemyPrefab("EP_Enemies001");
            networkObjectPool.ReturnNetworkObject(networkObject, enemyPrefab);
        }      
    }

}
