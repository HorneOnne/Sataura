using System;
using UnityEngine;

namespace Sataura
{
    /// <summary>
    /// Base class for all item data.
    /// </summary>
    public abstract class ItemData : ScriptableObject
    {      
        [Header("ITEM DATA")]
        public string itemName;
        public Sprite icon;
        public ItemCategory itemCategory;
        public ItemType itemType;
        public int max_quantity;
        [Multiline(5)]
        public string hoverDescription;
        [Multiline(5)]
        public string equipDescription;
        [Multiline(5)]
        public string unequipDescription;
        [Multiline(5)]
        public string ingameDescription;

       
        public float rateOfFire = 1.0f;


        [Header("LEVEL")]
        public int currentLevel;
        public int maxLevel;


        [Header("UPGRADE RECIPE")]
        public ItemUpgradeRecipe upgradeRecipe;

        public bool IsMaxLevel()
        {
            return currentLevel == maxLevel;
        }

        /// <summary>
        /// Determines whether this instance and another specified <see cref="ItemData"/> object have the same value.
        /// </summary>
        /// <param name="other">The <see cref="ItemData"/> object to compare to this instance.</param>
        /// <returns><c>true</c> if the value of the <paramref name="other"/> parameter is the same as the value of this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object other)
        {
            if (this.itemName != ((ItemData)other).itemName) return false;
            if (this.icon != ((ItemData)other).icon) return false;
            if (this.itemCategory != ((ItemData)other).itemCategory) return false;
            if (this.itemType != ((ItemData)other).itemType) return false;
            if (this.max_quantity != ((ItemData)other).max_quantity) return false;
            if (this.hoverDescription != ((ItemData)other).hoverDescription) return false;
            if (this.ingameDescription != ((ItemData)other).ingameDescription) return false;
            if (this.rateOfFire != ((ItemData)other).rateOfFire) return false;
            if (this.currentLevel != ((ItemData)other).currentLevel) return false;
            if (this.maxLevel != ((ItemData)other).maxLevel) return false;

            return true;
        }


        /// <summary>
        /// Determines whether two specified <see cref="ItemData"/> objects have the same value.
        /// </summary>
        /// <param name="itemA">The first <see cref="ItemData"/> object to compare.</param>
        /// <param name="itemB">The second <see cref="ItemData"/> object to compare.</param>
        /// <returns><c>true</c> if the value of the <paramref name="itemA"/> parameter is the same as the value of the <paramref name="itemB"/> parameter; otherwise, <c>false</c>.</returns>
        public static bool IsSameItem(ItemData itemA, ItemData itemB)
        {
            if (itemA == null || itemB == null) return false;
            return itemA.Equals(itemB);
        }

        public static bool IsSameName(ItemData itemA, ItemData itemB)
        {
            if (itemA == null || itemB == null) return false;
            return itemA.itemName.Equals(itemB.itemName);
        }


        public override int GetHashCode()
        {
            return HashCode.Combine(itemName, icon, itemCategory, itemType, max_quantity, rateOfFire, currentLevel);
        }
    }
}