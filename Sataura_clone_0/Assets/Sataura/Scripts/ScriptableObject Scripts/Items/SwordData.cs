using UnityEngine;

namespace Sataura
{
    [CreateAssetMenu(fileName = "New Sword Object", menuName = "UltimateItemSystem/Item/Weapons/Sword", order = 51)]
    public class SwordData : UpgradeableItemData
    {
        [Header("SWORD PROPERTIES")]
        public int damage;
        public float knockback;

        [Header("UPGRADE PROPERTIES")]
        public int useType;
    }
}
