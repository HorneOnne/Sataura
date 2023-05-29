using System.Collections.Generic;
using UnityEngine;

namespace Sataura
{
    public class UIDisplayCharactersSelection : Singleton<UIDisplayCharactersSelection> 
    {
        [Header("References")]
        [SerializeField] private Transform parent;
        [SerializeField] private UICharacterInformation uiCharacterInformationPrefab;

        // Cached
        private SaveManager saveManager;
        private List<UICharacterInformation> uiCharacterInformations;


        private void Awake()
        {
            saveManager = SaveManager.Instance;
            uiCharacterInformations = new List<UICharacterInformation>();
        }

        private void OnEnable()
        {         
            CreateList();
        }

        private void OnDisable()
        {
            ClearAll();
        }

        public void ClearAll()
        {
            for(int i = 0; i < uiCharacterInformations.Count; i++)
            {
                if(uiCharacterInformations[i] != null)
                    Destroy(uiCharacterInformations[i].gameObject);
            }
            uiCharacterInformations.Clear();
        }

        public void CreateList()
        {
            int size = SaveManager.Instance.charactersData.Count;
            if (size == 0) return;


            for (int i = 0; i < size; i++)
            {
                var uiObject = Instantiate(uiCharacterInformationPrefab, parent);
                uiObject.index = i;
                uiObject.SetCharacterData(SaveManager.Instance.charactersData[i]);
                uiCharacterInformations.Add(uiObject);
            }
        }
    }
}

