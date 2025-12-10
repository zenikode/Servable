using System;
using System.Reflection;
using Servable.Runtime.Attributes;
using UnityEngine;

namespace Servable.Runtime
{
    public abstract class MonoBehaviorLifetimeAttributes : MonoBehaviourAttributeDummy
    {
        private void CallAttributedMethods<T>() where T: Attribute
        {
            var type = GetType();
            var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var methodInfo in methods)
                if (methodInfo.GetCustomAttribute<T>(true) != null)
                {
                    if (methodInfo.IsPrivate)
                    {
                        Debug.LogWarning($"{typeof(T).Name} is not allowed on private methods.");
                        continue;
                    }
                    if (methodInfo.GetParameters().Length == 0)
                        methodInfo.Invoke(this, null);

                }
        }
        
        protected sealed override void Awake() => CallAttributedMethods<OnAwakeAttribute>();
        protected sealed override void OnEnable() => CallAttributedMethods<OnEnableAttribute>();
        protected sealed override void OnDisable() => CallAttributedMethods<OnDisableAttribute>();
        protected sealed override void OnDestroy() => CallAttributedMethods<OnDestroyAttribute>();

    }
}