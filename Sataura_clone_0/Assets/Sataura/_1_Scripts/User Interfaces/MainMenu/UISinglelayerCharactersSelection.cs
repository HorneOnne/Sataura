using System.Collections.Generic;
using UnityEngine;

namespace Sataura
{
    public class UISinglelayerCharactersSelection : SatauraCanvas
    {
        [Header("References")]
        [SerializeField] private Transform parent;
        [SerializeField] private UICharacterInformation _uiCharacterInfoPrefab;
        [SerializeField] private SatauraButton createBtn;
        [SerializeField] private SatauraButton backBtn;


        // Cached
        private SaveManager _saveManager;
        private List<UICharacterInformation> _listUICharacterInfomation;


        private void Awake()
        {
            _saveManager = SaveManager.Instance;
            _listUICharacterInfomation = new List<UICharacterInformation>();
        }

        private void OnEnable()
        {         
            CreateList();
        }

        private void OnDisable()
        {
            ClearAll();
        }

        private void Start()
        {
            createBtn.Button.onClick.AddListener(() =>
            {
                MainMenuUIManager.Instance.CloseAll();
                MainMenuUIManager.Instance.DisplayCreateNewSingleplayerCharacter(true);
            });

            backBtn.Button.onClick.AddListener(() =>
            {
                MainMenuUIManager.Instance.CloseAll();
                MainMenuUIManager.Instance.BackToMainMenu();
            });
        }

        private void OnDestroy()
        {
            createBtn.Button.onClick.RemoveAllListeners();
            backBtn.Button.onClick.RemoveAllListeners();
        }


        public override void DisplayCanvas(bool isDisplay)
        {
            base.DisplayCanvas(isDisplay);

            if(isDisplay)
                CreateList();
            else
                ClearAll();
        }

        public void ClearAll()
        {
            for(int i = 0; i < _listUICharacterInfomation.Count; i++)
            {
                if(_listUICharacterInfomation[i] != null)
                    Destroy(_listUICharacterInfomation[i].gameObject);
            }
            _listUICharacterInfomation.Clear();
        }

        public void CreateList()
        {
            int size = SaveManager.Instance.charactersData.Count;
            if (size == 0) return;


            for (int i = 0; i < size; i++)
            {
                var uiObject = Instantiate(_uiCharacterInfoPrefab, parent);
                uiObject.index = i;
                uiObject.SetCharacterData(SaveManager.Instance.charactersData[i]);
                _listUICharacterInfomation.Add(uiObject);
            }
        }
    }
}

