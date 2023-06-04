using UnityEngine;
using System.Collections.Generic;
using TMPro;

namespace Sataura
{
    public class ItemDesc : MonoBehaviour
    {
        public static ItemDesc Instance { get; private set; }

        [Header("References")]
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _descTextPrefab;
        private List<TextMeshProUGUI> _descList = new List<TextMeshProUGUI>();

        private void Awake()
        {
            Instance = this;
        }

        public void CreateDesc(ItemData itemData)
        {
            _nameText.text = $"{itemData.itemName}";

            ClearOldDesc();
            CreateNewDesc(itemData);
        }

        private void ClearOldDesc()
        {
            if (_descList.Count <= 0) return;

            for(int i = 0; i < _descList.Count; i++) 
            {
                Destroy(_descList[i].gameObject);
            }
            _descList.Clear();
        }

        private void CreateNewDesc(ItemData itemData)
        {
            string itemDesc = itemData.hoverDescription;
            if (string.IsNullOrEmpty(itemDesc)) return;
            string[] itemDescArray = itemDesc.Split(new char[] { '~' }, System.StringSplitOptions.RemoveEmptyEntries);

            for(int i = 0; i < itemDescArray.Length; i++)
            {
                _descList.Add(Instantiate(_descTextPrefab, this.transform));
                _descList[i].text = itemDescArray[i];           
            }
        }
    }
}
