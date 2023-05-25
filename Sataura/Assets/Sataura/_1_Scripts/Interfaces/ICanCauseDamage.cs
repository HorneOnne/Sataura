namespace Sataura
{
    /// <summary>
    /// Interface for objects that can cause damage.
    /// </summary>
    public interface ICanCauseDamage
    {
        public int GetDamage();
        public float GetKnockback();
    }
}