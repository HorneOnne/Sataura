using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sataura
{
    public class UIMenuPanel : MonoBehaviour
    {

        private void Start()
        {
            UIManager.Instance.menuCanvas.SetActive(false);
        }

        public void ReloadScene()
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentSceneIndex, LoadSceneMode.Single);
        }

        public void Save()
        {
            SaveManager.Instance.Save();
        }

        public void Load()
        {
            SaveManager.Instance.Load();
        }
    }

}
