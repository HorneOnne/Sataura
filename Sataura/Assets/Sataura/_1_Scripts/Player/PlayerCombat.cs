using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using System.Collections;

namespace Sataura
{
    public class PlayerCombat : NetworkBehaviour, IDamageable
    {
        [Header("Combat properties")]
        [SerializeField] private int currentHealth;

        [Header("Player references")]
        [SerializeField] private IngamePlayer player;

        [Header("References")]
        [SerializeField] private Slider healthBarSlider;
        [SerializeField] private Material characterMaterial;
        [SerializeField] private BoxCollider2D characterCollider2D;

        [Header("Runtime References")]
        [SerializeField] private GameObject bloodTearVFXObject;
        [SerializeField] private ParticleSystem _bloodTearPS;
        


        [Header("Performance added")]
        private BaseEnemy baseEnemy;
        private Color damageColor = Color.red;
        private Color defaultColor = Color.white;


        [Header("Detect Enemy Settings")]
        private float detectionRadius = 1.2f;
        private float detectionInterval = 0.2f;
        private float lastDetectionTime = 0f;
        [SerializeField] private Collider2D[] enemies = new Collider2D[7]; // Array to store results of the overlap check
        [SerializeField] private LayerMask enemyLayer;

        #region Properties
        public float cooldown { get; set; }
        #endregion


        private void Start()
        {
            healthBarSlider.minValue = 0;
            currentHealth = player.characterData._currentMaxHealth;

            healthBarSlider.maxValue = player.characterData._currentMaxHealth;
            healthBarSlider.value = currentHealth;

            characterMaterial.color = defaultColor;
        }

        public void UpdateMaxHealthSlider()
        {
            healthBarSlider.minValue = 0;
            healthBarSlider.maxValue = player.characterData._currentMaxHealth;
        }
        
        public override void OnNetworkSpawn()
        {
            StartCoroutine(WaitForOtherObject());
        }
        private IEnumerator WaitForOtherObject()
        {
            yield return new WaitUntil(() => GameDataManager.Instance.bloodTearVFX_001 != null);
            bloodTearVFXObject = Instantiate(GameDataManager.Instance.bloodTearVFX_001, transform.position, Quaternion.identity, this.transform);
            _bloodTearPS = bloodTearVFXObject.GetComponent<ParticleSystem>();
        }


        private void Update()
        {
            if (player.IsGameOver()) return;

            if (Time.time - lastDetectionTime > detectionInterval)
            {
                lastDetectionTime = Time.time;

                // Detect enemies within the specified radius
                int numEnemies = Physics2D.OverlapCircleNonAlloc(transform.position, detectionRadius, enemies, enemyLayer);
                if (numEnemies > 0)
                {
                    for (int i = 0; i < numEnemies; i++)
                    {
                        if (enemies[i].TryGetComponent<BaseEnemy>(out baseEnemy))
                        {
                            TakeDamage(baseEnemy.EnemyDamaged);
                            UpdateHealthBarUI();
                        }          
                    }

                    PlayBloodTearVFX();
                }
                else
                {
                    StopBloodTearVFX();
                }

                if(IsOutOfHealth())
                {
                    SetGameOverCharacterSettings();
                    _bloodTearPS.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                    UICanvasGameOver.Instance.SetActive();
                    IngameInformationManager.Instance.SetGameState(IngameInformationManager.GameState.GameOver);
                    
                }
            }

        }

        public void Healing(int value)
        {
            if(currentHealth + value <= player.characterData._currentMaxHealth)
            {
                currentHealth += value;
            }
            else
            {
                currentHealth = player.characterData._currentMaxHealth;
            }
        }

        public bool IsOutOfHealth()
        {
            // Call this event when player out of health.
            return currentHealth <= 0;
        }

        private int CalculateDamage(float baseDamage, float armor)
        {
            float damageReduction = armor / (armor + 100); // Calculate the percentage of damage reduction based on armor
            float finalDamage = baseDamage * (1 - damageReduction); // Calculate the final damage after applying damage reduction

            return (int)finalDamage;
        }

        public void TakeDamage(int enemyDamage)
        {
            int damageToPlayer = CalculateDamage(enemyDamage, player.characterData._currentArmor);
            currentHealth -= damageToPlayer;

            Debug.Log($"{enemyDamage}\t{damageToPlayer}\t{player.characterData._currentArmor}");
        }

        public void UpdateHealthBarUI()
        {
            healthBarSlider.value = currentHealth;
        }

        public void PlayBloodTearVFX()
        {
            if (_bloodTearPS != null)
            {
                if (_bloodTearPS.isPlaying == false)
                {
                    characterMaterial.color = damageColor;
                    _bloodTearPS.Play();
                }
            }
        }

     
        public void StopBloodTearVFX()
        {
            if (_bloodTearPS != null)
            {
                if (_bloodTearPS.isPlaying == true || _bloodTearPS.isEmitting == true)
                {
                    _bloodTearPS.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                    characterMaterial.color = defaultColor;
                }
            }
        }

 


        private void SetGameOverCharacterSettings()
        {
            healthBarSlider.gameObject.SetActive(false);
            player.playerMovement.Rb2D.velocity = Vector2.zero;
            player.playerMovement.Rb2D.isKinematic = true; 
            characterCollider2D.isTrigger = true;
            defaultColor = Utilities.HexToColor("#636363");
            characterMaterial.color = defaultColor;
        }




        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
    }

}