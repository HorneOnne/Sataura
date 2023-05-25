using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace Sataura
{
    public static class Utilities
    {
        public static void AddEvent(GameObject obj, EventTriggerType type, UnityAction<BaseEventData> action)
        {
            EventTrigger trigger = obj.GetComponent<EventTrigger>();
            EventTrigger.Entry eventTrigger = new EventTrigger.Entry();
            eventTrigger.eventID = type;
            eventTrigger.callback.AddListener(action);
            trigger.triggers.Add(eventTrigger);
        }



        public static Item InstantiateItemObject(ItemSlot itemSlot, Transform parent = null)
        {
            GameObject returnGameObject = null;

            var itemPrefab = GameDataManager.Instance.GetItemPrefab(itemSlot.ItemData.itemType);
            if (itemPrefab != null)
            {
                returnGameObject = MonoBehaviour.Instantiate(itemPrefab, parent);
                returnGameObject.GetComponent<Item>().SetData(itemSlot);
  
            }
            else
            {
                throw new System.Exception($"Not found prefab name {itemSlot.ItemData.itemType} in GameDataManager.cs");
            }

            return returnGameObject.GetComponent<Item>();
        }

        public static Item InstantiateItemNetworkObject(int itemID, int itemQuantity, Transform parent = null)
        {
            GameObject returnGameObject = null;
            var itemData = GameDataManager.Instance.GetItemData(itemID);
            
            var itemPrefab = GameDataManager.Instance.GetItemPrefab(itemData.itemType);
            if (itemPrefab != null)
            {
                returnGameObject = MonoBehaviour.Instantiate(itemPrefab, parent);
                returnGameObject.GetComponent<Item>().SetData(new ItemSlot(itemData, itemQuantity));

            }
            else
            {
                throw new System.Exception($"Not found prefab name {itemData.itemType} in GameDataManager.cs");
            }

            return returnGameObject.GetComponent<Item>();
        }

        public static void RotateObjectTowardMouse2D(Vector3 mousePos, Transform objectTransform, float offsetZAngle)
        {
            Vector3 spritePos = objectTransform.position;
            //Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f; // Ensure the z-coordinate is 0 to keep the mouse and sprite on the same plane

            // Calculate the difference between the sprite's position and the mouse position
            float dx = mousePos.x - spritePos.x;
            float dy = mousePos.y - spritePos.y;

            // Calculate the angle between the sprite's position and the mouse position
            float angle = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;

            // Set the rotation of the sprite to the calculated angle
            objectTransform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle + offsetZAngle));
        }

        public static void RotateObjectTowardDirection2D(Vector2 direction, Transform objectTransform, float offsetZAngle)
        {
            if (direction.magnitude > 0.1f)
            {
                // Calculate the angle between the sprite's position and the analog stick direction
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                // Calculate the target rotation quaternion
                Quaternion targetRotation = Quaternion.Euler(new Vector3(0f, 0f, angle + offsetZAngle));

                // Lerp the current rotation toward the target rotation over time
                objectTransform.rotation = Quaternion.Lerp(objectTransform.rotation, targetRotation, 90 * Time.deltaTime);
            }
        }

        public static ItemData GetUpgradeItemData(ItemData itemData)
        {
            return itemData.upgradeRecipe.outputItemSlot.itemData;
        }

        public static ItemSlot GetUpgradeItemSlot(ItemData itemData)
        {
            return new ItemSlot(itemData.upgradeRecipe.outputItemSlot.itemData,1);
        }

        public static Color HexToColor(string hexCode)
        {
            Color color;
            if (ColorUtility.TryParseHtmlString(hexCode, out color))
            {
                return color;
            }
            else
            {
                // If the conversion fails, return white color
                return Color.white;
            }
        }

        public static InventoryData ConvertInventoryDataStructToInventoryData(InventoryStruct inventoryDataStruct)
        {
            InventoryData inventoryData = ScriptableObject.CreateInstance<InventoryData>();
            inventoryData.itemSlots = new List<ItemSlot>();

            for (int i = 0; i < inventoryDataStruct.itemDatas.Count; i++)
            {
                int id = inventoryDataStruct.itemDatas[i].itemID;
                int quantity = inventoryDataStruct.itemDatas[i].itemQuantity;
                inventoryData.itemSlots.Add(new ItemSlot(GameDataManager.Instance.GetItemData(id), quantity));
            }

            return inventoryData;
        }

        public static CharacterMovementData ConvertStructToCharacterMovementData(CharacterMovementDataStruct characterMovementDataStruct)
        {
            var characterMovementData = ScriptableObject.CreateInstance<CharacterMovementData>();

            characterMovementData.movementForceInAir = characterMovementDataStruct.movementForceInAir;
            characterMovementData.airDragMultiplier = characterMovementDataStruct.airDragMultiplier;
            characterMovementData.hangTime = characterMovementDataStruct.hangTime;
            characterMovementData.jumpBufferLength = characterMovementDataStruct.jumpBufferLength;
            characterMovementData.fallMultiplier = characterMovementDataStruct.fallMultiplier;
            characterMovementData.lowMultiplier = characterMovementDataStruct.lowMultiplier;
            characterMovementData.maxMovementSpeed = characterMovementDataStruct.maxMovementSpeed;
            characterMovementData.maxJumpVelocity = characterMovementDataStruct.maxJumpVelocity;
            characterMovementData.maxFallVelocity = characterMovementDataStruct.maxFallVelocity;

            return characterMovementData;
        }
    }
}

