using System.Collections;
using UnityEngine;

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

       

        #region Properties
        public int Level { get { return level; } }  
        public int Experience { get { return experience; } }  
        public int ExperienceToNextLevel { get { return experienceToNextLevel; } }  
        #endregion


        private void Awake()
        {
            level = 0;
            experience = 0;
            experienceToNextLevel = initialexperienceToNextLevelValue;
        }

        // Call this method whenever the character gains experience points
        public void GainExperience(int amount)
        {
            experience += amount;
            if (experience >= experienceToNextLevel)
            {
                Debug.Log($"lv: {level}\t exp: {experience} \t expToNextLv: {experienceToNextLevel}");
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
    }
}

