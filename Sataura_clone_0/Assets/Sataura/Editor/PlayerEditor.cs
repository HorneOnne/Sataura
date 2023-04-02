using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;

namespace Sataura
{
    [CustomEditor(typeof(Player))]
    public class PlayerEditor : Editor
    {
        private SerializedProperty playerDataProperty;
        private SerializedProperty characterDataProperty;


        private SerializedProperty handleItemProperty;
        private SerializedProperty handleMovementProperty;


        private SerializedProperty canUseItemProperty;
        private SerializedProperty handHoldItemProperty;

   
        private SerializedProperty playerInventoryProperty;
        private SerializedProperty playerInGameInventoryProperty;
        private SerializedProperty itemInHandyProperty;
        private SerializedProperty playerMovementyProperty;
        private SerializedProperty playerInputHandleryProperty;
        private SerializedProperty playerUseItemProperty;

        private void OnEnable()
        {
            handleItemProperty = serializedObject.FindProperty("handleItem");
            handleMovementProperty = serializedObject.FindProperty("handleMovement");

            playerDataProperty = serializedObject.FindProperty("playerData");
            characterDataProperty = serializedObject.FindProperty("characterData");

            canUseItemProperty = serializedObject.FindProperty("canUseItem");
            handHoldItemProperty = serializedObject.FindProperty("handHoldItemToSpawn");

            
            playerInventoryProperty = serializedObject.FindProperty("playerInventory");
            playerInGameInventoryProperty = serializedObject.FindProperty("playerInGameInventory");
            itemInHandyProperty = serializedObject.FindProperty("itemInHand");
            playerMovementyProperty = serializedObject.FindProperty("playerMovement");
            playerInputHandleryProperty = serializedObject.FindProperty("playerInputHandler");
            playerUseItemProperty = serializedObject.FindProperty("playerUseItem");
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
            EditorGUILayout.PropertyField(characterDataProperty);

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
                EditorGUILayout.PropertyField(playerUseItemProperty);
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

            serializedObject.ApplyModifiedProperties();
        }

    }

}
