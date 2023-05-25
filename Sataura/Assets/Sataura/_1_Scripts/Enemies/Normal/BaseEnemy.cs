using UnityEngine;
using Unity.Netcode;
using System.Collections;

namespace Sataura
{

    public abstract class BaseEnemy : NetworkBehaviour, IDamageable, IShowDamage
    {
        [Header("Base properties")]
        [SerializeField] protected Rigidbody2D rb2D;
        [SerializeField] protected BoxCollider2D boxCollider2D;
        [SerializeField] protected SpriteRenderer sr;
        [SerializeField] protected EnemyData enemyData;
        [SerializeField] protected ItemDropData itemDropData;
        protected NetworkObject networkObject;
        protected NetworkObjectPool networkObjectPool;


        [SerializeField] private GameObject damagedPopupPrefab;
        protected bool isDeath = false;


        [Header("Effects")]
        public bool blackHole = false;

        #region Properties
        [field: SerializeField] public float Cooldown { get; set; }
        public int CurrentHealth { get => currentHealth.Value; }
        public int MaxHealth { get => enemyData.maxHealth; }
        public Rigidbody2D Rb2D { get => rb2D; }
        public int EnemyDamaged
        {
            get
            {
                if (enemyData != null)
                    return enemyData.damage;
                else
                    return 0;
            }
        }
        #endregion


        // Cached
        protected bool canTrigger = true;
        protected bool isBeingKnockback;
        protected bool isDead = false;
        protected Transform playerTranform;
        [SerializeField] protected NetworkVariable<int> currentHealth = new NetworkVariable<int>(0);


        private void OnEnable()
        {
            EnemySpawnWaves.OnNewEnemyWaveSpawned += DestroyEnemyWhenSpawnNewWave;
        }

        private void OnDisable()
        {
            EnemySpawnWaves.OnNewEnemyWaveSpawned -= DestroyEnemyWhenSpawnNewWave;
        }

        private void DestroyEnemyWhenSpawnNewWave()
        {
            if (playerTranform == null) return;
            if ((playerTranform.position - transform.position).sqrMagnitude > 4900f)
            {
                OnEnemyDead();
                Despawn();
            }                
        }


        public override void OnNetworkSpawn()
        {
            currentHealth.Value = enemyData.maxHealth;
            networkObjectPool = NetworkObjectPool.Singleton;
            networkObject = GetComponent<NetworkObject>();
        }

        public virtual void SetFollowTarget(Transform target)
        {
            playerTranform = target;
        }


        public virtual void MoveAI(Vector2 target)
        {
            if (blackHole)
            {
                Invoke(nameof(ResetBlackHoleEffect), 0.2f);
                return;
            }          
        }

        private void ResetBlackHoleEffect()
        {
            blackHole = false;
        }

        float timeElapse = 0.0f;
        protected virtual void FixedUpdate()
        {
            if (!IsServer) return;

            if (Time.time - timeElapse >= 2.0f)
            {
                timeElapse = Time.time;

                if (playerTranform == null) return;
                if ((playerTranform.position - transform.position).sqrMagnitude > 10000f)
                {
                    OnEnemyDead();
                    Despawn();
                }
            }

            MoveAI(playerTranform.position);
        }

        public virtual void TakeDamage(int damaged)
        {
            currentHealth.Value -= damaged;
        }

        public virtual void OnEnemyDead()
        {
            boxCollider2D.enabled = false;
            isDead = true;

            IngameInformationManager.Instance.currentTotalEnemiesIngame--;
        }

        public bool IsOutOfHealth()
        {
            return currentHealth.Value <= 0;
        }


        private void OnCollisionEnter2D(Collision2D collision)
        {

        }



        private void OnTriggerStay2D(Collider2D collision)
        {
            if (!IsServer) return;

            if (canTrigger)
            {
                ICanCauseDamage projectile = collision.gameObject.GetComponent<ICanCauseDamage>();
                if (projectile != null)
                {
                    int damageReceived = projectile.GetDamage();
                    if (damageReceived <= 0) return;

                    TakeDamage(damageReceived);
                    if (isDead == true) return;

                    if (IsOutOfHealth())
                    {
                        rb2D.velocity = Vector2.zero;
                        Vector2 direction = transform.position - collision.transform.position;
                        direction.Normalize();
                        rb2D.AddForce(direction * 100, ForceMode2D.Impulse);

                        isDead = true;
                        OnEnemyDead();
                        DropItem();

                        Invoke(nameof(Despawn), 0.5f);
                        IngameInformationManager.Instance.totalEnemiesKilled += 1;
                    }
                    else
                    {
                        SoundManager.Instance.PlaySound(SoundType.EnemyHit, playRandom: true);
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

        public void GetLightningDamaged(int damageReceived)
        {
            TakeDamage(damageReceived);
            if (isDead == true) return;

            if (IsOutOfHealth())
            {
                rb2D.velocity = Vector2.zero;

                isDead = true;
                OnEnemyDead();
                DropItem();

                Invoke(nameof(Despawn), 0.5f);
                IngameInformationManager.Instance.totalEnemiesKilled += 1;
            }
            else
            {
                SoundManager.Instance.PlaySound(SoundType.EnemyHit, playRandom: true);
                ShowDamage(damageReceived);
            }
        }


        protected abstract void Despawn();


  


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
            // ===============================
            //var currencyValue = Random.Range(itemDropData.currencyDropFrom, itemDropData.currencyDropTo);
            //ConvertToCoin(currencyValue);


            // Items
            // ===============================
            for (int i = 0; i < itemDropData.itemDrops.Count; i++)
            {
                float rand = Random.Range(0f, 1f);
                if (rand <= itemDropData.itemDrops[i].dropRate / 100f)
                {
                    var itemDropPrefab = GameDataManager.Instance.itemForDropPrefab;
                    if (itemDropPrefab != null)
                    {
                        var itemDropObject = Instantiate(itemDropPrefab, transform.position, Quaternion.identity);
                        itemDropObject.GetComponent<ItemForDrop>().Set(itemDropData.itemDrops[i].itemDrop);
                    }
                }
            }


            // Experiences
            // ===============================
            if(IngameInformationManager.Instance.currentExpCount < 150)
            {
                var expObject = Instantiate(itemDropData.exp, transform.position, Quaternion.identity);
                expObject.GetComponent<NetworkObject>().Spawn();
            }
            
       


            // Test Probability
            // ===============================
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
            if (knockback < 0)
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
            if(IngameInformationManager.Instance.currentDamagePopupCount > 30)
            {
                return;
            }

            var textNetworkObject = Instantiate(damagedPopupPrefab).GetComponent<NetworkObject>();
            textNetworkObject.Spawn();
            textNetworkObject.transform.position = transform.position;
            var moveTextObjectVector = new Vector3(Random.Range(-1f, 1f), Random.Range(0.5f, 1f));

            Vector3 textObjectRotation;
            if (moveTextObjectVector.x > 0)
                textObjectRotation = new Vector3(0, 0, Random.Range(-30f, 0));
            else
                textObjectRotation = new Vector3(0, 0, Random.Range(0f, 30f));

            textNetworkObject.GetComponent<DamagePopup>().SetUp(damaged, GetDamageColor(damaged), GetDamageSize(damaged), moveTextObjectVector, textObjectRotation);
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
                    return 10;
                case float n when (n >= 25 && n < 50):
                    return 11;
                case float n when (n >= 50 && n < 75):
                    return 12; // Purple
                case float n when (n >= 75 && n < 90):
                    return 13;
                case float n when (n >= 90):
                    return 15;
                default:
                    return 15;
            }
        }
    }

}
