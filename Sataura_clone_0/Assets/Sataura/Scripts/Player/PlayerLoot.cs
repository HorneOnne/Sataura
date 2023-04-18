using Unity.Netcode;
using UnityEngine;

namespace Sataura
{
    public class PlayerLoot : NetworkBehaviour
    {
        public GameObject playerGameObject;
        private float lootRadius = 5;
        [SerializeField] private Collider2D[] overlappedColliders = new Collider2D[10];
        [SerializeField] private LayerMask lootableLayer;

        // Cached
        private Player player;
        //private ICollectible collectibleObject;
        //private UIPlayerInGameInventory uiInGameInventory;


        [Header("Options")]
        [SerializeField] bool showGizmos = true;

        

        private void Start()
        {
            player = playerGameObject.GetComponent<Player>();
            //uiInGameInventory = UIPlayerInGameInventory.Instance;
        }


        
        private void FixedUpdate()
        {
            int numOverlaps = Physics2D.OverlapCircleNonAlloc(transform.position, lootRadius, overlappedColliders, lootableLayer);
            
            if(numOverlaps > 0)
            {
                for (int i = 0; i < numOverlaps; i++)
                {
                    Collider2D overlappedCollider = overlappedColliders[i];
                    Experience experience;

                    if (overlappedCollider.TryGetComponent(out experience))
                    {
                        experience.Collect(player);
                    }
                }
            }
            
        
        }

        void OnDrawGizmos()
        {
            if (showGizmos == false) return;

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, lootRadius);
        }


        public void SetLootRadius(float radius)
        {
            if (radius <= 0.0f)
                return;

            this.lootRadius = radius;
        }


        /*private void OnTriggerEnter2D(Collider2D collision)
        {
            collectibleObject = collision.gameObject.GetComponent<ICollectible>();
            if (collectibleObject != null)
            {
                if(collectibleObject is Item)
                {
                    collectibleObject.Collect(player);
                    uiInGameInventory.UpdateInventoryUI();
                }
                else if(collectibleObject is Currency)
                {
                    collectibleObject.Collect(player);

                    if(IsServer)
                        collision.gameObject.GetComponent<NetworkObject>().Despawn();
                }
                else if(collectibleObject is Experience)
                {
                    collectibleObject.Collect(player);
                }
            }
        }*/





        [ClientRpc]
        private void LootClientRpc()
        {

        }
    }
}
