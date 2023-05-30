namespace Sataura
{
    public class DoTProjectile : NetworkProjectile
    {
        public override void OnNetworkSpawn()
        {
            damageType = DamageType.DoT;
        }
    }
}