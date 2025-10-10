using System;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using Servable.Runtime.Extension;
using Servable.Runtime.ObservableProperty;
using Object = UnityEngine.Object;

namespace Servable.Runtime.Bindings
{
    public abstract class ABinding : MonoBehaviour
    {
        private static MethodInfo GetGenericMethod(Type hostType, string name, int genericArgCount, params Type[] parameterTypes)
        {
            var methods = hostType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
            foreach (var m in methods)
            {
                if (m.Name != name) continue;
                if (!m.IsGenericMethodDefinition) continue;
                if (m.GetGenericArguments().Length != genericArgCount) continue;
                var prms = m.GetParameters();
                if (prms.Length != parameterTypes.Length) continue;
                var match = true;
                for (var i = 0; i < prms.Length; i++)
                {
                    if (prms[i].ParameterType != parameterTypes[i]) { match = false; break; }
                }
                if (match) return m;
            }
            return null;
        }
        public abstract bool IsValid();

        public abstract Object GetModel();
        public Transform Tf => ResolveComponent<Transform>();
        
        private readonly Dictionary<Type, Component> _componentStorage = new();
        public TComponent ResolveComponent<TComponent>(bool createNew = true) where TComponent : Component
        {
            var keyType = typeof(TComponent);
            if (_componentStorage.TryGetValue(keyType, out var result))
                return result as TComponent;
            if (TryGetComponent<TComponent>(out var component))
            {
                _componentStorage[keyType] = component;
                return component;
            }
            
            return createNew ? gameObject.AddComponent<TComponent>() : null;
        }
        
        private readonly Dictionary<Type, object> _parentStorage = new();
        public TComponent ResolveParent<TComponent>() where TComponent : class
        {
            var keyType = typeof(TComponent);
            if (_parentStorage.TryGetValue(keyType, out var result))
                return result as TComponent;
            var component = GetComponentInParent<TComponent>(true);
            _parentStorage[keyType] = component;
            return component;
        }

        private readonly Dictionary<Type, object> _childStorage = new();
        public TComponent ResolveChild<TComponent>() where TComponent : class
        {
            var keyType = typeof(TComponent);
            if (_childStorage.TryGetValue(keyType, out var result))
                return result as TComponent;
            var component = GetComponentInChildren<TComponent>(true);
            _childStorage[keyType] = component;
            return component;
        }

        private void Awake()
        {
            var type = GetType();
            var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var methodInfo in methods)
            {
                if (methodInfo.GetCustomAttribute<Servable.Runtime.OnAwakeAttribute>(true) == null)
                    continue;
                if (methodInfo.GetParameters().Length != 0)
                {
                    Debug.LogError($"[OnAwake] метод {methodInfo.DeclaringType?.Name}.{methodInfo.Name} должен быть без параметров");
                    continue;
                }
                methodInfo.Invoke(this, null);
            }

            // Auto-subscribe: ObservableData
            foreach (var methodInfo in methods)
            {
                var onDataAttrs = methodInfo.GetCustomAttributes<Servable.Runtime.OnDataAttribute>(true);
                foreach (var attr in onDataAttrs)
                {
                    var parameters = methodInfo.GetParameters();
                    if (parameters.Length != 1)
                    {
                        Debug.LogError($"[OnData] метод {methodInfo.DeclaringType?.Name}.{methodInfo.Name} должен иметь один параметр типа данных ObservableData");
                        continue;
                    }
                    var dataType = parameters[0].ParameterType;
                    var getDataGeneric = typeof(ObservablePropertyLocatorExt).GetMethod("GetData").MakeGenericMethod(dataType);
                    var data = getDataGeneric.Invoke(null, new object[] { this as Object, attr.PropertyName }) as AObservableProperty;
                    if (data == null)
                    {
                        Debug.LogWarning($"[OnData] Свойство {attr.PropertyName} не найдено в {GetType().Name}", this);
                        continue;
                    }
                    var actionType = typeof(System.Action<>).MakeGenericType(dataType);
                    var handler = System.Delegate.CreateDelegate(actionType, this, methodInfo, false);
                    if (handler == null)
                    {
                        Debug.LogError($"[OnData] не удалось создать делегат для {methodInfo.Name} с параметром {dataType.Name}", this);
                        continue;
                    }
                    var addListener = data.GetType().GetMethod("AddListener", BindingFlags.Instance | BindingFlags.Public);
                    addListener?.Invoke(data, new object[] { handler });
                }
            }

            // Auto-subscribe: ObservableCommand
            foreach (var methodInfo in methods)
            {
                var onCmdAttrs = methodInfo.GetCustomAttributes<Servable.Runtime.OnCommandAttribute>(true);
                foreach (var attr in onCmdAttrs)
                {
                    var parameters = methodInfo.GetParameters();
                    if (parameters.Length == 0)
                    {
                        var cmd = this.GetCommand(attr.PropertyName);
                        if (cmd == null) { Debug.LogWarning($"[OnCommand] Команда {attr.PropertyName} не найдена в {GetType().Name}", this); continue; }
                        var handler = System.Delegate.CreateDelegate(typeof(System.Action), this, methodInfo, false) as System.Action;
                        if (handler == null)
                        {
                            Debug.LogError($"[OnCommand] не удалось создать делегат Action для {methodInfo.Name}", this);
                            continue;
                        }
                        cmd.AddListener(handler);
                        continue;
                    }

                    if (parameters.Length == 1)
                    {
                        var payloadType = parameters[0].ParameterType;
                        var genericDef = GetGenericMethod(typeof(ObservablePropertyLocatorExt), "GetCommand", 1, typeof(Object), typeof(string));
                        var getCmdGeneric = genericDef?.MakeGenericMethod(payloadType);
                        var cmd = getCmdGeneric.Invoke(null, new object[] { this as Object, attr.PropertyName });
                        if (cmd == null) { Debug.LogWarning($"[OnCommand] Команда<{payloadType.Name}> {attr.PropertyName} не найдена в {GetType().Name}", this); continue; }
                        var actionType = typeof(System.Action<>).MakeGenericType(payloadType);
                        var handler = System.Delegate.CreateDelegate(actionType, this, methodInfo, false);
                        if (handler == null)
                        {
                            Debug.LogError($"[OnCommand] не удалось создать делегат Action<{payloadType.Name}> для {methodInfo.Name}", this);
                            continue;
                        }
                        var addListener = cmd.GetType().GetMethod("AddListener", BindingFlags.Instance | BindingFlags.Public);
                        addListener?.Invoke(cmd, new object[] { handler });
                        continue;
                    }

                    Debug.LogError($"[OnCommand] метод {methodInfo.DeclaringType?.Name}.{methodInfo.Name} должен быть без параметров или с одним параметром payload");
                }
            }
        }

        private void OnEnable()
        {
            var type = GetType();
            var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var methodInfo in methods)
            {
                if (methodInfo.GetCustomAttribute<Servable.Runtime.OnEnableAttribute>(true) == null)
                    continue;
                if (methodInfo.GetParameters().Length != 0)
                {
                    Debug.LogError($"[OnEnable] метод {methodInfo.DeclaringType?.Name}.{methodInfo.Name} должен быть без параметров");
                    continue;
                }
                methodInfo.Invoke(this, null);
            }
        }

        private void OnDisable()
        {
            var type = GetType();
            var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var methodInfo in methods)
            {
                if (methodInfo.GetCustomAttribute<Servable.Runtime.OnDisableAttribute>(true) == null)
                    continue;
                if (methodInfo.GetParameters().Length != 0)
                {
                    Debug.LogError($"[OnDisable] метод {methodInfo.DeclaringType?.Name}.{methodInfo.Name} должен быть без параметров");
                    continue;
                }
                methodInfo.Invoke(this, null);
            }
        }

        private void OnDestroy()
        {
            var type = GetType();
            var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var methodInfo in methods)
            {
                if (methodInfo.GetCustomAttribute<Servable.Runtime.OnDestroyAttribute>(true) == null)
                    continue;
                if (methodInfo.GetParameters().Length != 0)
                {
                    Debug.LogError($"[OnDestroy] метод {methodInfo.DeclaringType?.Name}.{methodInfo.Name} должен быть без параметров");
                    continue;
                }
                methodInfo.Invoke(this, null);
            }

            // Auto-unsubscribe: ObservableData
            var type2 = GetType();
            var methods2 = type2.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var methodInfo in methods2)
            {
                var onDataAttrs = methodInfo.GetCustomAttributes<Servable.Runtime.OnDataAttribute>(true);
                foreach (var attr in onDataAttrs)
                {
                    var parameters = methodInfo.GetParameters();
                    if (parameters.Length != 1) continue;
                    var dataType = parameters[0].ParameterType;
                    var getDataGeneric = typeof(ObservablePropertyLocatorExt).GetMethod("GetData").MakeGenericMethod(dataType);
                    var data = getDataGeneric.Invoke(null, new object[] { this as Object, attr.PropertyName }) as AObservableProperty;
                    if (data == null) continue;
                    var actionType = typeof(System.Action<>).MakeGenericType(dataType);
                    var handler = System.Delegate.CreateDelegate(actionType, this, methodInfo, false);
                    if (handler != null)
                    {
                        try
                        {
                            var removeListener = data.GetType().GetMethod("RemoveListener", BindingFlags.Instance | BindingFlags.Public);
                            removeListener?.Invoke(data, new object[] { handler });
                        }
                        catch (System.Exception e) { Debug.LogException(e, this); }
                    }
                }
            }

            // Auto-unsubscribe: ObservableCommand
            foreach (var methodInfo in methods2)
            {
                var onCmdAttrs = methodInfo.GetCustomAttributes<Servable.Runtime.OnCommandAttribute>(true);
                foreach (var attr in onCmdAttrs)
                {
                    var parameters = methodInfo.GetParameters();
                    if (parameters.Length == 0)
                    {
                        var cmd = this.GetCommand(attr.PropertyName);
                        if (cmd == null) continue;
                        var handler = System.Delegate.CreateDelegate(typeof(System.Action), this, methodInfo, false) as System.Action;
                        if (handler != null)
                        {
                            try { cmd.RemoveListener(handler); } catch (System.Exception e) { Debug.LogException(e, this); }
                        }
                        continue;
                    }
                    if (parameters.Length == 1)
                    {
                        var payloadType = parameters[0].ParameterType;
                        var genericDef = GetGenericMethod(typeof(ObservablePropertyLocatorExt), "GetCommand", 1, typeof(Object), typeof(string));
                        var getCmdGeneric = genericDef?.MakeGenericMethod(payloadType);
                        var cmd = getCmdGeneric.Invoke(null, new object[] { this as Object, attr.PropertyName });
                        if (cmd == null) continue;
                        var actionType = typeof(System.Action<>).MakeGenericType(payloadType);
                        var handler = System.Delegate.CreateDelegate(actionType, this, methodInfo, false);
                        if (handler != null)
                        {
                            try
                            {
                                var removeListener = cmd.GetType().GetMethod("RemoveListener", BindingFlags.Instance | BindingFlags.Public);
                                removeListener?.Invoke(cmd, new object[] { handler });
                            }
                            catch (System.Exception e) { Debug.LogException(e, this); }
                        }
                        continue;
                    }
                }
            }
        }
    }
}