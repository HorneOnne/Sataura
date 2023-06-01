using System.Collections.Generic;
using UnityEngine;

namespace Sataura
{
    public class UIMultiplayerCharactersSelection : SatauraCanvas
    {
        [Header("References")]
        [SerializeField] private Transform parent;
        [SerializeField] private UICharacterInformation _uiCharacterInfoPrefab;

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

        public override void DisplayCanvas(bool isDisplay)
        {
            base.DisplayCanvas(isDisplay);

            if (isDisplay)
                CreateList();
            else
                ClearAll();
        }

        public void ClearAll()
        {
            for (int i = 0; i < _listUICharacterInfomation.Count; i++)
            {
                if (_listUICharacterInfomation[i] != null)
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
                var uiCharacterInformation = Instantiate(_uiCharacterInfoPrefab, parent);
                uiCharacterInformation.index = i;
                uiCharacterInformation.SetCharacterData(SaveManager.Instance.charactersData[i]);
                _listUICharacterInfomation.Add(uiCharacterInformation);
            }
        }
    }
}

