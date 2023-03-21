using UnityEngine;
using Unity.Netcode;
using System.Collections;

namespace Sataura
{

    public abstract class BaseEnemy : NetworkBehaviour, IDamageable
    {
        [Header("Base properties")]
        [SerializeField] protected Animator anim;
        [SerializeField] protected Rigidbody2D rb2D;
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


        float knockbackDuration = 2f;

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
            if (canTrigger)
            {
                ICanCauseDamage projectile = collision.gameObject.GetComponent<ICanCauseDamage>();
                if (projectile != null)
                {
                    TakeDamage(projectile.GetDamage());
                    if (isDead == true) return;
                    
                    if (IsOutOfHealth())
                    {
                        isDead = true;
                        OnEnemyDead();
                        DropItem();

                        Invoke("ReturnToNetworkPoolServerRpc", 1f);
                        //ReturnToNetworkPoolServerRpc();
                    }
                    else
                    {
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

        private void ConvertToCoin(int currency)
        {
            int gold = currency / 10000; // calculate the number of gold coins
            int silver = (currency % 10000) / 100; // calculate the number of silver coins
            int bronze = currency % 100; // calculate the number of bronze coins

            float coinForce = Random.Range(3f, 7f);


            if (gold > 0 && gold <= 99)
            {
                var goldCoinPrefab = GameDataManager.Instance.GetGoldCoin();
                var goldCoinObject = Instantiate(goldCoinPrefab, (Vector2)transform.position + Random.insideUnitCircle, Quaternion.identity);
                Currency currencyCoin = goldCoinObject.GetComponent<Currency>();
                currencyCoin.coinValue = gold;
                goldCoinObject.GetComponent<NetworkObject>().Spawn();

                // Apply force to the coin
                currencyCoin.Rb2D.AddForce(Vector2.up * coinForce, ForceMode2D.Impulse);
            }
            else
            {
                if(gold > 99)
                {
                    throw new System.Exception("Currency out of range.");
                }          
            }

            if (silver > 0)
            {
                var sliverCoinPrefab = GameDataManager.Instance.GetSliverCoin();
                var sliverCoinObject = Instantiate(sliverCoinPrefab, (Vector2)transform.position + Random.insideUnitCircle, Quaternion.identity);
                Currency currencyCoin = sliverCoinObject.GetComponent<Currency>();
                currencyCoin.coinValue = silver;
                sliverCoinObject.GetComponent<NetworkObject>().Spawn();

                // Apply force to the coin
                currencyCoin.Rb2D.AddForce(Vector2.up * coinForce, ForceMode2D.Impulse);               
            }

            if (bronze > 0)
            {
                var bronzeCoinPrefab = GameDataManager.Instance.GetBronzeCoin();
                var bronzeCoinObject = Instantiate(bronzeCoinPrefab, (Vector2)transform.position + Random.insideUnitCircle, Quaternion.identity);
                Currency currencyCoin = bronzeCoinObject.GetComponent<Currency>();
                currencyCoin.coinValue = bronze;
                bronzeCoinObject.GetComponent<NetworkObject>().Spawn();

                // Apply force to the coin
                currencyCoin.Rb2D.AddForce(Vector2.up * coinForce, ForceMode2D.Impulse);
            }


            //Debug.Log($"gold: {gold}, \t silver: {silver} \t bronze: {bronze}");
        }
        public void DropItem()
        {
            var currencyValue = Random.Range(itemDropData.currencyDropFrom, itemDropData.currencyDropTo);
            ConvertToCoin(currencyValue);
        }

        public void Knockback(Vector2 direction, float knockback)
        {
            isBeingKnockback = true;
            rb2D.velocity = Vector2.zero;
            Debug.Log($"Knockback Applided: {CalculateKnockbackFormula(knockback)}");
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



    }

}
