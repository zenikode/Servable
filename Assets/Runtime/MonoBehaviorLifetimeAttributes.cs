using System;
using System.Reflection;
using Servable.Runtime.Attributes;

namespace Servable.Runtime
{
    public abstract class MonoBehaviorLifetimeAttributes : MonoBehaviourCached
    {
        private void CallAttributedMethods<T>() where T: Attribute
        {
            var type = GetType();
            var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var methodInfo in methods)
                if (methodInfo.GetCustomAttribute<T>(true) != null)
                    if (methodInfo.GetParameters().Length == 0)
                        methodInfo.Invoke(this, null);
        }
        
        private void Awake() => CallAttributedMethods<OnAwakeAttribute>();
        private void OnEnable() => CallAttributedMethods<OnEnableAttribute>();
        private void OnDisable() => CallAttributedMethods<OnDisableAttribute>();
        private void OnDestroy() => CallAttributedMethods<OnDestroyAttribute>();

    }
}