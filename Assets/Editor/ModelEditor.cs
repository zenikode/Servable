using System;
using System.Linq;
using System.Reflection;
using Servable.Runtime;
using Servable.Runtime.Bindings;
using Servable.Runtime.ObservableProperty;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Servable.Editor
{
    [CustomEditor(typeof(UnityEngine.Object), true)]
    [CanEditMultipleObjects]
    public class ModelEditor: UnityEditor.Editor
    {
        private const BindingFlags BindingAttr = BindingFlags.Instance | BindingFlags.Public;
        public override void OnInspectorGUI()
        {
            ListData();
            ListCommands();
            ListCommandsWithArg();
            ListInvalidBindings();
            DrawDefaultInspector();
            if (target is ModelBehaviour viewModel)
                if(!PrefabUtility.IsPartOfPrefabInstance(target))
                    ComponentUtility.MoveComponentUp(viewModel);
        }

        private void ListInvalidBindings()
        {
            if (target is ModelBehaviour viewModel)
            {
                var bindings = viewModel.GetComponentsInChildren<ABinding>();
                foreach (dynamic binding in bindings)
                {
                    try
                    {
                        if (!binding.IsValid())
                        { 
                            var color = GUI.color;
                            GUI.color = Color.red;
                            GUILayout.BeginHorizontal(EditorStyles.helpBox);
                            GUILayout.Label($"{binding.GetType().Name} on {binding.name}");
                            GUILayout.FlexibleSpace();
                            if (GUILayout.Button("FIX", GUILayout.Width(50)))
                            {
                                Selection.activeObject = binding;
                                EditorGUIUtility.PingObject(binding);
                            }
                            GUILayout.EndHorizontal();
                            GUI.color = color;
                        }
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }
            }
        }
    
        public bool DisplayData { get; set; } = true;
        private void ListData()
        {
            var properties = target.GetType().GetProperties(BindingAttr);
            var copyIcon = EditorGUIUtility.IconContent("Clipboard");
            var dict = properties.ToDictionary(i => i, i=> i.GetGetMethod(true).ReturnType);
            var query = dict
                .Where(i => i.Value.IsGenericType)
                .Where(i => i.Value.GetGenericTypeDefinition().IsEquivalentTo(typeof(ObservableData<>)))
                .ToArray();
            if (query.Any())
            {
                var oddSkin = GUI.skin.label;
                oddSkin.padding = new RectOffset(2, 2, 2, 2);
                var evenSkin = GUI.skin.box;
                evenSkin.padding = new RectOffset(2, 2, 2, 2);
                var odd = true;

                GUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUI.indentLevel++;
                DisplayData = EditorGUILayout.Foldout(DisplayData, "Data:");
                if (DisplayData)
                {
                    foreach (var kv in query)
                    {
                        var item = kv.Key;
                        var info = item.GetGetMethod(true);
                        var type = kv.Value;

                        odd = !odd;
                        GUILayout.BeginHorizontal(odd ? oddSkin : evenSkin);

                        if (GUILayout.Button(copyIcon, GUILayout.Width(24)))
                            EditorGUIUtility.systemCopyBuffer = item.Name;
                        try
                        {
                            var typeArgument = type.GenericTypeArguments[0];
                            var value = "";
                            if (info.Invoke(target, null) is AObservableProperty observable)
                                value = observable.GetValue();
                            GUILayout.Label($"{item.Name} ({typeArgument.Name})", GUILayout.MinWidth(100));
                            GUILayout.FlexibleSpace();
                            GUILayout.TextField(value, GUILayout.MinWidth(100));

                        }
                        catch (Exception e)
                        {
                            GUILayout.TextField($"{item.Name} ({e})", GUILayout.Height(20));
                        }
                        GUILayout.EndHorizontal();
                    }        
                }
                EditorGUI.indentLevel--;
                GUILayout.EndVertical();
            }
        }
    
        public bool DisplayCommand { get; set; } = true;
        private void ListCommands()
        {
            var properties = target.GetType().GetProperties(BindingAttr);
            var copyIcon = EditorGUIUtility.IconContent("Clipboard");
            var runIcon = EditorGUIUtility.IconContent("PlayButton");
            var dict = properties.ToDictionary(i => i, i=> i.GetGetMethod(true).ReturnType);
            var query = dict
                .Where(i => i.Value == typeof(ObservableCommand))
                .ToArray();
            if (query.Any())
            {
                var oddSkin = GUI.skin.label;
                oddSkin.padding = new RectOffset(2, 2, 2, 2);
                var evenSkin = GUI.skin.box;
                evenSkin.padding = new RectOffset(2, 2, 2, 2);
                var odd = true;

                GUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUI.indentLevel++;
                DisplayCommand = EditorGUILayout.Foldout(DisplayCommand, "Commands:");
                if (DisplayCommand)
                {
                    foreach (var kv in query)
                    {
                        var item = kv.Key;
                        var info = item.GetGetMethod(true);
                        var type = kv.Value;
                    
                        odd = !odd;
                        GUILayout.BeginHorizontal(odd ? oddSkin : evenSkin);

                        if (GUILayout.Button(copyIcon, GUILayout.Width(24)))
                            EditorGUIUtility.systemCopyBuffer = item.Name;
                        GUILayout.Label($"{item.Name}", GUILayout.MinWidth(100));
                        try
                        {
                            if (GUILayout.Button(runIcon, GUILayout.Width(24)))
                                if (info.Invoke(target, Array.Empty<object>()) is ObservableCommand cmd)
                                    cmd.Emit();
                        }
                        catch (Exception e)
                        {
                            GUILayout.TextField($"{item.Name} ({e})", GUILayout.Height(20));
                        }
                        GUILayout.EndHorizontal();
                    }        
                }
                EditorGUI.indentLevel--;
                GUILayout.EndVertical();
            }
        }

        public bool DisplayCommandArg { get; set; } = true;
        private void ListCommandsWithArg()
        {
            var properties = target.GetType().GetProperties(BindingAttr);
            var copyIcon = EditorGUIUtility.IconContent("Clipboard");
            var dict = properties.ToDictionary(i => i, i=> i.GetGetMethod(true).ReturnType);
            var query = dict
                .Where(i => i.Value.IsGenericType)
                .Where(i => i.Value.GetGenericTypeDefinition().IsEquivalentTo(typeof(ObservableCommand<>)))
                .ToArray();
            if (query.Any())
            {
                var oddSkin = GUI.skin.label;
                oddSkin.padding = new RectOffset(2, 2, 2, 2);
                var evenSkin = GUI.skin.box;
                evenSkin.padding = new RectOffset(2, 2, 2, 2);
                var odd = true;

                GUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUI.indentLevel++;
                DisplayCommandArg = EditorGUILayout.Foldout(DisplayCommandArg, "Commands<T>:");
                if (DisplayCommandArg)
                {
                    foreach (var kv in query)
                    {
                        var item = kv.Key;
                        var type = kv.Value;
                    
                        odd = !odd;
                        GUILayout.BeginHorizontal(odd ? oddSkin : evenSkin);
                    
                        if (GUILayout.Button(copyIcon, GUILayout.Width(24)))
                            EditorGUIUtility.systemCopyBuffer = item.Name;
                        try
                        {
                            var typeArgument = type.GenericTypeArguments[0];
                            GUILayout.Label($"{item.Name} ({typeArgument.Name})", GUILayout.MinWidth(100));
                        }
                        catch (Exception e)
                        {
                            GUILayout.TextField($"{item.Name} ({e})", GUILayout.Height(20));
                        }
                        GUILayout.EndHorizontal();
                    }        
                }
                EditorGUI.indentLevel--;
                GUILayout.EndVertical();
            }
        }
    }
}