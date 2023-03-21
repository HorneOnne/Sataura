using UnityEngine;

namespace Sataura
{
    [CreateAssetMenu(fileName = "EnemyData", menuName = "Sataura/Enemy Data", order = 51)]
    public class EnemyData : ScriptableObject
    {
        [Header("Movement")]
        public float moveSpeed;

        [Header("Combat")]
        public int maxHealth;  
        public int damage;
        public float attackRange;
        public float attackCooldown;
        public float knockbackResistence = 5f; // The force of the knockback applied to the enemy
        public float knockbackDuration = 1f; 


       
    }

}
