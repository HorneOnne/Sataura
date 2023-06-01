using UnityEngine;
using Cinemachine;

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


       

        public void Exit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        
    }
}