using System;
using System.Collections.Generic;
using UnityEngine;

namespace Servable.Runtime.ObservableProperty
{
    [Serializable]
    public class ObservableData<T>: AObservableData
    {
        [SerializeField]
        private T _value;
        
        private Action<T> _onValue;
        
        public ObservableData(T value = default) => _value = value;
        public T Value
        {
            get => _value;
            set 
            {
                if (EqualityComparer<T>.Default.Equals(_value, value)) return;
                _value = value;
                Touch();
            }
        }
        
        public void Touch()
        {
            try
            {
                _onValue?.Invoke(_value);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        
        public void AddListener(Action<T> listener)
        {
            if (listener == null) return;
            _onValue -= listener;
            _onValue += listener;
            listener.Invoke(_value);
        }

        public void RemoveListener(Action<T> listener)
        {
            _onValue -= listener;
        }

        public override string GetValue()
        {
            return Equals(_value, null) ? "null" : _value.ToString();
        }

        public override void AddListener(Delegate handler)
        {
            if (handler == null) return;
            if (handler is Action<T> typed)
            {
                _onValue -= typed;
                _onValue += typed;
                typed(_value);
                return;
            }
            var d = (Action<T>)Delegate.CreateDelegate(typeof(Action<T>), handler.Target, handler.Method);
            _onValue -= d;
            _onValue += d;
            d(_value);
        }

        public override void RemoveListener(Delegate handler)
        {
            if (handler == null) return;
            if (handler is Action<T> typed)
            {
                _onValue -= typed;
                return;
            }
            var d = (Action<T>)Delegate.CreateDelegate(typeof(Action<T>), handler.Target, handler.Method);
            _onValue -= d;
        }
    }
}