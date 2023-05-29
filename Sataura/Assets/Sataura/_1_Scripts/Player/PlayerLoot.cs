using Unity.Netcode;
using UnityEngine;

namespace Sataura
{
    public class PlayerLoot : NetworkBehaviour
    {
        public GameObject playerGameObject;
        [SerializeField] private Collider2D[] overlappedColliders = new Collider2D[10];
        [SerializeField] private LayerMask lootableLayer;

        // Cached
        private IngamePlayer _player;
        private CharacterData _characterData;



        [Header("Options")]
        [SerializeField] bool showGizmos = true;


        [Header("Performance Settings")]
        [Tooltip("The larger the value, the higher the performance.")]
        [SerializeField] private float updateInterval = 0.2f;
        private float updateTimer = 0.0f;



        private void Start()
        {
            _player = playerGameObject.GetComponent<IngamePlayer>();
            _characterData = _player.characterData;
        }


        private void Update()
        {
            updateTimer += Time.deltaTime;
            if(updateTimer >= updateInterval)
            {
                // Reset the timer.
                updateTimer = 0.0f;

                LootExperiences();
            }
        }

        private void LootExperiences()
        {
            int numOverlaps = Physics2D.OverlapCircleNonAlloc(transform.position, _characterData._currentMagnet, overlappedColliders, lootableLayer);
            
            if(numOverlaps > 0)
            {
                for (int i = 0; i < numOverlaps; i++)
                {
                    Collider2D overlappedCollider = overlappedColliders[i];
                    Experience experience;
                    if (overlappedCollider.TryGetComponent(out experience))
                    {
                        experience.Collect(_player);
                    }
                }
            }                  
        }

        void OnDrawGizmos()
        {
            if (showGizmos == false) return;
            if (_characterData == null) return;

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, _characterData._currentMagnet);
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
    }
}
