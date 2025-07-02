using System;
using System.Collections.Generic;
using UnityEngine;

namespace Servable.Runtime
{
    [DisallowMultipleComponent]
    public abstract class ModelBehaviour: MonoBehaviour
    {
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
    }
}