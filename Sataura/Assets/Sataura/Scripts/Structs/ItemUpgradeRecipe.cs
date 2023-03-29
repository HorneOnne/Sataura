using System.Collections.Generic;

namespace Sataura
{
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
        public RecipeSlot outputItemSlot;


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