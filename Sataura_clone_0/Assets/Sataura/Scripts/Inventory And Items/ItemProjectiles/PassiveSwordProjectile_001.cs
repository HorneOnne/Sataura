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
    }
}

