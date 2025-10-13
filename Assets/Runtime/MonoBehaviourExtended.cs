using System;
using System.Reflection;
using Servable.Runtime.Attributes;
using Servable.Runtime.Extension;
using Servable.Runtime.ObservableProperty;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Servable.Runtime
{
    public abstract class MonoBehaviourExtended : MonoBehaviorLifetimeAttributes
    {
        [OnAwake]
        internal void AttachBindings()
        {
            var type = GetType();
            var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var methodInfo in methods)
            {
                foreach (var attr in methodInfo.GetCustomAttributes<OnDataAttribute>(true))
                {
                    var info = type.GetProperty(attr.PropertyName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                    if (info == null) continue;
                    
                    var data = info.GetValue(this) as AObservableProperty;
                    if (data == null) continue;
                    
                    var parameters = methodInfo.GetParameters();
                    if (parameters.Length != 1) continue;
                    
                    var dataType = parameters[0].ParameterType;
                    var actionType = typeof(Action<>).MakeGenericType(dataType);
                    var handler = Delegate.CreateDelegate(actionType, this, methodInfo, false);
                    if (handler == null) continue;
                    
                    var addListener = data.GetType().GetMethod("AddListener", BindingFlags.Instance | BindingFlags.Public);
                    if (addListener == null) continue;
                    
                    addListener.Invoke(data, new object[] { handler });
                }
            
                foreach (var attr in methodInfo.GetCustomAttributes<OnCommandAttribute>(true))
                {
                    var info = type.GetProperty(attr.PropertyName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                    if (info == null) continue;
                    
                    var cmd = info.GetValue(this) as AObservableProperty;
                    if (cmd == null) continue;
                    
                    var parameters = methodInfo.GetParameters();
                    switch (parameters.Length)
                    {
                        case 0:
                        {
                            var handler = Delegate.CreateDelegate(typeof(Action), this, methodInfo, false) as Action;
                            if (handler == null) continue;

                            var addListener = cmd.GetType().GetMethod("AddListener", BindingFlags.Instance | BindingFlags.Public);
                            if (addListener == null) continue;
                            
                            addListener.Invoke(cmd, new object[] { handler });
                            break;
                        }
                        
                        case 1:
                        {
                            var payloadType = parameters[0].ParameterType;
                            var actionType = typeof(Action<>).MakeGenericType(payloadType);
                            
                            var handler = Delegate.CreateDelegate(actionType, this, methodInfo, false);
                            if (handler == null) continue;
                        
                            var addListener = cmd.GetType().GetMethod("AddListener", BindingFlags.Instance | BindingFlags.Public);
                            if (addListener == null) continue;
                            
                            addListener.Invoke(cmd, new object[] { handler });
                            break;
                        }
                    }
                }
            }
        }

        [OnDestroy]
        internal void DetachBindings()
        {
            var type = GetType();
            var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var methodInfo in methods)
            {
                foreach (var attr in methodInfo.GetCustomAttributes<OnDataAttribute>(true))
                {
                    var info = type.GetProperty(attr.PropertyName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                    if (info == null) continue;
                    
                    var data = info.GetValue(this) as AObservableProperty;
                    if (data == null) continue;

                    var parameters = methodInfo.GetParameters();
                    if (parameters.Length != 1) continue;
                    
                    var dataType = parameters[0].ParameterType;
                    var actionType = typeof(Action<>).MakeGenericType(dataType);
                    var handler = Delegate.CreateDelegate(actionType, this, methodInfo, false);
                    if (handler == null) continue;
                    
                    var removeListener = data.GetType().GetMethod("RemoveListener", BindingFlags.Instance | BindingFlags.Public);
                    if (removeListener == null) continue;
                    
                    removeListener.Invoke(data, new object[] { handler });
                }
                
                foreach (var attr in methodInfo.GetCustomAttributes<OnCommandAttribute>(true))
                {
                    var info = type.GetProperty(attr.PropertyName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                    if (info == null) continue;
                    
                    var cmd = info.GetValue(this) as AObservableProperty;
                    if (cmd == null) continue;   

                    var parameters = methodInfo.GetParameters();
                    switch (parameters.Length)
                    {
                        case 0:
                        {
                            var handler = Delegate.CreateDelegate(typeof(Action), this, methodInfo, false) as Action;
                            if (handler == null) continue;
                        
                            var removeListener = cmd.GetType().GetMethod("RemoveListener", BindingFlags.Instance | BindingFlags.Public);
                            if (removeListener == null) continue;
                    
                            removeListener.Invoke(cmd, new object[] { handler });
                            break;
                        }
                        
                        case 1:
                        {
                            var payloadType = parameters[0].ParameterType;
                            var actionType = typeof(Action<>).MakeGenericType(payloadType);
                            
                            var handler = Delegate.CreateDelegate(actionType, this, methodInfo, false);
                            if (handler == null) continue;
                        
                            var removeListener = cmd.GetType().GetMethod("RemoveListener", BindingFlags.Instance | BindingFlags.Public);
                            if (removeListener == null) continue;
                    
                            removeListener.Invoke(cmd, new object[] { handler });
                            break;
                        }
                    }
                }
            }
        }
    }
}