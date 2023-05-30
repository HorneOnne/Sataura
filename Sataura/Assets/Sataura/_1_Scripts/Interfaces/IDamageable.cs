using System;

namespace Sataura
{
    public interface IDamageable
    {
        public void TakeDamage(int damaged);

        public bool IsOutOfHealth();
    }
}

