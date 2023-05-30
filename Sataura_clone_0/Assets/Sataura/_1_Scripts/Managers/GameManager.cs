using UnityEngine;
using Cinemachine;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine.SceneManagement;

namespace Sataura
{
    public class GameManager : Singleton<GameManager> 
    {
        [SerializeField] private int limitFps = 144;
        [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;


        public CinemachineVirtualCamera CinemachineVirtualCamera { get => cinemachineVirtualCamera; }


        private void Awake()
        {
            Application.targetFrameRate = limitFps;
            
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                UIManager.Instance.TogglePause();
            }
        }



        public void GotoGameScene()
        {
            Loader.LoadNetwork(Loader.Scene._2_GameScene);
        }

        public void BackToMainMenu()
        {          
            SaveManager.Instance.SaveCharacterData();
            NetworkManager.Singleton.Shutdown();
            DestroyAllDontDestroyOnLoadObjects();
            SceneManager.LoadScene(Loader.Scene._0_MainMenuScene.ToString());         
        }

        



        public void Exit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public void DestroyAllDontDestroyOnLoadObjects()
        {
            var go = new GameObject("Sacrificial Lamb");
            DontDestroyOnLoad(go);

            foreach (var root in go.scene.GetRootGameObjects())
            {
                if(root.GetComponent<NetworkManager>() != null)
                {
                    //root.GetComponent<NetworkManager>().Shutdown();
                    //Destroy(root);

                    //NetworkManager.Singleton.Shutdown();
                    if (NetworkManager.Singleton != null)
                    {
                        //Destroy the extra instance
                        Destroy(NetworkManager.Singleton.gameObject);
                    }
                    //Destroy(root);
                }
                else
                    Destroy(root);

            }              
        }
    }
}