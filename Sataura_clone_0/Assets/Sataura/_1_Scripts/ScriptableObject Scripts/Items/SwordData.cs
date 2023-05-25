using UnityEngine;

namespace Sataura
{
    [CreateAssetMenu(fileName = "New Sword Object", menuName = "Sataura/Item/Weapons/Sword", order = 51)]
    public class SwordData : ItemData
    {
        [Header("SWORD PROPERTIES")]
        public int damage;
        public float knockback;
        public float size = 1.0f;

        [Header("UPGRADE PROPERTIES")]
        public int useType = 1;
    }
}
