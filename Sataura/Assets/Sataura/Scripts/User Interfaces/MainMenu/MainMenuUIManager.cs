using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sataura
{
    public class MainMenuUIManager : Singleton<MainMenuUIManager>
    {
        public GameObject mainMenuCanvas;
        public GameObject playerSelectionCanvas;
        public GameObject createNewCharacterCanvas;

        private void Start()
        {
            
            if (mainMenuCanvas != null)
                mainMenuCanvas.SetActive(true);

            // InActivate all UI canvases on start (except main menu).
            if (playerSelectionCanvas != null)
                playerSelectionCanvas.SetActive(false);

            if (createNewCharacterCanvas != null)
                createNewCharacterCanvas.SetActive(false);

        }


        public void SetActiveMainMenuCanvas(bool isActive)
        {
            mainMenuCanvas.SetActive(isActive);
        }
 

        public void SetActivePlayerSelectionCanvas(bool isActive)
        {
            playerSelectionCanvas.SetActive(isActive);
        }


        public void SetActiveCreateNewCharacterCanvas(bool isActive)
        {
            createNewCharacterCanvas.SetActive(isActive);
        }
    }

}

