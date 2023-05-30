using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Sataura
{
    public class UIPauseCanvas : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Canvas _canvas;
        [SerializeField] private Button _pauseBtn;

        #region Properties
        public Canvas Canvas { get { return _canvas; } }
        #endregion


        private void OnEnable()
        {
            _pauseBtn.onClick.AddListener(PauseBtnHandler);
        }

     

        private void OnDisable()
        {
            _pauseBtn.onClick.RemoveAllListeners();
        }

        private void PauseBtnHandler()
        {
            BackToCharacterSelectionScene();
        }

        private void BackToCharacterSelectionScene()
        {
            Loader.LoadNetwork(Loader.Scene._1_CharacterInventoryScene);
        }
    }
}


