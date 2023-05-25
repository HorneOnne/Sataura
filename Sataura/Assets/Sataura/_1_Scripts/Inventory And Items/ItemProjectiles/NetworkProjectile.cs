using Unity.Netcode;
using UnityEngine;

namespace Sataura
{

    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class NetworkProjectile : NetworkBehaviour
    {
        #region Properties
        public ItemData ItemData { get; private set; }
        protected GameObject Model { get;  set; }

        /// <summary>
        /// Whether the projectile uses gravity or not.
        /// </summary>
        public bool useGravity;
        #endregion


        [Header("References")]
        public NetworkObject _networkObject;

        protected ParticleControl particleControl;
        [HideInInspector] public SpriteRenderer spriteRenderer;
        [SerializeField] protected Rigidbody2D rb;
        protected GameDataManager itemDataManager;


        private void Awake()
        {
            LoadComponents();
        }


        public override void OnNetworkSpawn()
        {
            _networkObject = GetComponent<NetworkObject>();
        }

        private void LoadComponents()
        {
            Model = GetComponentInChildren<SpriteRenderer>().gameObject;
            spriteRenderer = Model.GetComponent<SpriteRenderer>();
            rb = GetComponent<Rigidbody2D>();
            particleControl = GetComponentInChildren<ParticleControl>();
            itemDataManager = GameDataManager.Instance;
        }


     
        public void SetData(ItemData itemData, bool updateSprite = true)
        {
            this.ItemData = itemData;

            if(updateSprite)
            {
                SetSprite(itemData.icon);
            }
            
        }


        public void SetData(ItemData itemData, Sprite sprite, bool updateSprite = true)
        {
            this.ItemData = itemData;

            if (updateSprite)
            {
                SetSprite(itemData.icon);
            }
        }


        [ServerRpc]
        public void SetDataServerRpc(int itemID, bool updateSprite = true)
        {;
            ItemData itemData = GameDataManager.Instance.GetItemData(itemID);
            SetData(itemData, updateSprite);

            SetDataClientRpc(itemID, updateSprite);       
        }

        [ClientRpc]
        private void SetDataClientRpc(int itemID, bool updateSprite = true)
        {
            if (IsServer) return;
            ItemData itemData = GameDataManager.Instance.GetItemData(itemID);
            SetData(itemData, updateSprite);
        }



        /// <summary>
        /// Sets the particle system to emit dust based on the given dust index.
        /// </summary>
        /// <param name="dustIndex">The index of the dust particles to emit.</param>
        public virtual void SetDust(int dustIndex)
        {
            if (particleControl == null) return;
            particleControl.SetParticles(itemDataManager.GetProjectileParticleFrames(dustIndex));
        }

        [ServerRpc]
        public void SetDustServerRpc(int dustIndex)
        {
            SetDust(dustIndex);
            SetDustClientRpc(dustIndex);
        }

        [ClientRpc]
        private void SetDustClientRpc(int dustIndex)
        {
            if (IsServer) return;

            SetDust(dustIndex);
        }

        /// <summary>
        /// Sets the sprite renderer's sprite to the given sprite.
        /// </summary>
        /// <param name="sprite">The sprite to set.</param>
        private void SetSprite(Sprite sprite)
        {
            if (spriteRenderer == null)
                LoadComponents();

            spriteRenderer.sprite = sprite;
        }

        [ServerRpc]
        public void UpdateSpriteServerRpc()
        {
            SetSprite(ItemData.icon);
            UpdateSpriteClientRpc();
        }

        [ClientRpc]
        private void UpdateSpriteClientRpc()
        {
            SetSprite(ItemData.icon);
        }



        /// <summary>
        /// Enables or disables gravity on the game object.
        /// </summary>
        /// <param name="_useGravity">Whether to use gravity or not.</param>
        private void UseGravity(bool _useGravity)
        {
            this.useGravity = _useGravity;
            if (useGravity)
            {
                rb.isKinematic = false;
                rb.gravityScale = 1.0f;
            }
            else
            {
                rb.isKinematic = true;
            }
        }
    }
}