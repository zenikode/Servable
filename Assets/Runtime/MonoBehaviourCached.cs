using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Servable.Runtime
{
    public abstract class MonoBehaviourCached: MonoBehaviour
    {
        private readonly Dictionary<Type, Component> _componentStorage = new();
        private readonly Dictionary<Type, object> _parentStorage = new();
        private readonly Dictionary<Type, object> _childStorage = new();
        
        [UsedImplicitly]
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

        [UsedImplicitly]
        public TComponent ResolveParent<TComponent>() where TComponent : class
        {
            var keyType = typeof(TComponent);
            if (_parentStorage.TryGetValue(keyType, out var result))
                return result as TComponent;
            
            var component = GetComponentInParent<TComponent>(true);
            _parentStorage[keyType] = component;
            return component;
        }

        [UsedImplicitly]
        public TComponent ResolveChild<TComponent>() where TComponent : class
        {
            var keyType = typeof(TComponent);
            if (_childStorage.TryGetValue(keyType, out var result))
                return result as TComponent;
            var component = GetComponentInChildren<TComponent>(true);
            _childStorage[keyType] = component;
            return component;
        }
        
        [UsedImplicitly]
        public Transform Tf => ResolveComponent<Transform>();

        protected abstract void Awake();
        protected abstract void OnEnable();
        protected abstract void OnDisable();
        protected abstract void OnDestroy();

    }
}