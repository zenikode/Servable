using System;
using System.Collections.Generic;
using System.Linq;
using Servable.Runtime;
using Servable.Runtime.Bindings;
using UnityEditor;
using UnityEngine;

namespace Servable.Editor
{
    [InitializeOnLoad]
    internal static class HierarchyTags
    {
        private static GUIStyle _labelStyle;
        public static GUIStyle LabelStyle => _labelStyle ??= new GUIStyle(EditorStyles.label)
        {
            alignment = TextAnchor.LowerRight,
            fontSize = 7,
            richText = true,
            fontStyle = FontStyle.Bold
        };
   
        private static GUIStyle _modelStyle;
        public static GUIStyle ModelStyle => _modelStyle ??= new GUIStyle(EditorStyles.label)
        {
            alignment = TextAnchor.UpperRight,
            fontSize = 9,
            richText = true
        };
    
        static HierarchyTags()
        {
            EditorApplication.hierarchyWindowItemOnGUI -= HighlightItems;
            EditorApplication.hierarchyWindowItemOnGUI += HighlightItems;
        }

        private static void HighlightItems(int instanceID, Rect selectionRect)
        {
            var target = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (target == null) return;
        
            var vms = target.GetComponentsInParent<ModelBehaviour>();
            foreach (var vm in vms)
            { 
                var type = vm.GetType();
                var color = ComputeTypeHashedColor(type);
                var offset = EdgeOffset(target, vm);
            
                var rect = new Rect(selectionRect.x - selectionRect.height - (selectionRect.height - 2) * offset, selectionRect.y, 1, selectionRect.height);
                EditorGUI.DrawRect(rect, color);
            
                if (vm.gameObject == target)
                {
                    var add = selectionRect.height + (selectionRect.height - 2) * offset;
                    var rect1 = new Rect(selectionRect.x - add, selectionRect.y, 8, 1);
                    EditorGUI.DrawRect(rect1, color);
                    var modelTag = GetModelTag(target);
                    GUI.Label(selectionRect, modelTag, ModelStyle);
                }
            }

            var tagsBlack = GetBindingTagsBlack(target);
            var labelTextBlack = ComposeBindingList(tagsBlack);
            var rectS = selectionRect;
            GUI.Label(rectS, labelTextBlack, LabelStyle);
            GUI.Label(rectS, labelTextBlack, LabelStyle);
            GUI.Label(rectS, labelTextBlack, LabelStyle);
            rectS.y += 1;
            rectS.x += 1;
            GUI.Label(rectS, labelTextBlack, LabelStyle);

            var tags = GetBindingTags(target);
            var labelText = ComposeBindingList(tags);
            GUI.Label(selectionRect, labelText, LabelStyle);
            GUI.Label(selectionRect, labelText, LabelStyle);
        }

        private static string ComposeBindingList(string[] tags)
        {
            var dict = new Dictionary<string, int>();
            foreach (var tag in tags)
            {
                dict.TryAdd(tag, 0);
                dict[tag] += 1;
            }

            var result = new List<string>();
            foreach (var pair in dict)
            {
                var entry = pair.Key;
                if (pair.Value > 1)
                    entry += $" ({pair.Value})";
                result.Add(entry);
            }
            return string.Join(", ", result);
        }

        public static Color ComputeTypeHashedColor(Type type)
        {
            var hash = type.Name.GetHashCode();
            hash %= 100;
            var conv = Mathf.Abs(hash * 0.01f);
            var color = Color.HSVToRGB(conv, 0.5f, 1f);
            return color;
        }

        private static string[] GetBindingTags(GameObject target)
        {
            var vimBehaviours = target.GetComponents<ABinding>();
            var names = vimBehaviours.Select(binding =>
            {
                var type = binding.GetType();
                try
                {
                    if (!binding.IsValid())
                    {
                        return $"<color=black>!{type.Name.Replace("Bind","B")}!</color>";
                    }
                }
                catch (Exception)
                {
                    // ignored
                }

                var color = ComputeTypeHashedColor(binding.GetModel().GetType());
                var colorString = ColorUtility.ToHtmlStringRGB(color);
                return $"<color=#{colorString}>{type.Name.Replace("Bind","B")}</color>";
            });

            return names.ToArray();
        }
        private static string[] GetBindingTagsBlack(GameObject target)
        {
            var vimBehaviours = target.GetComponents<ABinding>();
            var names = vimBehaviours.Select(binding =>
            {
                var type = binding.GetType();
                try
                {
                    if (!binding.IsValid())
                    {
                        return $"{type.Name.Replace("Bind","B")}!";
                    }
                }
                catch (Exception)
                {
                    // ignored
                }
                return $"{type.Name.Replace("Bind","B")}";
            });

            return names.ToArray();
        }

        private static string GetModelTag(GameObject target) => target.GetComponents<ModelBehaviour>()
            .Select(component => component.GetType())
            .Select(type =>
            {
                if (type.IsSubclassOf(typeof(ModelBehaviour)))
                {
                    var color = ComputeTypeHashedColor(type);
                    var colorString = ColorUtility.ToHtmlStringRGB(color);
                    return $"<color=#{colorString}>\u25cf</color>";
                }
                return type.Name;
            })
            .FirstOrDefault();
    
        public static int EdgeOffset(GameObject go, ModelBehaviour vm)
        {
            var result = 0;
            while (go!=vm.gameObject)
            {
                go = go.transform.parent.gameObject;
                result += 1;
            }
            return result;
        }
    }
}