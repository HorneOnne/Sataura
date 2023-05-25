using JetBrains.Annotations;
using System.Collections;
using UnityEngine;

namespace Sataura
{
    /// <summary>
    /// The UIManager class manages all the UI canvas in the game and provides easy access to them through properties.
    /// </summary>
    public class UIManager : Singleton<UIManager>
    {
        [Header("Canvas")]
        public GameObject playerInGameInventoryCanvas;
        public GameObject playerInformationCanvas;
        public Canvas bossHealthBarCanvas;
        public GameObject menuCanvas;
        public Canvas victoryCanvas;

        [Header("Panel")]
        public GameObject upgradeItemSkillPanel;


        private void OnEnable()
        {
            IngameInformationManager.OnPlayerLevelUp += OpenUpgradeItemSkillPanel;
        }

        private void OnDisable() 
        {
            IngameInformationManager.OnPlayerLevelUp -= OpenUpgradeItemSkillPanel;
        }


        private void Awake()
        {
            // Activate all UI canvases on awake.
            if(playerInGameInventoryCanvas != null)
                playerInGameInventoryCanvas.SetActive(true);


            if (menuCanvas != null)
                menuCanvas.SetActive(true);

            if (bossHealthBarCanvas != null)
                bossHealthBarCanvas.enabled = false;

            if (victoryCanvas != null)
                victoryCanvas.enabled = false;
        }



        private void OpenUpgradeItemSkillPanel()
        {
            StartCoroutine(WaitOpenUpgradeItemSkillPanel(2f));
        }

        private IEnumerator WaitOpenUpgradeItemSkillPanel(float time)
        {
            yield return new WaitForSeconds(time);
            upgradeItemSkillPanel.SetActive(true);
            UIIngameInformationManager.Instance.FakeFullExpSliderWhenLevelUp();
            UIIngameInformationManager.Instance.GenerateUpgradeList();
            Time.timeScale = 0.0f;
        }


        public void CloseUpgradeItemSkillPanel()
        {
            upgradeItemSkillPanel.SetActive(false);        
            Time.timeScale = 1.0f;
        }

    }
}