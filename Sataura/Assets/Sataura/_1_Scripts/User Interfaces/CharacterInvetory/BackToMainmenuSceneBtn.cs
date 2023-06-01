using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

namespace Sataura
{
    public class BackToMainmenuSceneBtn : SatauraButton
    {
        public override void OnClick()
        {
            base.OnClick();

            SaveManager.Instance.SaveCharacterData();
            NetworkManager.Singleton.Shutdown();
            DestroyAllDontDestroyOnLoadObjects();
            SceneManager.LoadScene(Loader.Scene._0_MainMenuScene.ToString());
        }

        private void DestroyAllDontDestroyOnLoadObjects()
        {
            var go = new GameObject("Sacrificial Lamb");
            DontDestroyOnLoad(go);

            foreach (var root in go.scene.GetRootGameObjects())
            {
                if (root.GetComponent<NetworkManager>() != null)
                {
                    if (NetworkManager.Singleton != null)
                    {
                        //Destroy the extra instance
                        Destroy(NetworkManager.Singleton.gameObject);
                    }
                }
                else
                    Destroy(root);

            }
        }
    }
}
