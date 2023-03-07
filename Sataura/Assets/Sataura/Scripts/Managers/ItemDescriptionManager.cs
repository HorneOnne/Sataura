using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Sataura
{
    public class ItemDescriptionManager : Singleton<ItemDescriptionManager>
    {
        private const int childHeight = 40;    
        private RectTransform rect;
        private ItemData itemData;
        [SerializeField] private Player player;
        private ItemInHand itemInHand;


        private const char itemNamePrefix = '~';
        private const char itemDataPrefix = '-';
        private const char itemExtraPrefix = '+';
        private const int nameFontSize = 20;
        private const int otherFontSize = 20;

        private List<TextMeshProUGUI> descTexts = new List<TextMeshProUGUI>();
        Vector2 mousePosition;
        Vector2 worldPosition;

        void Start()
        {
            rect = GetComponent<RectTransform>();
            itemInHand = player.ItemInHand;
        }


        private void ExpandPanel(int childHeight, int childCount)
        {
            float calculatedHeight = childHeight * childCount;
            RectTransform rectTransform = GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, calculatedHeight);
        }

        public void SetItemData(ItemData itemData)
        {
            this.itemData = itemData;
        }



        private float updateFrequency = 0.02f; // Limit the update frequency to 10 times per second
        private float timeSinceLastUpdate = 0f;


        private void Update()
        {
            if (itemData == null) return;
            timeSinceLastUpdate += Time.deltaTime;

            if (timeSinceLastUpdate >= updateFrequency)
            {
                // Your code that needs to run at the limited update frequency goes here
                // ...
                timeSinceLastUpdate -= updateFrequency;
                TransformToMousePosition();

            }


            if(itemInHand.HasItem())
            {
                Hide();
            }
        }



        private void ItemDescriptionHandle()
        {
            if (itemData == null) return;

            string[] descString = itemData.description.Split('\n', StringSplitOptions.RemoveEmptyEntries);
 
            for (int i = 0; i < descTexts.Count; i++)
            {
                ItemDescTextSpawner.Instance.Pool.Release(descTexts[i].gameObject);
            }
            descTexts.Clear();

            ExpandPanel(childHeight, descString.Length);

            for (int i = 0; i < descString.Length; i++)
            {
                var obj = ItemDescTextSpawner.Instance.Pool.Get().GetComponent<TextMeshProUGUI>();
 
                obj.text = RemoveFirstCharIfSpecial(descString[i]);
                obj.transform.SetParent(this.transform);
                obj.transform.SetAsLastSibling();
                obj.GetComponent<RectTransform>().localScale = Vector3.one;

                // Try to parse the hexadecimal color code
                Color color;
                if (ColorUtility.TryParseHtmlString("#e9898a", out color))
                {
                    //Debug.Log("Color: " + color);
                }
                else
                {
                    Debug.LogError("Failed to parse hexadecimal color code: " + "#e9898a");
                }

                switch (descString[i][0])
                {
                    case itemNamePrefix:
                        //obj.color = Color.red;
                        obj.color = color;
                        obj.fontStyle |= FontStyles.Bold;
                        obj.fontSize = nameFontSize;
                        break;
                    case itemDataPrefix:
                        obj.color = Color.white;
                        obj.fontStyle &= ~FontStyles.Bold;
                        obj.fontSize = otherFontSize;
                        break;
                    case itemExtraPrefix:
                        obj.color = Color.green;
                        obj.fontStyle &= ~FontStyles.Bold;
                        obj.fontSize = otherFontSize;
                        break;
                    default:
                        Debug.LogWarning($"Not found item description prefix {descString[i][0]} definition.");
                        break;
                }

                descTexts.Add(obj);
            }


        }


        public void Show()
        {          
            ItemDescriptionHandle();
            TransformToMousePosition();
            this.gameObject.SetActive(true);
        }

        public void Hide()
        {
            for (int i = 0; i < descTexts.Count; i++)
            {
                ItemDescTextSpawner.Instance.Pool.Release(descTexts[i].gameObject);
            }
            descTexts.Clear();


            this.gameObject.SetActive(false);

        }

        public void TransformToMousePosition()
        {
            TransformToMousePosition(rect, 7f, -7f);
        }


        

        private void TransformToMousePosition(RectTransform rectTransform, float offsetX, float offsetY)
        {
            mousePosition = Input.mousePosition;
            worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            rectTransform.position = worldPosition + new Vector2(offsetX, offsetY);
        }

      
        private string RemoveFirstCharIfSpecial(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            char firstChar = input[0];
            if (firstChar == '~' || firstChar == '-' || firstChar == '.' || firstChar == '+')
            {
                return input.Substring(1);
            }

            return input;
        }

    }
}