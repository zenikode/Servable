using System;
using System.Collections.Generic;
using System.Reflection;
using Servable.Runtime.Attributes;
using UnityEngine;

namespace Servable.Runtime
{
    public abstract class MonoBehaviorLifetimeAttributes : MonoBehaviourAttributeDummy
    {
        private readonly Dictionary<Type, List<MethodInfo>> _lifetimeMethods = new();

        private void CollectAttributedMethods()
        {
            var type = GetType();
            var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var methodInfo in methods)
            foreach (var attr in methodInfo.GetCustomAttributes<Attribute>(true))
            {
                var attrType = attr.GetType();
                if (attrType != typeof(OnAwakeAttribute) &&
                    attrType != typeof(OnEnableAttribute) &&
                    attrType != typeof(OnDisableAttribute) &&
                    attrType != typeof(OnDestroyAttribute))
                    continue;

                if (methodInfo.IsPrivate)
                {
                    Debug.LogWarning($"{attrType.Name} is not allowed on private methods.");
                    continue;
                }

                if (methodInfo.GetParameters().Length != 0)
                    continue;

                if (!_lifetimeMethods.TryGetValue(attrType, out var list))
                {
                    list = new List<MethodInfo>();
                    _lifetimeMethods[attrType] = list;
                }

                list.Add(methodInfo);
            }
        }

        private void CallAttributedMethods(Type attributeType)
        {
            if (!_lifetimeMethods.TryGetValue(attributeType, out var list))
                return;

            foreach (var methodInfo in list)
                methodInfo.Invoke(this, null);
        }

        protected sealed override void Awake()
        {
            CollectAttributedMethods();
            CallAttributedMethods(typeof(OnAwakeAttribute));
        }
        protected sealed override void OnEnable() => CallAttributedMethods(typeof(OnEnableAttribute));
        protected sealed override void OnDisable() => CallAttributedMethods(typeof(OnDisableAttribute));
        protected sealed override void OnDestroy() => CallAttributedMethods(typeof(OnDestroyAttribute));

    }
}
