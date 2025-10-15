using System;
using System.Linq;
using System.Reflection;
using Servable.Runtime;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Servable.Editor
{
    public abstract class ObservableReferenceDrawer: PropertyDrawer
    {
        private const BindingFlags BindingAttr = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public;
  
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var positionViewModel = new Rect(position.x, position.y, position.width - 144, position.height);
            var positionDropdown = new Rect(position.x + position.width - 142, position.y, 142, position.height);

            var viewModelProperty = property.FindPropertyRelative("viewModel");
            if (viewModelProperty.objectReferenceValue is GameObject go)
            {
                viewModelProperty.objectReferenceValue = go.GetComponent<IModel>() as Object;
                viewModelProperty.serializedObject.ApplyModifiedProperties();
                return;            
            } 
        
            EditorGUI.ObjectField(positionViewModel, viewModelProperty, typeof(Object), GUIContent.none);
        
            var viewModel = viewModelProperty.objectReferenceValue;
            if (!viewModel)
            {
                var component = property.serializedObject.targetObject as Component;
                if (!component) return;
                viewModelProperty.objectReferenceValue = component.GetComponentInParent<IModel>() as Object;
                viewModelProperty.serializedObject.ApplyModifiedProperties();
                return;            
            }
        
            var icon = EditorGUIUtility.IconContent("Prefab On Icon");
            var positionIcon = new Rect(position.x - position.height, position.y, position.height, position.height);
        
            var backupColor = GUI.color;
            GUI.color = HierarchyTags.ComputeTypeHashedColor(viewModel.GetType());
            EditorGUI.DropdownButton(positionIcon, icon, FocusType.Passive, GUIStyle.none);
            GUI.color = backupColor;
        
            var serializedPropertyName = property.FindPropertyRelative("property");
            var propertyName = serializedPropertyName.stringValue;
        
            var infos = viewModel.GetType().GetProperties(BindingAttr);
            var content = new GUIContent($"!!{propertyName}!!");
            var info = infos.FirstOrDefault(i => i.Name == propertyName);
            if (info != null)
            {
                content = new GUIContent($"{propertyName}");
            }        
        
            if (EditorGUI.DropdownButton(positionDropdown, content, FocusType.Passive))
            {
                var propertyType = fieldInfo.FieldType;
                var propertyGenArg = propertyType.GenericTypeArguments.FirstOrDefault();

                var menu = new GenericMenu();
                foreach (var item in infos)
                {
                    var type = item.GetGetMethod(true).ReturnType;
                    if (!CheckPropertyType(type, propertyGenArg)) continue;
                    menu.AddItem(new GUIContent(item.Name), false, () =>
                    {
                        serializedPropertyName.stringValue = item.Name;
                        property.serializedObject.ApplyModifiedProperties();
                    });
                }
                menu.ShowAsContext();
            }
        }

        public abstract bool CheckPropertyType(Type type, Type propertyGenArg);
    }
}