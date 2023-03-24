using UnityEngine;
using Unity.Netcode;
using System.Collections;

namespace Sataura
{

    public abstract class BaseEnemy : NetworkBehaviour, IDamageable, IShowDamage
    {
        [Header("Base properties")]
        [SerializeField] protected Animator anim;
        [SerializeField] protected Rigidbody2D rb2D;
        [SerializeField] protected BoxCollider2D boxCollider2D;
        [SerializeField] protected SpriteRenderer sr;
        [SerializeField] protected EnemyData enemyData;
        [SerializeField] protected ItemDropData itemDropData;
        protected NetworkObject networkObject;
        protected NetworkObjectPool networkObjectPool;


        #region Properties
        [field: SerializeField] public float Cooldown { get; set; }
        #endregion


        // Cached
        protected bool canTrigger = true;
        protected bool isBeingKnockback;
        protected bool isDead = false;
        private Transform playerTranform;
        [SerializeField] protected NetworkVariable<int> currentHealth = new NetworkVariable<int>(0);

        public override void OnNetworkSpawn()
        {
            currentHealth.Value = enemyData.maxHealth;
            networkObjectPool = NetworkObjectPool.Singleton;
            networkObject = GetComponent<NetworkObject>();
        }

        public void SetFollowTarget(Transform target)
        {
            playerTranform = target;
        }


        public abstract void MoveAI(Vector2 target);


        float timeElapse = 0.0f;
        protected virtual void FixedUpdate()
        {
            if (!IsServer) return;

            if (Time.time - timeElapse >= 0.035f)
            {
                timeElapse = Time.time;
                MoveAI(playerTranform.position);
            }
        }

        public virtual void TakeDamage(int damaged)
        {
            currentHealth.Value -= damaged;          
        }

        public virtual void OnEnemyDead() { }

        public bool IsOutOfHealth()
        {
            return currentHealth.Value <= 0;
        }


        private void OnCollisionEnter2D(Collision2D collision)
        {
         
        }



        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!IsServer) return;

            if (canTrigger)
            {
                ICanCauseDamage projectile = collision.gameObject.GetComponent<ICanCauseDamage>();
                if (projectile != null)
                {
                    int damageReceived = projectile.GetDamage();
                   
                    TakeDamage(damageReceived);                       
                    if (isDead == true) return;
                    
                    if (IsOutOfHealth())
                    {
                        isDead = true;
                        OnEnemyDead();
                        DropItem();

                        Invoke("ReturnToNetworkPoolServerRpc", 1f);
                        EnemySpawnWaves.currentTotalEnemiesIngame -= 1;
                        //ReturnToNetworkPoolServerRpc();
                    }
                    else
                    {
                        SoundManager.Instance.PlaySound(SoundType.EnemyHit);
                        ShowDamage(damageReceived);

                        Vector2 direction = transform.position - collision.transform.position;
                        direction.Normalize();
                        Knockback(direction, projectile.GetKnockback());                       
                    }
                    
                }

                canTrigger = false;
                Invoke("ResetTrigger", Cooldown);
            }
        }

        private void ResetTrigger()
        {
            canTrigger = true;
        }

        protected abstract void ReturnToNetworkPool();
        

        [ServerRpc]
        private void ReturnToNetworkPoolServerRpc()
        {
            ReturnToNetworkPool();
            ReturnToNetworkPoolClientRpc();
        }

        [ClientRpc]
        private void ReturnToNetworkPoolClientRpc()
        {
            if (IsServer) return;
            ReturnToNetworkPool();
        }


        // DROP ITEM HANDLE
        // ==========================================================
        private void ConvertToCoin(int currency)
        {
            // calculate the number of gold, silver, and bronze coins
            int gold = currency / 10000;
            int silver = (currency / 100) % 100;
            int bronze = currency % 100;

            // spawn and apply force to gold coins
            if (gold > 0 && gold <= 99)
            {
                SpawnCoin(GameDataManager.Instance.GetGoldCoin(), gold);
            }
            else if (gold > 99)
            {
                throw new System.Exception("Currency out of range.");
            }

            // spawn and apply force to silver coins
            if (silver > 0)
            {
                SpawnCoin(GameDataManager.Instance.GetSliverCoin(), silver);
            }

            // spawn and apply force to bronze coins
            if (bronze > 0)
            {
                SpawnCoin(GameDataManager.Instance.GetBronzeCoin(), bronze);
            }
        }

        private void SpawnCoin(GameObject coinPrefab, int coinValue)
        {
            // instantiate the coin object and set its value
            var coinObject = Instantiate(coinPrefab, (Vector2)transform.position + Random.insideUnitCircle, Quaternion.identity);
            Currency currencyCoin = coinObject.GetComponent<Currency>();
            currencyCoin.coinValue = coinValue;
            coinObject.GetComponent<NetworkObject>().Spawn();

            // apply force to the coin
            float coinForce = Random.Range(3f, 7f);
            currencyCoin.Rb2D.AddForce(Vector2.up * coinForce, ForceMode2D.Impulse);
        }


        public float dropRate = 0.5f;  // The probability of the item dropping (between 0 and 1)
        public void DropItem()
        {
            // Coins
            var currencyValue = Random.Range(itemDropData.currencyDropFrom, itemDropData.currencyDropTo);
            ConvertToCoin(currencyValue);

          
            // Items
            for(int i = 0; i < itemDropData.itemDrops.Count; i++)
            {
                float rand = Random.Range(0f, 1f);
                if (rand <= itemDropData.itemDrops[i].dropRate / 100f)
                {
                    var itemDropPrefab = GameDataManager.Instance.GetItemPrefab($"IP_ItemForDrop");
                    if(itemDropPrefab != null)
                    {
                        var itemDropObject = Instantiate(itemDropPrefab, transform.position, Quaternion.identity);
                        itemDropObject.GetComponent<ItemForDrop>().Set(itemDropData.itemDrops[i].itemDrop);
                    }
                }
            }

            
            // Test Probability

            /*int sucessCount = 0;
            int failCount = 0;
            int totalTest = 30000;

            for(int k = 0; k < totalTest; k++)
            {
                float rand = Random.Range(0f, 1f);
                for (int i = 0; i < itemDropData.itemDrops.Count; i++)
                {
                    if (rand <= itemDropData.itemDrops[i].dropRate / 100f)
                    {
                        sucessCount++;
                    }
                    else
                    {
                        failCount++;
                    }
                }
            }
            Debug.Log($"Total: {totalTest}\tSucess: {sucessCount}\tFail: {failCount}\tPercent: {sucessCount * 100f/totalTest}");  */        
        }



        




        // KNOCKBACK HANDLE
        // ==========================================================
        public void Knockback(Vector2 direction, float knockback)
        {
            isBeingKnockback = true;
            rb2D.velocity = Vector2.zero;
            rb2D.AddForce(direction.normalized * CalculateKnockbackFormula(knockback), ForceMode2D.Impulse);
            StartCoroutine(KnockbackCoroutine(CalculateKnockbackFormula(knockback)));
        }


        private float CalculateKnockbackFormula(float knockback)
        {
            float knockbackValue = knockback - enemyData.knockbackResistence;
            if(knockback < 0)
            {
                return 0.0f;
            }
            else
            {
                return knockbackValue;
            }

        }

        private IEnumerator KnockbackCoroutine(float knockbackvalue)
        {
            yield return new WaitForSeconds(.3f + knockbackvalue / 60f);
            isBeingKnockback = false;
        }




        // DISPLAY DAMAGED HANDLE
        // ==========================================================
        public void ShowDamage(int damaged)
        {
            var textObject = DamagePopupSpawner.Instance.Pool.Get();
            textObject.transform.position = transform.position;
            var moveTextObjectVector = new Vector3(Random.Range(-1f, 1f), Random.Range(0.5f, 1f));

            Vector3 textObjectRotation;
            if (moveTextObjectVector.x > 0)
                textObjectRotation = new Vector3(0, 0, Random.Range(-30f, 0));
            else
                textObjectRotation = new Vector3(0, 0, Random.Range(0f, 30f));

            textObject.GetComponent<DamagePopup>().SetUp(damaged, GetDamageColor(damaged), GetDamageSize(damaged), moveTextObjectVector, textObjectRotation);
        }

        private Color GetDamageColor(float damage)
        {
            switch (damage)
            {
                case float n when (n >= 0 && n < 25):
                    return Color.green;
                case float n when (n >= 25 && n < 50):
                    return Color.blue;
                case float n when (n >= 50 && n < 75):
                    return new Color(0.6f, 0.2f, 1f); // Purple
                case float n when (n >= 75 && n < 90):
                    return Color.red;
                case float n when (n >= 90):
                    return Color.yellow;
                default:
                    return Color.white;
            }
        }

        private float GetDamageSize(float damage)
        {
            switch (damage)
            {
                case float n when (n >= 0 && n < 25):
                    return 15;
                case float n when (n >= 25 && n < 50):
                    return 16;
                case float n when (n >= 50 && n < 75):
                    return 17; // Purple
                case float n when (n >= 75 && n < 90):
                    return 19;
                case float n when (n >= 90):
                    return 25;
                default:
                    return 15;
            }
        }
    }

}
