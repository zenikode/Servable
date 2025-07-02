using System;
using UnityEngine;

namespace Servable.Runtime.ObservableProperty
{
    public class ObservableCommand
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
    }
    
    public class ObservableCommand<TPayload>
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
    }
}