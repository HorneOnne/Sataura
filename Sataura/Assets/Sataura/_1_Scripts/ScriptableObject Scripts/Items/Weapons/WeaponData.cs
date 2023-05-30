using UnityEngine;

namespace Sataura
{
    public class WeaponData : ItemData
    {
        [Header("PROPERTIES")]
        public int damage;
        public float knockback;
        public float size;

        [Header("LEVELUP PROPERTIES")]
        public int useType = 1;
    }
}

