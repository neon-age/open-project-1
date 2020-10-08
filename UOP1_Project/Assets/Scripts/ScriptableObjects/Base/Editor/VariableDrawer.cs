using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ScriptableObjects
{
    [CustomPropertyDrawer(typeof(Variable<>), true)]
    internal class VariableDrawer : PropertyDrawer
    {
        private static GUIStyle variableLabelStyle;
        private static GUIContent emptyContent = new GUIContent(" ");
        private static Dictionary<string, SerializedObject> scriptableObjects = new Dictionary<string, SerializedObject>();
        
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            if (variableLabelStyle == null)
            {
                variableLabelStyle = new GUIStyle(EditorStyles.label)
                {
                    alignment = TextAnchor.MiddleRight
                };
            }
            
            EditorGUI.BeginProperty(rect, label, property);

            var position = EditorGUI.PrefixLabel(rect, GUIUtility.GetControlID(FocusType.Passive), label);
            
            var reference = property.objectReferenceValue;
            if (!reference)
            {
                var attachRect = new Rect(position) { width = 60 };
                position.width -= attachRect.width;
                attachRect.x += position.width;
                
                EditorGUI.PropertyField(position, property, GUIContent.none);
                if (GUI.Button(attachRect, "Attach"))
                {
                    var variableInstance = ScriptableObject.CreateInstance(fieldInfo.FieldType);
                    variableInstance.name = property.displayName;

                    AttachObjectToAsset(variableInstance, property.serializedObject.targetObject);
                    
                    property.objectReferenceValue = variableInstance;
                    
                    Selection.activeObject = null;
                    Selection.activeObject = property.serializedObject.targetObject;
                }
            }
            else
            {
                TryGetPropertySerializedObject(property, out var serializedObject);
                DrawScriptableObject(rect, serializedObject);
            }

            EditorGUI.EndProperty();
        }

        /// Simplified and cleaned up version of https://gist.github.com/tomkail/ba4136e6aa990f4dc94e0d39ec6a058c
        
        public void DrawScriptableObject(Rect rect, SerializedObject serializedObject)
        {
            // A little hackish way of making label aligned to right
            var labelPadding = EditorStyles.label.padding;
            var fontSize = EditorStyles.label.fontSize;
            EditorStyles.label.alignment = TextAnchor.MiddleRight;
            EditorStyles.label.padding = new RectOffset(0, 4, 0, 0);
            
            var iterator = serializedObject.GetIterator();
            if (iterator.NextVisible(true)) 
            {
                do
                {
                    var name = iterator.name;
                    if(name == "m_Script") 
                        continue;
                    
                    var subProp = serializedObject.FindProperty(iterator.name);
                    rect.height = EditorGUI.GetPropertyHeight(subProp, GUIContent.none, true) + EditorGUIUtility.standardVerticalSpacing;

                    if(subProp.displayName == "Value")
                        EditorGUI.PropertyField(rect, subProp, emptyContent);
                    else
                        EditorGUI.PropertyField(rect, subProp);
                    
                    rect.y += rect.height;
                }
                while (iterator.NextVisible(false));
            }
            
            EditorStyles.label.alignment = TextAnchor.MiddleLeft;
            EditorStyles.label.padding = labelPadding;
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (TryGetPropertySerializedObject(property, out var serializedObject))
                return GetSerializedObjectHeight(serializedObject, label);

            return EditorGUIUtility.singleLineHeight;
        }

        
        private bool TryGetPropertySerializedObject(SerializedProperty property, out SerializedObject serializedObject)
        {
            var reference = property.objectReferenceValue;
            serializedObject = null;

            if (!reference)
                return false;

            var propertyPath = property.propertyPath;
            if (!scriptableObjects.TryGetValue(property.propertyPath, out serializedObject))
            {
                serializedObject = new SerializedObject(property.objectReferenceValue);
                scriptableObjects.Add(propertyPath, serializedObject);
                OnCreateSerializedObject(property.objectReferenceValue, serializedObject.targetObject);
            }

            return true;
        }

        private void OnCreateSerializedObject(Object reference, Object asset)
        {
            if (AssetDatabase.GetAssetPath(reference) == "")
                AttachObjectToAsset(reference, asset);
        }

        private void AttachObjectToAsset(Object objectToAdd, Object asset)
        {
            AssetDatabase.AddObjectToAsset(objectToAdd, asset);
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(asset));
        }
        
        private float GetSerializedObjectHeight(SerializedObject serializedObject, GUIContent label) 
        {
            var totalHeight = 0f;
            
            if(serializedObject == null)
                return EditorGUIUtility.singleLineHeight;

            //if (!property.isExpanded) 
            //    return totalHeight;
            
            var iterator = serializedObject.GetIterator();
            if (iterator.NextVisible(true)) 
            {
                do
                {
                    var name = iterator.name;
                    if(name == "m_Script") 
                        continue;
                    
                    var subProp = serializedObject.FindProperty(name);
                    
                    var height = EditorGUI.GetPropertyHeight(subProp, GUIContent.none, true) + EditorGUIUtility.standardVerticalSpacing;
                    totalHeight += height;
                }
                while (iterator.NextVisible(false));
            }

            totalHeight += EditorGUIUtility.standardVerticalSpacing;
            return totalHeight;
        }
    }
}