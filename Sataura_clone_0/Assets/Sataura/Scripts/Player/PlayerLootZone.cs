using UnityEngine;

namespace Sataura
{
    /// <summary>
    /// A script to handle the collection of items by the player in a loot zone.
    /// </summary>
    public class PlayerLootZone : MonoBehaviour
    {
        /// <summary>
        /// The game object representing the player.
        /// </summary>
        public GameObject playerGameObject;

        // Cached
        private Player player;
        private ICollectible collectibleObject;
        private UIPlayerInGameInventory uiInGameInventory;

        private void Start()
        {
            player = playerGameObject.GetComponent<Player>();
            uiInGameInventory = UIPlayerInGameInventory.Instance;
        }



        private void OnTriggerEnter2D(Collider2D collision)
        {
            collectibleObject = collision.gameObject.GetComponent<ICollectible>();
            if (collectibleObject != null)
            {
                collectibleObject.Collect(player);
                uiInGameInventory.UpdateInventoryUI();
            }
        }
    }
}
