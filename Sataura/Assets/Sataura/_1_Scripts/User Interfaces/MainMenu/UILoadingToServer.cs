using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace Sataura
{
    public class UILoadingToServer : SatauraCanvas
    {
        private void OnLoading()
        {
            // Testing
            //StartLoading();
            Loader.LoadNetwork(Loader.Scene._1_CharacterInventoryScene);
            // =======
        }

        private IEnumerator LoadSceneWithCondition()
        {
            Debug.Log("IEnumerator LoadSceneWithCondition");
            // Load the target scene additively
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(Loader.Scene._1_CharacterInventoryScene.ToString(), LoadSceneMode.Single);

            // Disable the scene from being automatically activated
            asyncLoad.allowSceneActivation = false;

            // Wait for the specific condition before activating the loaded scene
            while (!IsConditionMet())
            {

                yield return null;
            }

            // Allow the scene to be activated
            asyncLoad.allowSceneActivation = true;
        }

        private bool IsConditionMet()
        {

            // Implement your specific condition here
            // Return true when the condition is met, or false otherwise
            //return /* Your condition */;

            if (Input.GetKey(KeyCode.E))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void StartLoading()
        {
            StartCoroutine(LoadSceneWithCondition());
        }
    }

}
