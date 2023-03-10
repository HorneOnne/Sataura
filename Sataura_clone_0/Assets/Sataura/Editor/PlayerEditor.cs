using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;

namespace Sataura
{
    [CustomEditor(typeof(Player))]
    public class PlayerEditor : Editor
    {
        private SerializedProperty handleItemProperty;
        private SerializedProperty handleMovementProperty;
        private SerializedProperty handleEquipmentProperty;


        private SerializedProperty playerDataProperty;
        private SerializedProperty canUseItemProperty;
        private SerializedProperty handHoldItemProperty;

        
        private SerializedProperty playerInventoryProperty;
        private SerializedProperty playerInGameInventoryProperty;
        private SerializedProperty itemInHandyProperty;
        private SerializedProperty playerMovementyProperty;
        private SerializedProperty playerInputHandleryProperty;
        private SerializedProperty playerEquipmentyProperty;

        private void OnEnable()
        {
            handleItemProperty = serializedObject.FindProperty("handleItem");
            handleMovementProperty = serializedObject.FindProperty("handleMovement");
            handleEquipmentProperty = serializedObject.FindProperty("handleEquipment");


            playerDataProperty = serializedObject.FindProperty("playerData");
            canUseItemProperty = serializedObject.FindProperty("canUseItem");
            handHoldItemProperty = serializedObject.FindProperty("handHoldItemToSpawn");

            
            playerInventoryProperty = serializedObject.FindProperty("playerInventory");
            playerInGameInventoryProperty = serializedObject.FindProperty("playerInGameInventory");
            itemInHandyProperty = serializedObject.FindProperty("itemInHand");
            playerMovementyProperty = serializedObject.FindProperty("playerMovement");
            playerInputHandleryProperty = serializedObject.FindProperty("playerInputHandler");
            playerEquipmentyProperty = serializedObject.FindProperty("playerEquipment");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Show script reference
            MonoScript script = MonoScript.FromMonoBehaviour((Player)target);
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField("Script", script, typeof(MonoScript), false);
            EditorGUI.EndDisabledGroup();



            

            EditorGUILayout.PropertyField(playerDataProperty);
            EditorGUILayout.PropertyField(playerInputHandleryProperty);

            EditorGUILayout.PropertyField(handleItemProperty);          
            if (handleItemProperty.boolValue)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(15);
                EditorGUILayout.PropertyField(playerInventoryProperty);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(15);
                EditorGUILayout.PropertyField(playerInGameInventoryProperty);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(15);
                EditorGUILayout.PropertyField(itemInHandyProperty);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(15);
                EditorGUILayout.PropertyField(canUseItemProperty);
                GUILayout.EndHorizontal();

                if (canUseItemProperty.boolValue)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(30);
                    EditorGUILayout.PropertyField(handHoldItemProperty);
                    GUILayout.EndHorizontal();
                }
                
            }


            EditorGUILayout.PropertyField(handleMovementProperty);
            if(handleMovementProperty.boolValue)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(15);
                EditorGUILayout.PropertyField(playerMovementyProperty);
                GUILayout.EndHorizontal();
            }


            EditorGUILayout.PropertyField(handleEquipmentProperty);
            if(handleEquipmentProperty.boolValue)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(15);
                EditorGUILayout.PropertyField(playerEquipmentyProperty);
                GUILayout.EndHorizontal();
            }

            serializedObject.ApplyModifiedProperties();
        }

    }

}
