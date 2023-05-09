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
        public GameObject creativeInventoryCanvas;
        public GameObject playerInformationCanvas;
        public GameObject chestInventoryCanvas;
        public GameObject menuCanvas;

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

            /*if(creativeInventoryCanvas != null)
                creativeInventoryCanvas.SetActive(true);*/

            if (menuCanvas != null)
                menuCanvas.SetActive(true);     
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
            Time.timeScale = 0.0f;
        }


        public void CloseUpgradeItemSkillPanel()
        {
            upgradeItemSkillPanel.SetActive(false);        
            Time.timeScale = 1.0f;
        }

    }
}