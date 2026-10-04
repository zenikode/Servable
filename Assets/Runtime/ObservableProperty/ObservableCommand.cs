using System;
using UnityEngine;

namespace Servable.Runtime.ObservableProperty
{
    public class ObservableCommand: AObservableProperty
    {
        private Action _onCommand;
        
        public virtual void AddListener(Action listener)
        {
            if (listener == null) return;
            _onCommand -= listener;
            _onCommand += listener;
        }

        public void RemoveListener(Action listener)
        {
            _onCommand -= listener;
        }

        public void Emit()
        {
            try
            {
                _onCommand?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public override void AddListener(Delegate handler)
        {
            if (handler == null) return;
            if (handler is Action typed)
            {
                _onCommand -= typed;
                _onCommand += typed;
                return;
            }
            var d = (Action)Delegate.CreateDelegate(typeof(Action), handler.Target, handler.Method);
            _onCommand -= d;
            _onCommand += d;
        }

        public override void RemoveListener(Delegate handler)
        {
            if (handler == null) return;
            if (handler is Action typed)
            {
                _onCommand -= typed;
                return;
            }
            var d = (Action)Delegate.CreateDelegate(typeof(Action), handler.Target, handler.Method);
            _onCommand -= d;
        }
    }
    
    public class ObservableCommand<TPayload>: AObservableProperty
    {
        private Action<TPayload> _onCommand;
       

        public void AddListener(Action<TPayload> listener)
        {
            if (listener == null) return;
            _onCommand -= listener;
            _onCommand += listener;
        }

        public void RemoveListener(Action<TPayload> listener)
        {
            _onCommand -= listener;
        }
        
        public void Emit(TPayload payload)
        {
            try
            {
                _onCommand?.Invoke(payload);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public override void AddListener(Delegate handler)
        {
            if (handler == null) return;
            if (handler is Action<TPayload> typed)
            {
                _onCommand -= typed;
                _onCommand += typed;
                return;
            }
            var d = (Action<TPayload>)Delegate.CreateDelegate(typeof(Action<TPayload>), handler.Target, handler.Method);
            _onCommand -= d;
            _onCommand += d;
        }

        public override void RemoveListener(Delegate handler)
        {
            if (handler == null) return;
            if (handler is Action<TPayload> typed)
            {
                _onCommand -= typed;
                return;
            }
            var d = (Action<TPayload>)Delegate.CreateDelegate(typeof(Action<TPayload>), handler.Target, handler.Method);
            _onCommand -= d;
        }
    }
}