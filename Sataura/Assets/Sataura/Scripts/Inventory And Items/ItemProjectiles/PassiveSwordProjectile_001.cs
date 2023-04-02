namespace Sataura
{
    public class PassiveSwordProjectile_001 : NetworkProjectile, ICanCauseDamage
    {
        public int GetDamage()
        {
            return 30;
        }

        public float GetKnockback()
        {
            return 10;
        }

        public override void OnNetworkSpawn()
        {
            Invoke(nameof(DespawnNetworkObject), 0.25f);
        }

        private void DespawnNetworkObject()
        {
            if(base.networkObject.IsSpawned)
            {
                base.networkObject.Despawn();
            }
            
        }
    }
}

