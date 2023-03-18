using UnityEngine;
using Unity.Netcode;
using System.Collections;

namespace Sataura
{
    public abstract class BaseEnemy : NetworkBehaviour, IDamageable
    {
        [SerializeField] protected Rigidbody2D rb2D;
        [SerializeField] protected EnemyData enemyData;
        [SerializeField] protected ItemDropData itemDropData;
        protected NetworkObject networkObject;
        protected NetworkObjectPool networkObjectPool;


        protected bool canTrigger = true;
        [SerializeField] protected NetworkVariable<int> currentHealth = new NetworkVariable<int>(0);

        #region Properties
        public Rigidbody2D Rb2D { get => rb2D; }
        public float Cooldown { get; set; }


        #endregion

        public bool isBeingKnockback;


        public override void OnNetworkSpawn()
        {
            currentHealth.Value = enemyData.maxHealth;
            networkObjectPool = NetworkObjectPool.Singleton;
            networkObject = GetComponent<NetworkObject>();
        }


        public void TakeDamage(int damaged)
        {
            currentHealth.Value -= damaged;
        }

        public bool IsOutOfHealth()
        {
            return currentHealth.Value <= 0;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (canTrigger)
            {
                ICanCauseDamage projectile = collision.gameObject.GetComponent<ICanCauseDamage>();
                if (projectile != null)
                {
                    TakeDamage(projectile.GetDamage());
                    if(IsOutOfHealth())
                    {
                        ReturnToNetworkPoolServerRpc();
                    }
                }

                canTrigger = false;
                Invoke("ResetTrigger", Cooldown);
            }
        }



        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (canTrigger)
            {
                ICanCauseDamage projectile = collision.gameObject.GetComponent<ICanCauseDamage>();
                if (projectile != null)
                {
                    TakeDamage(projectile.GetDamage());
                                                         
                    if (IsOutOfHealth())
                    {
                        DropItem();
                        ReturnToNetworkPoolServerRpc();
                    }
                    else
                    {
                        Vector2 direction = transform.position - collision.transform.position;
                        direction.Normalize();
                        Knockback(direction);
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

        public void Knockback(Vector2 direction)
        {
            isBeingKnockback = true;
            rb2D.velocity = Vector2.zero;
            rb2D.AddForce(direction.normalized * enemyData.knockbackForce, ForceMode2D.Impulse);
            StartCoroutine(KnockbackCoroutine());
        }

        private IEnumerator KnockbackCoroutine()
        {
            float timer = 0f;
            while (timer < enemyData.knockbackDuration)
            {
                timer += Time.deltaTime;
                yield return null;
            }
            rb2D.velocity = Vector2.zero;
            isBeingKnockback = false;
        }
    }

}
