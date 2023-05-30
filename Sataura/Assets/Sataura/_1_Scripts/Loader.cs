using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sataura
{
    public static class Loader
    {
        public enum Scene
        {
            _0_MainMenuScene,
            _1_CharacterInventoryScene,
            _2_GameScene,
            
        }

        private static Scene targetScene;

        public static void Load(Scene targetScene)
        {
            Loader.targetScene = targetScene;
            SceneManager.LoadScene(Loader.targetScene.ToString());
        }

        public static void LoadNetwork(Scene targetScene)
        {
            var status = NetworkManager.Singleton.SceneManager.LoadScene(targetScene.ToString(), LoadSceneMode.Single);
            Debug.Log($"status: {status}");
        }

        public static void LoaderCallback()
        {
            SceneManager.LoadScene(targetScene.ToString());
        }
    }

}
