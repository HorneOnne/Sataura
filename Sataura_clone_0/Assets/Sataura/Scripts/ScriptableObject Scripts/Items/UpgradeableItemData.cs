using System.Collections.Generic;
using UnityEngine;

namespace Sataura
{
    /// <summary>
    /// Represents an upgradeable item that can be leveled up and has an upgrade recipe.
    /// </summary>
    public class UpgradeableItemData : ItemData
    {
        [Header("LEVEL")]
        public int currentLevel;
        public int maxLevel;


        [Header("UPGRADE RECIPE")]
        public ItemUpgradeRecipe upgradeRecipe;
    }


    /// <summary>
    /// Represents a recipe for upgrading an item.
    /// </summary>
    [System.Serializable]
    public struct ItemUpgradeRecipe
    {
        /// <summary>
        /// The materials required for the upgrade recipe.
        /// </summary>
        public List<RecipeSlot> materials;

        /// <summary>
        /// The output item of the upgrade recipe.
        /// </summary>
        public RecipeSlot outputItem;


        [System.Serializable]
        public struct RecipeSlot
        {
            public ItemData itemData;
            public int quantity;

            public RecipeSlot(ItemData itemData, int quantity)
            {
                this.itemData = itemData;
                this.quantity = quantity;
            }

        }
    }
}