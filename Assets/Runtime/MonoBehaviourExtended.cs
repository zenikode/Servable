using System;
using System.Collections.Generic;
using System.Reflection;
using Servable.Runtime.Attributes;
using Servable.Runtime.ObservableProperty;
using UnityEngine;

namespace Servable.Runtime
{
    public abstract class MonoBehaviourExtended : MonoBehaviorLifetimeAttributes
    {
        private readonly struct ObserveBinding
        {
            public readonly AObservableProperty Observable;
            public readonly Delegate Handler;

            public ObserveBinding(AObservableProperty observable, Delegate handler)
            {
                Observable = observable;
                Handler = handler;
            }
        }

        private List<ObserveBinding> _observeBindings;

        [OnAwake]
        internal void AttachBindings()
        {
            var type = GetType();
            var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var bindings = new List<ObserveBinding>();

            foreach (var methodInfo in methods)
            {
                foreach (var attr in methodInfo.GetCustomAttributes<ObserveAttribute>(true))
                {
                    if (methodInfo.IsPrivate)
                    {
                        Debug.LogWarning($"OnCommand Attribute is not allowed on private methods.");
                        continue;
                    }

                    var property = type.GetProperty(attr.PropertyName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                    if (property == null) continue;

                    var observable = property.GetValue(this) as AObservableProperty;
                    if (observable == null) continue;

                    var handler = BuildHandler(methodInfo, this);
                    if (handler == null) continue;

                    observable.AddListener(handler);
                    bindings.Add(new ObserveBinding(observable, handler));
                }
            }

            _observeBindings = bindings;
        }

        [OnDestroy]
        internal void DetachBindings()
        {
            if (_observeBindings == null) return;

            foreach (var binding in _observeBindings)
                binding.Observable.RemoveListener(binding.Handler);

            _observeBindings = null;
        }

        private static Delegate BuildHandler(MethodInfo methodInfo, object target)
        {
            var parameters = methodInfo.GetParameters();
            switch (parameters.Length)
            {
                case 0:
                    return Delegate.CreateDelegate(typeof(Action), target, methodInfo, false) as Action;

                case 1:
                {
                    var payloadType = parameters[0].ParameterType;
                    var actionType = typeof(Action<>).MakeGenericType(payloadType);
                    return Delegate.CreateDelegate(actionType, target, methodInfo, false);
                }

                default:
                    return null;
            }
        }
    }
}
