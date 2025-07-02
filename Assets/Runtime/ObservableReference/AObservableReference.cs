using UnityEngine;

namespace Servable.Runtime.ObservableReference
{
    public abstract class AObservableReference<T> where T: class
    {
        public Object viewModel;
        public string property;
        
        //cache
        private float _cacheLifetime;
        private T _observable;
        private T GetObservable()
        {
            if (!viewModel) return null;

            if (_cacheLifetime < Time.realtimeSinceStartup)
            { 
                _observable = null;
            }

            if (_observable == null)
            {
                _observable = GetProperty();
                _cacheLifetime = Time.realtimeSinceStartup + 5;
            } 
            return _observable;
        }
        public T Observable => GetObservable();
        public abstract T GetProperty();
        public bool IsValid() => viewModel != null && Observable != null;
    }
}