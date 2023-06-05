using UnityEngine;
using Unity.Netcode;

namespace Sataura
{
    public class UIGameNavigation : SatauraCanvas
    {
        [SerializeField] private SatauraButton gameBtn;
        [SerializeField] private SatauraButton backBtn;

        private void Start()
        {
            gameBtn.Button.onClick.AddListener(() =>
            {
                NetworkManager.Singleton.StartHost();

                Loader.LoadNetwork(Loader.Scene._2_GameScene);
            });

            backBtn.Button.onClick.AddListener(() =>
            {
                SaveManager.Instance.SaveCharacterData();
                DestroyAllDontDestroyOnLoadObjects();
                Loader.Load(Loader.Scene._0_MainMenuScene);
            });
        }

        private void OnDestroy()
        {
            gameBtn.Button.onClick.RemoveAllListeners();
            backBtn.Button.onClick.RemoveAllListeners();
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
