using System;
using System.Collections;
using UnityEngine;
using Unity.Netcode;

namespace Sataura
{
    public class IngameInformationManager : Singleton<IngameInformationManager>
    {
        public static event System.Action OnPlayerLevelUp;

        public int currentTotalEnemiesIngame;
        public int totalEnemiesKilled;

        
        private const int initialexperienceToNextLevelValue = 10;
        [Header("Level")]
        [SerializeField] private int level;

        [Header("EXP")]
        [SerializeField] private int experience;
        private int experienceToNextLevel;


        [Header("References")]
        private Player player;
        [SerializeField] private Animator fadeOutAnim;


        // Others
        [Header("Others")]
        public int currentDamagePopupCount;
        public int currentExpCount;




        private GameState currentGameState;
        public enum GameState
        {
            Start,
            Play,
            Pause,
            GameOver, 
            Victory
        }

        [Header("Options")]
        [SerializeField] private bool useLevelUpVFX = true;


        #region Properties
        public int Level { get { return level; } }  
        public int Experience { get { return experience; } }  
        public int ExperienceToNextLevel { get { return experienceToNextLevel; } }  
        public GameState CurrentGameState { get { return currentGameState; } }  
        #endregion


        private void Awake()
        {
            level = 0;
            experience = 0;
            experienceToNextLevel = initialexperienceToNextLevelValue;
            
            SetGameState(GameState.Start);
            StartCoroutine(StartGame());
        }

        private IEnumerator StartGame()
        {
            fadeOutAnim.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            fadeOutAnim.enabled = true;
            SetGameState(GameState.Play);
            yield return new WaitForSeconds(1f);
            fadeOutAnim.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            OnPlayerLevelUp += GenerateLevelUpVFX;
        }

 
        private void OnDisable()
        {
            OnPlayerLevelUp -= GenerateLevelUpVFX;
        }


        private void Start()
        {
            NetworkManager.Singleton.StartHost();
            currentDamagePopupCount = 0;
        }


        private float _victoryTimer = 0.0f;
        private void Update()
        {
            if(CurrentGameState == GameState.Victory)
            {
                _victoryTimer += Time.deltaTime;
                if(_victoryTimer >= 2.0f)
                {
                    Time.timeScale = 0.0f;

                    if(UIManager.Instance.victoryCanvas.enabled == false)
                        UIManager.Instance.victoryCanvas.enabled = true;
                }
                
            }
        }

        private void GenerateLevelUpVFX()
        {
            if(player == null)
                player = GameDataManager.Instance.singleModePlayer.GetComponent<Player>();

            var vfxObject = Instantiate(GameDataManager.Instance.levelUpVFX, player.transform.position, Quaternion.identity, player.transform);
            Destroy(vfxObject, 5f);
        }

        // Call this method whenever the character gains experience points
        public void GainExperience(int amount)
        {
            experience += amount;
            if (experience >= experienceToNextLevel)
            {
                //Debug.Log($"lv: {level}\t exp: {experience} \t expToNextLv: {experienceToNextLevel}");
                LevelUp();
            }
        }



        public void LevelUp()
        {
            level++;
            experience -= experienceToNextLevel;
            experienceToNextLevel = Mathf.RoundToInt(initialexperienceToNextLevelValue * Mathf.Pow(1.1f, level)); // Increase the experience required for the next level by 10% per level
            //Debug.Log("Congratulations, you've reached level " + level + "!");

            OnPlayerLevelUp?.Invoke();
        }


        public void SetGameState(GameState gameState)
        {
            currentGameState = gameState;
        }

        public void SetVictoryState()
        {
            currentGameState = GameState.Victory;
        }

        public bool IsGameOver()
        {
            return currentGameState == GameState.GameOver;
        }
    }
}

